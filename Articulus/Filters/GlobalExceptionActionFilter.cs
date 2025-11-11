using Microsoft.AspNetCore.Mvc.Filters;

namespace Articulus.Filters
{
    public class GlobalExceptionActionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionActionFilter> _logger;
        public GlobalExceptionActionFilter(ILogger<GlobalExceptionActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnException(ExceptionContext context)
        {
            var exception = context.Exception;
            var actionName = context.ActionDescriptor.DisplayName;

            _logger.LogError(exception, "An unhandled exception occurred while executing action {ActionName}.", actionName);
        }
    }
}
