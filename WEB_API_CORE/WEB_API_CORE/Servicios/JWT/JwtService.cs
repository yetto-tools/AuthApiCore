using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace WEB_API_CORE.Servicios.JWT
{
    /// <summary>
    /// Servicio centralizado para generación y validación de tokens JWT.
    /// Incluye soporte para Refresh Tokens.
    /// </summary>
    public sealed class JwtService
    {
        private readonly JwtSettings _settings;
        private readonly byte[] _key;

        public JwtService(IOptions<JwtSettings> options)
        {
            _settings = options.Value;
            _key = Encoding.UTF8.GetBytes(_settings.Key);
        }

        /// <summary>
        /// Genera un par de AccessToken + RefreshToken.
        /// </summary>
        public JwtTokenResult GenerateTokens(string userId, string userName, string role, IDictionary<string, string>? extraClaims = null)
        {
            var now = DateTime.UtcNow;

            // ======= Access Token =======
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.UniqueName, userName),
                new Claim(ClaimTypes.Role, role),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            if (extraClaims != null)
            {
                foreach (var pair in extraClaims)
                    claims.Add(new Claim(pair.Key, pair.Value));
            }

            var creds = new SigningCredentials(new SymmetricSecurityKey(_key), SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _settings.Issuer,
                audience: _settings.Audience,
                claims: claims,
                expires: now.AddMinutes(_settings.ExpiresInMinutes),
                signingCredentials: creds
            );

            string accessToken = new JwtSecurityTokenHandler().WriteToken(token);

            // ======= Refresh Token =======
            string refreshToken = GenerateSecureRefreshToken();
            DateTime refreshExpires = now.AddDays(_settings.RefreshTokenExpiresInDays);

            return new JwtTokenResult
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiresAt = token.ValidTo,
                RefreshTokenExpiresAt = refreshExpires
            };
        }

        /// <summary>
        /// Valida el access token y devuelve los claims si es válido.
        /// </summary>
        public ClaimsPrincipal? ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(_key),
                    ClockSkew = TimeSpan.Zero
                };

                var principal = tokenHandler.ValidateToken(token, parameters, out _);
                return principal;
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Genera un nuevo par de tokens usando un refresh token válido.
        /// Aquí normalmente validarías el refresh en base de datos.
        /// </summary>
        public JwtTokenResult RefreshTokens(string userId, string userName, string role, IDictionary<string, string>? extraClaims = null)
        {
            // En una implementación real deberías validar que el refresh token sigue activo en la BD.
            return GenerateTokens(userId, userName, role, extraClaims);
        }

        /// <summary>
        /// Genera un Refresh Token aleatorio seguro.
        /// </summary>
        private static string GenerateSecureRefreshToken()
        {
            var randomBytes = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
