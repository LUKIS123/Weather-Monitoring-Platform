using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.DeviceManagement.Features.GetCredentials;
using WeatherMonitor.Server.DeviceManagement.Features.GetDevices;
using WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;
using WeatherMonitor.Server.DeviceManagement.Features.RemoveDevice;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;
using WeatherMonitorCore.Contract.DeviceManagementModule;

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

        routes.MapPost(
            "/api/deviceManagement/register",
            async (HttpContext context, [FromServices] IRegisterDeviceService registerDeviceService,
                [FromBody] RegisterDeviceRequest deviceRequest) =>
            {
                var result = await registerDeviceService.Handle(deviceRequest);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapGet(
            "/api/deviceManagement/credentials",
            async (HttpContext context, [FromServices] IGetMqttCredentialsService credentialsService,
                [FromQuery] int deviceId) =>
            {
                var result = await credentialsService.Handle(deviceId);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapDelete(
            "/api/deviceManagement/remove",
            async (HttpContext context, [FromServices] IRemoveDeviceService removeDeviceService,
                [FromQuery] int deviceId) =>
            {
                var result = await removeDeviceService.Handle(deviceId);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");
    }
}
