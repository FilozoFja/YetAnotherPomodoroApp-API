using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

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

                    var problemDetails = new ProblemDetails
                    {
                        Title = "An unexpected error occurred.",
                        Status = StatusCodes.Status500InternalServerError,
                        Detail = exception?.Message
                    };

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = problemDetails.Status ?? 500;
                    await context.Response.WriteAsJsonAsync(problemDetails);
                });
            });

            return app;
        }
    }
}
