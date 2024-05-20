using LoggerEnhancer.Abstractions;

namespace sample.Middlewares
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class EnhancerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IContext _loggerContext;

        public EnhancerMiddleware(RequestDelegate next, IContext loggerContext)
        {
            _next = next;
            _loggerContext = loggerContext;
        }

        public Task Invoke(HttpContext httpContext)
        {
            var pairs = new Dictionary<string, string>();
            pairs.Add("IsAuth", httpContext.User.Identity.IsAuthenticated.ToString());

            _loggerContext.ContextId = Guid.NewGuid().ToString();
            _loggerContext.Pairs = pairs;

            return _next(httpContext);
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class EnhancerMiddlewareExtensions
    {
        public static IApplicationBuilder UseEnhancer(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<EnhancerMiddleware>();
        }
    }
}
