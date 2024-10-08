using Microsoft.AspNetCore.Http;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;
using WeatherMonitorCore.Contract.Auth;

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

        var (jwtToken, success, message) = await _httpClientWrapper.PostHttpRequest<AuthenticateRequest, string>(
            "api/user/googleAuthenticate",
            new AuthenticateRequest { IdToken = idToken });

        if (!success || jwtToken is null)
        {
            return new MicroserviceApiException(message);
        }

        return new JwtTokenResponse
        {
            Token = jwtToken
        };
    }
}
