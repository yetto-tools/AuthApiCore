using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WEB_API_CORE.Core.Config;
using WEB_API_CORE.Middleware;
using WEB_API_CORE.Servicios.Auth;
using WEB_API_CORE.Servicios.DataBase;
using WEB_API_CORE.Servicios.JWT;


var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// =======================================================
// 1️⃣ CONFIGURACIÓN GENERAL
// =======================================================

// JWT Settings desde appsettings.json
builder.Services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));

// CORS Settings desde appsettings.json
builder.Services.Configure<CorsSettings>(configuration.GetSection("CorsSettings"));

// =======================================================
// SERVICIOS PERSONALIZADOS
// =======================================================
builder.Services.AddSingleton<DataBaseService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<JwtService>();

// =======================================================
// 3️⃣ SERIALIZACIÓN GLOBAL (Newtonsoft.Json)
// =======================================================
builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
        options.SerializerSettings.Formatting = Formatting.Indented;
    });

// =======================================================
// AUTENTICACIÓN JWT
// =======================================================
var jwtSettings = configuration.GetSection("JwtSettings").Get<JwtSettings>()!;
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = true;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero // No margen de expiración adicional
    };
    // ✅ Lee el token desde la cookie
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            if (context.Request.Cookies.ContainsKey("access_token"))
                context.Token = context.Request.Cookies["access_token"];
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// =======================================================
// CORS CONFIGURABLE DESDE APPSETTINGS
// =======================================================
var corsSettings = configuration.GetSection("CorsSettings").Get<CorsSettings>()!;
builder.Services.AddCors(options =>
{
    options.AddPolicy("CorsPolicy", policy =>
    {
        policy.WithOrigins(corsSettings.AllowedOrigins)
              .WithHeaders(corsSettings.AllowedHeaders)
              .WithMethods(corsSettings.AllowedMethods);

        if (corsSettings.AllowCredentials)
            policy.AllowCredentials();
        else
            policy.DisallowCredentials();
    });
});

// =======================================================
//  SWAGGER + JWT SUPPORT
// =======================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "WEB_API_CORE", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Ejemplo: \"Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});



// =======================================================
// CONSTRUIR APP
// =======================================================
var app = builder.Build();


//  JWT + HttpOnly Cookies

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});


// =======================================================
// PIPELINE DE MIDDLEWARE
// =======================================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ Activar CORS antes de autenticación
app.UseCors("CorsPolicy");

// ✅ Importante: primero autenticación, luego autorización
app.UseMiddleware<JwtCookieMiddleware>(); // auth por medio del httponly cookies
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
