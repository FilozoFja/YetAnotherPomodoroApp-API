using System.ComponentModel.DataAnnotations;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using YAPA.Interface;
using YAPA.Models.Auth;
using YAPA.Models.Response;
using YAPA.Validators;

namespace YAPA.Handlers
{
    public class AuthHandler
    {
        private readonly IAuthService _service;
        private readonly IValidator<LoginRequest> _loginRequestValidator;
        private readonly IValidator<TokenRefreshRequest> _tokenRefreshValidator;
        public AuthHandler(IAuthService service,
            IValidator<LoginRequest> loginRequestValidator,
            IValidator<TokenRefreshRequest> tokenRefreshValidator)
        {
            _service = service;
            _loginRequestValidator = loginRequestValidator;
            _tokenRefreshValidator = tokenRefreshValidator;
        }
        public async Task<IResult> HandleLoginAsync(LoginRequest request)
        {
            var validationResult =  await _loginRequestValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(new ResponseModel<Object>
                {
                    Message = "There is a validation issue.",
                    Status = false,
                    Errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList()
                });

            var result = await _service.LoginAsync(request);

            return Results.Ok(new ResponseModel<LoginResponse>
            {
                Data = result,
                Message= "Login successful",
                Status = true
            });
        }
    
        public async Task<IResult> HandlerTokenRefresh(TokenRefreshRequest request)
        {
            var validationResult = await _tokenRefreshValidator.ValidateAsync(request);
            if (!validationResult.IsValid)
                return Results.BadRequest(validationResult);
            
            var result = await _service.RefreshTokenAsync(request.RefreshToken, request.Email);
            var response = new ResponseModel<TokenRefreshResponse>();
            
            return Results.Ok(new ResponseModel<TokenRefreshResponse>
            {
                Data = result,
                Message = "Refresh token generated.",
                Status = true
            });
        }
    }
}
