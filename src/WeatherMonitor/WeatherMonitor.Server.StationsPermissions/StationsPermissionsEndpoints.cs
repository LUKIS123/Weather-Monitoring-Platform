using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;
using WeatherMonitor.Server.StationsPermissions.Features.ListAvailableStations;

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
    }
}
