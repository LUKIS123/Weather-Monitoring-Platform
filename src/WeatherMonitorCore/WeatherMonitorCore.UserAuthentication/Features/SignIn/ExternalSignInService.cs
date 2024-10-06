using Google.Apis.Auth;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Jwt;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Models;

namespace WeatherMonitorCore.UserAuthentication.Features.SignIn;

internal interface IExternalSignInService
{
    Task<JwtTokenResponse> Handle(string token);
}

internal class ExternalSignInService : IExternalSignInService
{
    private readonly IJwtAuthorizationService _jwtAuthorizationService;

    public ExternalSignInService(IJwtAuthorizationService jwtAuthorizationService)
    {
        _jwtAuthorizationService = jwtAuthorizationService;
    }

    public async Task<JwtTokenResponse> Handle(string idToken)
    {
        try
        {
            if (string.IsNullOrEmpty(idToken))
            {
                return new JwtTokenResponse();
            }

            var validatedToken = await GoogleJsonWebSignature.ValidateAsync(idToken);

            var jwtToken = _jwtAuthorizationService.GenerateJwtToken(
                new UserInfo()
                {
                    UserId = validatedToken.Subject,
                    PhotoUrl = validatedToken.Picture,
                    UserName = validatedToken.Name
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