using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using YAPA.Models.Auth;
using YAPA.Models.Response;

namespace YAPA.Tests
{
    public class UserStatusHubTests: IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;
        private readonly HttpClient _client;
        public UserStatusHubTests(CustomWebApplicationFactory factory,
            HttpClient client)
        {
             _factory = factory;
            _client = client;
        }

        private async Task<string> GetAuthenticatedClientAsync()
        {
            var loginResponse = await _client.PostAsJsonAsync("/api/auth/login",
                new Models.Auth.LoginRequest("admin@admin.com", "Admin123!"));

            var loginResult = loginResponse.Content.ReadFromJsonAsync<ResponseModel<LoginResponse>>();

            return loginResult!.Result.Data.Token;
        }
        
        private HubConnection CreateHubConnection(string token)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_client.BaseAddress}statushub", options =>
                {
                    options.HttpMessageHandlerFactory = _ => _factory.Server.CreateHandler();
                    options.AccessTokenProvider = () => Task.FromResult(token!);
                })
                .Build();
            return hubConnection;
        }

        [Fact]
        public async Task UserStatus_With_Valid_Credentials()
        {
            var token = await GetAuthenticatedClientAsync();
            var hubConnection = CreateHubConnection(token);


        }
    }
}
