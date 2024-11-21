using Google.Apis.Auth;
using WeatherMonitorCore.Contract.UserAuthenticationModule;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Jwt;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Models;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;

namespace WeatherMonitorCore.UserAuthentication.Features.SignIn;

internal interface IExternalSignInService
{
    Task<JwtTokenResponse> Handle(AuthenticateRequest authenticateRequest);
}

internal class GoogleSignInService : IExternalSignInService
{
    private readonly IJwtAuthorizationService _jwtAuthorizationService;
    private readonly IUserSettingsRepository _userSettingsRepository;

    public GoogleSignInService(IJwtAuthorizationService jwtAuthorizationService, IUserSettingsRepository userSettingsRepository)
    {
        _jwtAuthorizationService = jwtAuthorizationService;
        _userSettingsRepository = userSettingsRepository;
    }

    public async Task<JwtTokenResponse> Handle(AuthenticateRequest authenticateRequest)
    {
        if (string.IsNullOrEmpty(authenticateRequest.IdToken))
        {
            return new JwtTokenResponse();
        }

        try
        {
            var validatedToken = await GoogleJsonWebSignature.ValidateAsync(authenticateRequest.IdToken);
            var userSettings = await _userSettingsRepository.GetOrCreateUser(
                validatedToken.Subject,
                validatedToken.Email,
                validatedToken.Name);

            var jwtToken = _jwtAuthorizationService.GenerateJwtToken(
                new UserInfo
                {
                    UserId = userSettings.UserId,
                    PhotoUrl = validatedToken.Picture,
                    UserName = validatedToken.Name,
                    Email = validatedToken.Email,
                    Role = userSettings.Role
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