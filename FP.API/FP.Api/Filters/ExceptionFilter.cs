using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace FP.Api.Filters
{
    public class ExceptionFilter : IAsyncExceptionFilter
    {
        public async Task OnExceptionAsync(ExceptionContext context)
        {
            object errorResponse = null;
            var statusCode = 500;
            if (context.Exception is ValidationException)
            {
                var exception = (ValidationException)context.Exception;
                errorResponse = new
                {
                    Errors = exception.Errors.Select(c => c.ErrorMessage),
                };
                statusCode = 400;
            }
            if (errorResponse == null)
            {
                errorResponse = new
                {
                    Message = "An unexpected error occurred. Please try again later.",
                };
            }
            context.Result = new JsonResult(errorResponse)
            {
                StatusCode = statusCode
            };
            context.ExceptionHandled = true;
            await Task.CompletedTask;
        }
    }
}
