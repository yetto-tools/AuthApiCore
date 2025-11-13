namespace WEB_API_CORE.Core.Config
{
    public class CorsSettings
    {
        public string[] AllowedOrigins { get; set; } = [];
        public string[] AllowedMethods { get; set; } = [];
        public string[] AllowedHeaders { get; set; } = [];
        public bool AllowCredentials { get; set; }
    }
}
