﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Features.GetDeviceMqttCredentials;
using WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;
using WeatherMonitorCore.DeviceManagement.Features.RemoveDevice;
using WeatherMonitorCore.DeviceManagement.Features.UpdateDeviceStatus;
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

        routes.MapGet(
            "/api/deviceManagement/credentials",
            async (HttpContext context, [FromServices] IGetMqttCredentialsService mqttCredentialsService,
                [FromQuery] int deviceId) =>
            {
                var result = await mqttCredentialsService.Handle(deviceId);
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

        routes.MapPost(
            "/api/deviceManagement/statusUpdate",
            async (HttpContext context, [FromServices] IUpdateDeviceStatusService updateDeviceStatusService,
                [FromBody] UpdateDeviceStatusRequest requests) =>
            {
                var result = await updateDeviceStatusService.Handle(requests);
                await context.HandleResult(result);
            }).AllowAnonymous();
    }
}
