using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;
using WeatherMonitorCore.SharedKernel;

namespace WeatherMonitorCore.DeviceManagement;
public static class DeviceManagementEndpoints
{
    public static void RegisterDeviceManagementEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/api/deviceManagement/register",
            async (HttpContext context, [FromServices] IRegisterDeviceService deviceService,
                [FromBody] RegisterDeviceRequest registerDeviceRequest) =>
            {
                var result = await deviceService.Handle(registerDeviceRequest);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");
    }
}
