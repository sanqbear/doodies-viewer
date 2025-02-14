using Microsoft.AspNetCore.Diagnostics;

namespace DoodieViewer.Server.Middleware
{
    public class ServerExceptionHandler : IExceptionHandler
    {
        private readonly ILogger logger;
        private readonly IConfiguration configuration;

        public ServerExceptionHandler(ILogger<ServerExceptionHandler> logger, IConfiguration configuration)
        {
            this.logger = logger;
            this.configuration = configuration;
        }


        public ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (configuration.GetValue<bool>("Exception:LoggingDetails"))
            {
                logger.LogError(exception, "An exception occurred.\n{Message}\n\n{StackTrace}", exception.Message, exception.StackTrace);
            }
            else
            {
                logger.LogError(exception, "An exception occurred: {Message}", exception.Message);
            }
            return new ValueTask<bool>(false);
        }
    }
}
