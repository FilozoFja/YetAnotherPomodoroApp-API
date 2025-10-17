using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Json;
using LoginRequest = YAPA.Models.Auth.LoginRequest;
using YAPA.Models.Auth;
using YAPA.Models.Response;

namespace YAPA.Tests;

public class AuthTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public AuthTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task UserLogin_WithValidCredentials_ReturnsOk()
    {
        var loginRequest = new LoginRequest("admin@admin.com", "Admin123!");
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var content = await response.Content.ReadAsStringAsync();
        Assert.NotEmpty(content);
    }

    [Fact]
    public async Task UserLogin_WithInvalidEmail_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest("wrong@email.com", "Admin123!");
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserLogin_WithInvalidPassword_ReturnsUnauthorized()
    {
        var loginRequest = new LoginRequest("admin@admin.com", "WrongPassword123!");
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserLogin_WithEmptyEmail_ReturnsBadRequest()
    {
        var loginRequest = new LoginRequest("", "Admin123!");
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserLogin_WithEmptyPassword_ReturnsBadRequest()
    {
        var loginRequest = new LoginRequest("admin@admin.com", "");
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task UserLogin_WithInvalidEmailFormat_ReturnsBadRequest()
    {
        var loginRequest = new LoginRequest("not-an-email", "Admin123!");
        
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task RefreshToken_WithValidToken_ReturnsOk()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@admin.com", "Admin123!"));
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ResponseModel<LoginResponse>>();
        
        var refreshRequest = new TokenRefreshRequest(loginResult.Data.RefreshToken, "admin@admin.com");
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
        
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RefreshToken_WithInvalidToken_ReturnsUnauthorized()
    {
        var refreshRequest = new TokenRefreshRequest("invalid-token", "admin@admin.com");
        
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
        
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }
    
    [Fact]
    public async Task RefreshToken_WithInvalidEmail_ReturnsUnauthorized()
    {
        var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
            new LoginRequest("admin@admin.com", "Admin123!"));
        var loginResult = await loginResponse.Content.ReadFromJsonAsync<ResponseModel<LoginResponse>>();
        
        var refreshRequest = new TokenRefreshRequest(loginResult.Data.RefreshToken, "wrong@admin.com");
        var response = await _client.PostAsJsonAsync("/api/auth/refresh-token", refreshRequest);
        
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}