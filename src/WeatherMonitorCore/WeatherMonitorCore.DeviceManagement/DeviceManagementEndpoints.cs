using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace WeatherMonitorCore.DeviceManagement;
public static class DeviceManagementEndpoints
{
    public static void RegisterDeviceManagementEndpoints(this IEndpointRouteBuilder routes)
    {
        // routes.MapPost(
        //     "/api/user/googleAuthenticate",
        //     async (HttpContext context, IExternalSignInService externalSignInService,
        //         [FromBody] AuthenticateRequest authenticateRequest) =>
        //     {
        //         var tokenResponse = await externalSignInService.Handle(authenticateRequest);
        //
        //         if (!string.IsNullOrEmpty(tokenResponse.Token))
        //         {
        //             await context.Response.WriteAsJsonAsync(new AuthenticationResponse
        //             {
        //                 Token = tokenResponse.Token
        //             });
        //         }
        //         else
        //         {
        //             context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        //         }
        //     }).AllowAnonymous();
    }
}
