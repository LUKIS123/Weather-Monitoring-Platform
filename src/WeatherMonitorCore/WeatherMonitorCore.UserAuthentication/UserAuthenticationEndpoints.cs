using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitorCore.Contract.UserAuthenticationModule;
using WeatherMonitorCore.UserAuthentication.Features.SignIn;

namespace WeatherMonitorCore.UserAuthentication;

public static class UserAuthenticationEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/api/user/googleAuthenticate",
            async (HttpContext context, IExternalSignInService externalSignInService,
                [FromBody] AuthenticateRequest authenticateRequest) =>
            {
                var tokenResponse = await externalSignInService.Handle(authenticateRequest);

                if (!string.IsNullOrEmpty(tokenResponse.Token))
                {
                    await context.Response.WriteAsync(tokenResponse.Token);
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }).AllowAnonymous();
    }
}
