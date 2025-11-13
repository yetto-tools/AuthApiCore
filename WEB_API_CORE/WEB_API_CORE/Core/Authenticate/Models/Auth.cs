namespace WEB_API_CORE.Core.Authenticate.Models
{
    public class Usuario
    {
        public int Id { get; set; }                    // Identity interno
        public Guid UserRef { get; set; }              // GUID público
        public string Nombre { get; set; } = "";
        public string Email { get; set; } = "";
        public string PasswordHash { get; set; } = "";
        public string Rol { get; set; } = "User";
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
    public class AuthToken
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string AccessToken { get; set; } = "";
        public string RefreshToken { get; set; } = "";
        public DateTime? ExpiraAccess { get; set; }
        public DateTime ExpiraRefresh { get; set; }
        public bool Revocado { get; set; } = false;
    }

    public class ResetToken
    {
        public Guid Id { get; set; }
        public Guid UsuarioId { get; set; }
        public string Token { get; set; } = "";
        public DateTime Expira { get; set; }
        public bool Usado { get; set; } = false;
    }
}
