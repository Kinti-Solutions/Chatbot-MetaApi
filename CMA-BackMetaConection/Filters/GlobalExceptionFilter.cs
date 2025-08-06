using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Chatbot.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        private readonly ILogger<GlobalExceptionFilter> _logger;

        public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            _logger.LogError(context.Exception, "An unhandled exception occurred during the request.");

            var response = new
            {
                Message = "An unexpected error occurred. Please try again later.",
                Error = context.Exception.Message 
            };

            context.Result = new JsonResult(response)
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true; 
        }
    }
}
