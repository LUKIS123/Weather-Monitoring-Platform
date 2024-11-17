using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;
using WeatherMonitor.Server.StationsPermissions.Features.GetUsersPermissions;
using WeatherMonitor.Server.StationsPermissions.Features.ListAvailableStations;
using WeatherMonitor.Server.StationsPermissions.Features.SendPermissionRequest;
using WeatherMonitor.Server.StationsPermissions.Features.StationPermissionStatus;

namespace WeatherMonitor.Server.StationsPermissions;

public static class StationsPermissionsEndpoints
{
    public static void RegisterStationsPermissionsEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet(
            "/api/permissions/availableStations",
            async (HttpContext context, [FromServices] IListAvailableStationsService stationsPermissionsService,
                [FromQuery] int pageNumber) =>
            {
                var result = await stationsPermissionsService.Handle(pageNumber);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapGet(
            "/api/permissions/stationPermissionStatus",
            async (HttpContext context, [FromServices] IGetStationPermissionStatusService getStationPermissionStatusService,
                [FromQuery] int stationId) =>
            {
                var result = await getStationPermissionStatusService.Handle(stationId);
                await context.HandleResult(result);
            }).RequireAuthorization();

        routes.MapPost(
            "/api/permissions/sendRequest",
            async (HttpContext context, [FromServices] ISendPermissionRequestService sendPermissionRequestService,
                [FromQuery] int stationId) =>
            {
                var result = await sendPermissionRequestService.Handle(stationId);
                await context.HandleResult(result);
            }).RequireAuthorization();

        routes.MapGet(
            "/api/permissions/permissionRequests",
            async (HttpContext context, [FromServices] IGetUsersPermissionsService getUsersPermissionsService,
                [FromQuery] int pageNumber) =>
            {
                var result = await getUsersPermissionsService.Handle(pageNumber);
                await context.HandleResult(result);
            }).RequireAuthorization();
    }
}
