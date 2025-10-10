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
                        Message = "An error occurred while processing your request.",
                        Data = null,
                        Errors = new List<string> { exception?.Message ?? "No additional error information available." }
                    };

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                    await context.Response.WriteAsJsonAsync(response);
                });
            });

            return app;
        }
    }
}
