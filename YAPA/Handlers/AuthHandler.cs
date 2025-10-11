using YAPA.Interface;
using YAPA.Models.Auth;
using YAPA.Service;

namespace YAPA.Handlers
{
    public class AuthHandler
    {
        private readonly IAuthService _service;
        public AuthHandler(IAuthService service)
        {
            _service = service;
        }
        public async Task<IResult> HandleLoginAsync(LoginRequest request)
        {
            //TODO ZROBIC TO JAKO HANDLER DLA WIEKSZEJ WYGODY I POZNIEJSZEJ TESTOWALNOSCI
            //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
            //TODO SPRAWDZANIE JAKOS TEGO EMAIL BO TO MOZE BYC SLABE KIEDY DAPPER WEJDZIE
            if (string.IsNullOrWhiteSpace(request.Email))
                return Results.BadRequest(new { error = "Email is required" });
            //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
            if (string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest(new { error = "Password is required" });

            var response = await _service.LoginAsync(request);

            return response.Status
                ? Results.Ok(response)
                : Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Unauthorized",
                    detail: response.Message
                );
        }
    
        public async Task<IResult> HandlerTokenRefresh(TokenRefreshRequest request)
        {

            //TODO ZROBIC TO JAKO HANDLER DLA WIEKSZEJ WYGODY I POZNIEJSZEJ TESTOWALNOSCI
            //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
            //TODO SPRAWDZANIE JAKOS TEGO EMAIL BO TO MOZE BYC SLABE KIEDY DAPPER WEJDZIE
            if (string.IsNullOrWhiteSpace(request.Email))
                return Results.BadRequest(new { error = "Email is required" });
            //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return Results.BadRequest(new { error = "RefreshToken is required" });
            var response = await _service.RefreshTokenAsync(request.RefreshToken, request.Email);

            return response.Status
                ? Results.Ok(response)
                : Results.Problem(
                    statusCode: StatusCodes.Status401Unauthorized,
                    title: "Unauthorized",
                    detail: response.Message
                );
        }
    }
}
