namespace WEB_API_CORE.Servicios.JWT
{
    public class JwtSettings
    {
        public string Key { get; set; } = string.Empty;
        public string Issuer { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
        public int ExpiresInMinutes { get; set; } = 60;           // Token de acceso
        public int RefreshTokenExpiresInDays { get; set; } = 7;   // Refresh token (por defecto 7 días)
    }

    public class JwtTokenResult
    {
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiresAt { get; set; }
        public DateTime RefreshTokenExpiresAt { get; set; }
    }
}
