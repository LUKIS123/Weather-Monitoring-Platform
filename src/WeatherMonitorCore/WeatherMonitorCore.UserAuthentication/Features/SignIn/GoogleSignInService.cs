using Google.Apis.Auth;
using WeatherMonitorCore.Contract.Auth;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Jwt;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Models;

namespace WeatherMonitorCore.UserAuthentication.Features.SignIn;

internal interface IExternalSignInService
{
    Task<JwtTokenResponse> Handle(AuthenticateRequest authenticateRequest);
}

internal class GoogleSignInService : IExternalSignInService
{
    private readonly IJwtAuthorizationService _jwtAuthorizationService;

    public GoogleSignInService(IJwtAuthorizationService jwtAuthorizationService)
    {
        _jwtAuthorizationService = jwtAuthorizationService;
    }

    public async Task<JwtTokenResponse> Handle(AuthenticateRequest authenticateRequest)
    {
        try
        {
            if (string.IsNullOrEmpty(authenticateRequest.IdToken))
            {
                return new JwtTokenResponse();
            }

            var validatedToken = await GoogleJsonWebSignature.ValidateAsync(authenticateRequest.IdToken);

            var jwtToken = _jwtAuthorizationService.GenerateJwtToken(
                new UserInfo
                {
                    UserId = validatedToken.Subject,
                    PhotoUrl = validatedToken.Picture,
                    UserName = validatedToken.Name,
                    Email = validatedToken.Email
                });
            return new JwtTokenResponse
            {
                Token = jwtToken
            };
        }
        catch
        {
            return new JwtTokenResponse();
        }
    }
}