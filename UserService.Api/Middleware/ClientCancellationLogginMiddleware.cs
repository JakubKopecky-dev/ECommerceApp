namespace UserService.Api.Middleware
{
    public sealed class ClientCancellationLogginMiddleware(RequestDelegate next, ILogger<ClientCancellationLogginMiddleware> logger)
    {
        private readonly RequestDelegate _next = next;
        private readonly ILogger<ClientCancellationLogginMiddleware> _logger = logger;



        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException) when (context.RequestAborted.IsCancellationRequested)
            {
                _logger.LogInformation("Reqeust cancelled by client. TraceId={TraceId}, Path={Path}",context.TraceIdentifier,context.Request.Path);
            }
        }



    }
}
