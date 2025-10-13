using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using YAPA.Interface;
using YAPA.Models.Auth;
using YAPA.Models.Response;

namespace YAPA.Handlers
{
    public class AuthHandler
    {
        private readonly IAuthService _service;
        private readonly IValidator<LoginRequest> _loginRequestValidator;
        public AuthHandler(IAuthService service)
        {
            _service = service;
        }
        public async Task<IResult> HandleLoginAsync(LoginRequest request)
        {
            var validationResult =  await _loginRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult);

            var result = await _service.LoginAsync(request);
            var response = new ResponseModel<LoginResponse>();

            if (result == null)
                return Results.Unauthorized();

            response.Data = result;
            response.Message = "Login successful";
            response.Status = true;
            return Results.Ok(response);
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
            var result = await _service.RefreshTokenAsync(request.RefreshToken, request.Email);
            var response = new ResponseModel<TokenRefreshResponse>();
            if (result != null)
            {
                response.Data = result;
                response.Message = "RefreshToken successful";
                response.Status = true;
            }
            else
            {
                return Results.Unauthorized();
            }
            return Results.Ok(response);
        }
    }
}
