namespace ContaFacil.Utilities
{
    public class SessionTimeoutMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SessionTimeoutMiddleware> _logger;

        public SessionTimeoutMiddleware(RequestDelegate next, IConfiguration configuration, ILogger<SessionTimeoutMiddleware> logger)
        {
            _next = next;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path;
            _logger.LogInformation($"Processing request for path: {path}");

            var publicPaths = _configuration.GetSection("PublicPaths").Get<List<string>>() ?? new List<string>();
            publicPaths.AddRange(new[] { "/", "/Login", "/Login/RecoverPassword", "/Login/ChangePassword" });

            _logger.LogInformation($"Public paths: {string.Join(", ", publicPaths)}");

            // Permitir acceso a rutas públicas y recursos estáticos
            if (publicPaths.Any(p => path.StartsWithSegments(p, StringComparison.OrdinalIgnoreCase)) ||
                path.StartsWithSegments("/lib") ||
                path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/img"))
            {
                _logger.LogInformation($"Path {path} is public or static resource. Allowing access.");
                await _next(context);
                return;
            }

            _logger.LogInformation($"Path {path} is not public. Checking for authentication.");

            // Verificar la autenticación para otras rutas
            string idUsuario = context.Session.GetString("_idUsuario");
            if (string.IsNullOrEmpty(idUsuario))
            {
                _logger.LogInformation($"User not authenticated. Redirecting to login.");
                context.Response.Redirect("/Login");
                return;
            }

            _logger.LogInformation($"User authenticated. Allowing access to {path}.");
            await _next(context);
        }
    }
}
