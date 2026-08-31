using DomainLayer.Exceptions;
using Shared.ErrorModels;

namespace ToDoApp.CustomMiddlewares
{
    public class CustomExceptionHandlerMiddleWare
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<CustomExceptionHandlerMiddleWare> _logger;

        public CustomExceptionHandlerMiddleWare(RequestDelegate next, ILogger<CustomExceptionHandlerMiddleWare> logger)
        {
            _next = next;
            _logger = logger;
        }
        
        public async Task InvokeAsync(HttpContext httpContext)
        {
            try
            {
                await _next.Invoke(httpContext);
                await HandleNotFoundEndPoint(httpContext);
            }
            catch (Exception ex)
            {

                // Log the exception
                _logger.LogError(ex, "An error occurred while processing the request.");

                await HandleExceptions(httpContext, ex);
            }
        }

        private static async Task HandleExceptions(HttpContext httpContext, Exception ex)
        {
            // Status coode 500 for internal server error
            httpContext.Response.StatusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            //Response message
            var response = new ErrorToReturn()
            {
                StatusCode = httpContext.Response.StatusCode,
                ErrorMessage = ex.Message
            };

            // Set the response content type to JSON
            await httpContext.Response.WriteAsJsonAsync(response);
        }

        private static async Task HandleNotFoundEndPoint(HttpContext httpContext)
        {
            if (httpContext.Response.StatusCode == StatusCodes.Status404NotFound)
            {
                var response = new ErrorToReturn()
                {
                    StatusCode = StatusCodes.Status404NotFound,
                    ErrorMessage = $"The requested resource '{httpContext.Request.Path}' was not found."
                };
                await httpContext.Response.WriteAsJsonAsync(response);
            }
        }

    }
}
