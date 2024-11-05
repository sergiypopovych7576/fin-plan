using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FP.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            var errorResponse = new
            {
                Message = "An unexpected error occurred. Please try again later.",
            };
            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = 500
            };
            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }
    }
}
