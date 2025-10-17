using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Json;

namespace YAPA.Tests
{
    public class AuthTests
    {
        [Fact]
        public async Task UserLogin()
        {
            await using var application = new WebApplicationFactory<Program>();

            var client = application.CreateClient();

            var result = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest
            {
                Email = "admin@admin.com",
                Password = "Admin123!"
            });
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
        }
    }
}
