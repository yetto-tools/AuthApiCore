using System.ComponentModel.DataAnnotations;

namespace WEB_API_CORE.Core.Authenticate.Models
{
    public class LoginRequest
    {
        public string Username { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class RefreshRequest
    {
        public string RefreshToken { get; set; } = "";
    }

    public record RegisterRequest
    {
        [Required]
        public string Nombre { get; set; }
        
        [Required]
        public string Email { get; set; }
        
        [Required]
        public  string Password { get; set; }
    }

    
}
