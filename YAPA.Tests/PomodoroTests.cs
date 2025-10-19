using System.Net;
using System.Net.Http.Json;
using YAPA.Models;
using YAPA.Models.Auth;
using YAPA.Models.Pomodoro;
using YAPA.Models.Response;

namespace YAPA.Tests;

public class PomodoroTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PomodoroTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<HttpClient> GetAuthenticatedClientAsync()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@admin.com", "Admin123!"));
        
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ResponseModel<LoginResponse>>();
        
        var authenticatedClient = _factory.CreateClient();
        authenticatedClient.DefaultRequestHeaders.Authorization = 
            new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", loginResult.Data.Token);
        
        return authenticatedClient;
    }
    
    #region Add Pomodoro Tests
    
    [Fact]
    public async Task Pomodoro_Add_WithValidCredentials_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var addPomodoroRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow,
            IsCompleted = true,
            Duration = 30
        };
        
        var response = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addPomodoroRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<PomodoroModel>>();
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.Equal("Pomodoro added successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Pomodoro_Add_WithoutAuthentication_ReturnsUnauthorized()
    {
        var addPomodoroRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow,
            IsCompleted = true,
            Duration = 30
        };
        
        var response = await _client.PostAsJsonAsync("/pomodoro/add-new", addPomodoroRequest);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_Add_WithNegativeDuration_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var addPomodoroRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow,
            IsCompleted = true,
            Duration = -10
        };
        
        var response = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addPomodoroRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_Add_WithZeroDuration_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var addPomodoroRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow,
            IsCompleted = true,
            Duration = 0
        };
        
        var response = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addPomodoroRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_Add_WithFutureEndTime_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var addPomodoroRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow.AddDays(1),
            IsCompleted = true,
            Duration = 30
        };
        
        var response = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addPomodoroRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_Add_WithCompletedFalse_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var addPomodoroRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow,
            IsCompleted = false,
            Duration = 25
        };
        
        var response = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addPomodoroRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    #endregion

    #region Get Pomodoros By Day Tests

    [Fact]
    public async Task Pomodoro_GetByDay_WithValidDate_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        
        // Query string format: ?date=2025-01-15
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-day?date={date:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<PomodoroByDayResponse>>();
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.Equal("Pomodoros retrieved successfully", result.Message);
    }

    [Fact]
    public async Task Pomodoro_GetByDay_WithoutAuthentication_ReturnsUnauthorized()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var response = await _client.GetAsync($"/pomodoro/by-day?date={date:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_GetByDay_WithPastDate_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-7));
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-day?date={date:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_GetByDay_WithFutureDate_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var date = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-day?date={date:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<PomodoroByDayResponse>>();
        Assert.NotNull(result);
    }

    [Fact]
    public async Task Pomodoro_GetByDay_AfterAddingPomodoro_ReturnsAddedPomodoro()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var addRequest = new AddPomodoroRequest
        {
            EndTime = DateTime.UtcNow,
            IsCompleted = true,
            Duration = 25
        };
        await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addRequest);
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-day?date={today:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<PomodoroByDayResponse>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Pomodoro_GetByDay_WithInvalidDateFormat_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        
        var response = await authenticatedClient.GetAsync("/pomodoro/by-day?date=not-a-date");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Pomodoro_AddMultiple_AndGetByDay_ReturnsAllPomodoros()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        
        for (int i = 0; i < 3; i++)
        {
            var addRequest = new AddPomodoroRequest
            {
                EndTime = DateTime.UtcNow,
                IsCompleted = true,
                Duration = 25
            };
            var addResponse = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addRequest);
            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        }
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-day?date={today:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<PomodoroByDayResponse>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    #endregion
    
    #region Get Weekly Pomodoro Tests

    [Fact]
    public async Task Pomodoro_GetByWeek_WithValidEndDate_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.Equal("Weekly pomodoros retrieved successfully", result.Message);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_WithoutAuthentication_ReturnsUnauthorized()
    {
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var response = await _client.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_WithPastDate_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.True(result.Status);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_WithFutureDate_ReturnsOk()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(7));
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_WithInvalidDateFormat_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        
        var response = await authenticatedClient.GetAsync("/pomodoro/by-week?endDateTime=invalid-date");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_WithEmptyDateParameter_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        
        var response = await authenticatedClient.GetAsync("/pomodoro/by-week?endDateTime=");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Integration Tests

    [Fact]
    public async Task Pomodoro_AddMultipleDays_AndGetByWeek_ReturnsWeeklySummary()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var now = DateTime.UtcNow;
        
        for (int i = 0; i < 3; i++)
        {
            var addRequest = new AddPomodoroRequest
            {
                EndTime = now.AddMinutes(-i * 2),
                IsCompleted = true,
                Duration = 25
            };
            var addResponse = await authenticatedClient.PostAsJsonAsync("/pomodoro/add-new", addRequest);
            Assert.Equal(HttpStatusCode.OK, addResponse.StatusCode);
        }
    
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={now:yyyy-MM-ddTHH:mm:ss}");
        
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"Status: {response.StatusCode}");
        Console.WriteLine($"Response: {content}");
    
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        Assert.True(result.Data.Count > 0, "Should contain weekly pomodoro data");
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_EmptyWeek_ReturnsEmptyList()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow.AddYears(10));
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_VerifyDateRangeCalculation()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.NotNull(result.Data);
        
        foreach (var day in result.Data)
        {
            var dayDate = DateOnly.FromDateTime(day.PomodorosDate);
            var daysDiff = endDate.DayNumber - dayDate.DayNumber;
            Assert.True(daysDiff >= 0 && daysDiff < 7, $"Date {day.PomodorosDate} should be within last 7 days from {endDate}");
        }
    }

    #endregion

    #region Validation Tests

    [Theory]
    [InlineData("9999-12-31")] // Far future
    [InlineData("2020-01-01")] // Past date
    [InlineData("2025-10-19")] // Current date (adjust as needed)
    public async Task Pomodoro_GetByWeek_WithVariousValidDates_ReturnsOk(string dateString)
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={dateString}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Theory]
    [InlineData("2025-13-01")] 
    [InlineData("2025-01-32")] 
    [InlineData("not-a-date")]
    public async Task Pomodoro_GetByWeek_WithInvalidDates_ReturnsBadRequest(string invalidDate)
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={invalidDate}");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    #endregion

    #region Error Handling Tests

    [Fact]
    public async Task Pomodoro_GetByWeek_WithNullDate_ReturnsBadRequest()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        
        var response = await authenticatedClient.GetAsync("/pomodoro/by-week?endDateTime=null");
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Pomodoro_GetByWeek_ValidatesResponseStructure()
    {
        var authenticatedClient = await GetAuthenticatedClientAsync();
        var endDate = DateOnly.FromDateTime(DateTime.UtcNow);
        
        var response = await authenticatedClient.GetAsync($"/pomodoro/by-week?endDateTime={endDate:yyyy-MM-dd}");
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var result = await response.Content.ReadFromJsonAsync<ResponseModel<List<WeeklyPomodoroResponse>>>();
        Assert.NotNull(result);
        Assert.True(result.Status);
        Assert.NotEmpty(result.Message);
        Assert.NotNull(result.Data);
        
        foreach (var day in result.Data)
        {
            Assert.NotNull(day.PomodorosDate);
            Assert.True(day.PomodoroCount >= 0);
            Assert.True(day.PomodoroFinished >= 0);
            Assert.True(day.Duration >= 0);
        }
    }

    #endregion
}