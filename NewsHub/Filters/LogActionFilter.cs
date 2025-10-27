using Microsoft.AspNetCore.Mvc.Filters;

namespace NewsHub.Filters
{
    public class LogActionFilter : IActionFilter
    {
        private readonly ILogger<LogActionFilter> _logger;
        public LogActionFilter(ILogger<LogActionFilter> logger)
        {
            _logger = logger;
        }
        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Log the action execution started
            var actionName = context.ActionDescriptor.DisplayName;
            _logger.LogInformation($"Action '{actionName}' execution started.");
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Log the action execution result
            var actionName = context.ActionDescriptor.DisplayName;
            var result = context.Result?.ToString() ?? "No Result";
            _logger.LogInformation($"Action '{actionName}' execution finished with result: {result}");
        }
    }
}