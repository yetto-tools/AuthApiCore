using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace WEB_API_CORE.Middleware
{
    public class RequireJwtCookieAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var cookies = context.HttpContext.Request.Cookies;
            if (!cookies.ContainsKey("access_token"))
            {
                context.Result = new UnauthorizedObjectResult(new { message = "Token requerido" });
                return;
            }
            base.OnActionExecuting(context);
        }
    }
    
}
