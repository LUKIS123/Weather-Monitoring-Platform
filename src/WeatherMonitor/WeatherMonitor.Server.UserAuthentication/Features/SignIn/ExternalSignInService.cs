using Microsoft.AspNetCore.Http;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;
using WeatherMonitorCore.Contract.UserAuthenticationModule;

namespace WeatherMonitor.Server.UserAuthentication.Features.SignIn;
internal interface IExternalSignInService
{
    Task<Result<JwtTokenResponse>> Handle(string token);
}

internal class ExternalSignInService : IExternalSignInService
{
    private readonly ICoreMicroserviceHttpClientWrapper _httpClientWrapper;

    public ExternalSignInService(ICoreMicroserviceHttpClientWrapper httpClientWrapper)
    {
        _httpClientWrapper = httpClientWrapper;
    }

    public async Task<Result<JwtTokenResponse>> Handle(string idToken)
    {
        if (string.IsNullOrEmpty(idToken))
        {
            return new BadHttpRequestException("Did not receive token");
        }

        var (response, success, message) = await _httpClientWrapper.PostHttpRequest<AuthenticateRequest, AuthenticationResponse>(
            "api/user/googleAuthenticate",
            new AuthenticateRequest { IdToken = idToken });

        if (!success || string.IsNullOrEmpty(response?.Token))
        {
            return new MicroserviceApiException(message);
        }

        return new JwtTokenResponse
        {
            Token = response.Token
        };
    }
}
