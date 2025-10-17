using Microsoft.AspNetCore.Diagnostics;
using YAPA.Models.Response;

namespace YAPA.Extensions
{
    public static class ExceptionHandlerExtension
    {
        public static IApplicationBuilder UseExceptionHandlerExtension(this IApplicationBuilder app)
        {
            app.UseExceptionHandler(errorApp =>
            {
                errorApp.Run(async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();
                    var exception = exceptionFeature?.Error;

                    var response = new ResponseModel<object>
                    {
                        Status = false,
                        Data = null
                    };
                    
                    switch (exception)
                    {
                        case UnauthorizedAccessException:
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            response.Message = exception.Message;
                            response.Errors = new List<string> { exception.Message };
                            break;

                        case KeyNotFoundException:
                            context.Response.StatusCode = StatusCodes.Status404NotFound;
                            response.Message = exception.Message;
                            response.Errors = new List<string> { exception.Message };
                            break;

                        case ArgumentException:
                        case InvalidOperationException:
                            context.Response.StatusCode = StatusCodes.Status400BadRequest;
                            response.Message = exception?.Message ?? "Bad request.";
                            response.Errors = new List<string> { exception?.Message ?? "Invalid operation." };
                            break;

                        default:
                            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                            response.Message = "An error occurred while processing your request.";
                            response.Errors = new List<string> { exception?.Message ?? "No additional error information available." };
                            break;
                    }

                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsJsonAsync(response);
                });
            });

            return app;
        }
    }
}