namespace UserService.Api.Middleware
{
    public static class ClientCancellationLoggingExtensions
    {
        public static IApplicationBuilder UseClientCancellationLogging(this IApplicationBuilder app)
        {
            return app.UseMiddleware<ClientCancellationLogginMiddleware>();
        }


    }
}
