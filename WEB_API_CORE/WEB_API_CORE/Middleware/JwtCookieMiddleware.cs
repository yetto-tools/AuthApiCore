namespace WEB_API_CORE.Middleware
{
    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Si existe cookie de token, la inyectamos al header Authorization
            if (context.Request.Cookies.TryGetValue("access_token", out var token))
            {
                context.Request.Headers["Authorization"] = $"Bearer {token}";
            }

            await _next(context);
        }
    }

}
