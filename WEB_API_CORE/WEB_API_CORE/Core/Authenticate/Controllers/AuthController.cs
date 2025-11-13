using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using WEB_API_CORE.Core.Authenticate.Models;
using WEB_API_CORE.Middleware;
using WEB_API_CORE.Servicios.Auth;
using WEB_API_CORE.Servicios.JWT;

namespace WEB_API_CORE.Core.Authenticate.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly JwtService _jwt;
        private readonly AuthService _auth;

        // Simulación: tabla temporal para refresh tokens.
        private static readonly Dictionary<string, string> RefreshTokens = new();

        public AuthController(JwtService jwt, AuthService auth)
        {
            _jwt = jwt ?? throw new ArgumentNullException(nameof(jwt));
            _auth = auth ?? throw new ArgumentNullException(nameof(auth));
        }

        // 🔹 Registro de usuario
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest req)
        {
            var ds = await _auth.RegistrarUsuarioAsync(req.Nombre, req.Email, req.Password);
            return Ok(ds.Tables[0]);
        }

        // 🔹 LOGIN: genera JWT + Refresh Token
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest req)
        {
            var ds = await _auth.ValidarLoginAsync(req.Username, req.Password);

            // 🔹 Validar que exista al menos una tabla y una fila
            if (ds == null || ds.Tables.Count == 0 || ds.Tables[0].Rows.Count == 0)
            {
                return Unauthorized(new { error = "Credenciales incorrectas o usuario no encontrado." });
            }

            var row = ds.Tables[0].Rows[0];

            if (!ds.Tables[0].Columns.Contains("UserRef"))
            {
                return StatusCode(401, new { error = "Sin Accesos para Autenticar" });
            }
            // 🔹 Validar que las columnas esperadas existan
            if (
                !ds.Tables[0].Columns.Contains("Email") ||
                !ds.Tables[0].Columns.Contains("Nombre"))
            {
                return StatusCode(500, new { error = "Estructura de datos inesperada en la respuesta del servidor." });
            }

            // 🔹 Crear modelo de usuario
            var usuario = new Usuario
            {
                Id = Convert.ToInt32(row["Id"]),
                UserRef = Guid.Parse(row["UserRef"].ToString() ?? Guid.Empty.ToString()),
                Nombre = row["Nombre"].ToString() ?? "",
                Email = row["Email"].ToString() ?? "",
                Rol = row["Rol"].ToString() ?? "User",
                Activo = Convert.ToBoolean(row["Activo"]),
            };

            // 🔹 Generar tokens
            var jwtResult = _jwt.GenerateTokens(usuario.Id.ToString(), usuario.Nombre, usuario.Rol);

            // 🔹 Establecer cookies HttpOnly
            Response.Cookies.Append("access_token", jwtResult.AccessToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwtResult.AccessTokenExpiresAt
            });

            Response.Cookies.Append("refresh_token", jwtResult.RefreshToken, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Expires = jwtResult.RefreshTokenExpiresAt
            });

            // 🔹 Devolver respuesta
            return Ok(new
            {
                usuario.Id,
                usuario.UserRef,
                usuario.Nombre,
                usuario.Email,
                usuario.Rol,
                usuario.Activo,
                token = new { resfresh = jwtResult.RefreshToken }
            });
        }


        // 🔹 REFRESH: genera un nuevo par de tokens
        [HttpPost("refresh")]
        public IActionResult Refresh([FromBody] RefreshRequest request)
        {
            if (!RefreshTokens.ContainsKey(request.RefreshToken))
                return Unauthorized(new { message = "Refresh token inválido o expirado" });

            string email = RefreshTokens[request.RefreshToken];
            RefreshTokens.Remove(request.RefreshToken); // Invalida el anterior

            // Aquí deberías recuperar el usuario real desde la BD si fuese necesario
            var tokens = _jwt.RefreshTokens("1", email, "User");

            RefreshTokens[tokens.RefreshToken] = email;

            return Ok(new
            {
                tokens.AccessToken,
                tokens.RefreshToken,
                tokens.AccessTokenExpiresAt,
                tokens.RefreshTokenExpiresAt
            });
        }


        // 🔹 Endpoint protegido: obtiene perfil del usuario logueado
        [Authorize]
        [RequireJwtCookie] // ✅ Este filtro asegura que exista la cookie
        [HttpGet("perfil")]
        public async Task<IActionResult> Perfil()
        {
            var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(idClaim))
                return Unauthorized(new { message = "Usuario no autenticado" });

            if (!int.TryParse(idClaim, out int userId))
                return BadRequest(new { message = "ID de usuario inválido" });

            var user = await _auth.ObtenerUsuarioPorIdAsync(userId);
            if (user == null)
                return NotFound(new { message = "Usuario no encontrado" });

            return Ok(new
            {
                id = user.Id,
                userRef = user.UserRef,
                nombre = user.Nombre,
                email = user.Email,
                rol = user.Rol,
                activo = user.Activo
            });
        }




        [HttpPost("logout")]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");
            return Ok(new { message = "Sesión cerrada correctamente" });
        }

    }
}
