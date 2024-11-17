using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitorCore.Contract.UserAuthenticationModule;
using WeatherMonitorCore.UserAuthentication.Features.SignIn;
using WeatherMonitorCore.UserAuthentication.Features.UpdateRole;

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
                    await context.Response.WriteAsJsonAsync(new AuthenticationResponse
                    {
                        Token = tokenResponse.Token
                    });
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
            }).AllowAnonymous();

        routes.MapPost(
            "/api/user/setRole",
            async (HttpContext context, IUpdateRoleService updateRoleService,
                [FromBody] UpdateRoleRequest request) =>
            {
                var success = await updateRoleService.UpdateRole(request.UserId, request.Role);

                context.Response.StatusCode = success
                    ? StatusCodes.Status200OK
                    : StatusCodes.Status400BadRequest;
            }).RequireAuthorization("IsAdminPolicy");
    }
}
