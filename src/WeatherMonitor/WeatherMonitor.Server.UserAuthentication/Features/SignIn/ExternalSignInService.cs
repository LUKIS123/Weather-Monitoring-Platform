using Google.Apis.Auth;
using WeatherMonitor.Server.UserAuthentication.Infrastructure.Jwt;
using WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;

namespace WeatherMonitor.Server.UserAuthentication.Features.SignIn;
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

            // todo:
            // jesli valid to uderzamy do Core zapisujemy usera.
            // ewentualnie walidacja dopiero w Core Microservice <- moze lepsze rozwiazanie
            // napisac http clienta do Core i tam walidowac token, zwracac UserInfo

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
