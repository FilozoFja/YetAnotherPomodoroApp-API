using YAPA.Models.Pomodoro;

namespace YAPA.Extensions
{
    public static class EndpointExtensions
    {
        public static IEndpointRouteBuilder PomodoroEndpoints( this IEndpointRouteBuilder app )
        {
            app.MapPost("/add-pomodoro", (AddPomodoroRequest request) =>
            {

            });
            return app;
        }
        public static IEndpointRouteBuilder MapWeatherEndpoints(this IEndpointRouteBuilder app)
        {
            var summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            app.MapGet("/weatherforecast", () =>
                {
                    var forecast = Enumerable.Range(1, 5).Select(index =>
                            new WeatherForecast
                            (
                                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                                Random.Shared.Next(-20, 55),
                                summaries[Random.Shared.Next(summaries.Length)]
                            ))
                        .ToArray();
                    return forecast;
                })
                .WithName("GetWeatherForecast")
                .WithOpenApi();

            return app;
        }
    }

    internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
    {
        public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
    }
}