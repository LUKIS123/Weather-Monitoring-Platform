using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.DeviceManagement.Features.GetDevices;
using WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;

namespace WeatherMonitor.Server.DeviceManagement;
public static class DeviceManagementEndpoints
{
    public static void RegisterDeviceManagementEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet(
            "/api/deviceManagement/list",
            async (HttpContext context, [FromServices] IGetDevicesService getDevicesService,
                [FromQuery] int pageNumber) =>
            {
                var result = await getDevicesService.Handle(pageNumber);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapGet(
            "/api/deviceManagement/register",
            async (HttpContext context, [FromServices] IRegisterDeviceService registerDeviceService,
                [FromBody] RegisterDeviceResponse registerDeviceResponse) =>
            {

            }).RequireAuthorization("IsAdminPolicy");
    }
}
