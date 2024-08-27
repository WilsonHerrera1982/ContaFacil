namespace ContaFacil.Utilities
{
    public static class SessionTimeoutMiddlewareExtensions
    {
        public static IApplicationBuilder UseSessionTimeout(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<SessionTimeoutMiddleware>();
        }
    }
}
