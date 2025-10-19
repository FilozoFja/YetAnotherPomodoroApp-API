using YAPA.Db;
using YAPA.Extensions;
using YAPA.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddIdentityService();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddCustomAuthorization();
builder.Services.AddApplicationServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddRedis(builder.Configuration);
builder.Services.AddRateLimiter();
builder.Services.AddApplicationHandlers();
builder.Services.AddFluentValidationService();
builder.Services.AddModelsValidators();
builder.Services.AddSignalR();

var app = builder.Build();

// Middleware
app.UseExceptionHandlerExtension();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using (var scope = app.Services.CreateScope())
{
    await Seeder.InitializeAsync(scope.ServiceProvider);
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();


// Endpoints
app.MapAuthEndpoints();
app.PomodoroEndpoints();
app.MapHub<StatusHub>("/statusHub").RequireAuthorization();


app.Run();

public partial class Program { }