using YAPA.Interface;
using YAPA.Models.Auth;
using YAPA.Models.Response;

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
            //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
            //TODO SPRAWDZANIE JAKOS TEGO EMAIL BO TO MOZE BYC SLABE KIEDY DAPPER WEJDZIE
            if (string.IsNullOrWhiteSpace(request.Email))
                return Results.BadRequest(new { error = "Email is required" });
            //TODO MOZE JAKAS WALIDACJA LITER WCHODZACYCH 
            if (string.IsNullOrWhiteSpace(request.Password))
                return Results.BadRequest(new { error = "Password is required" });

            var result = await _service.LoginAsync(request);
            var response = new ResponseModel<LoginResponse>();
            if (result != null)
            {
                response.Data = result;
                response.Message = "Login successful";
                response.Status = true;
                return Results.Ok(response);
            }
            else
            {
                response.Message = "Invalid email or password.";
                response.Status = false;
                response.Data = null;
                return Results.ValidationProblem(response);
            }
        }
    
        public async Task<IResult> HandlerTokenRefresh(TokenRefreshRequest request)
        {
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
