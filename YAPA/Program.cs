using YAPA.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Services
builder.Services.AddDatabase(builder.Configuration);
builder.Services.AddSwagger();
builder.Services.AddIdentityService();
builder.Services.AddJwtAuthentication(builder.Configuration, builder.Environment.IsDevelopment());
builder.Services.AddCustomAuthorization();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// Endpoints
app.MapWeatherEndpoints();

app.Run();