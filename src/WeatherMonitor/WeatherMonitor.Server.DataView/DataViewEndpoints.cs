using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.DataView.Features.GetStationsList;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;

namespace WeatherMonitor.Server.DataView;

public static class DataViewEndpoints
{
    private const int DefaultPageSize = 10;

    public static void RegisterDataViewEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet(
            "/api/dataView/stations/list",
            async (HttpContext context, [FromServices] IGetStationsListService stationsListService,
                [FromQuery] int pageNumber) =>
            {
                var result = await stationsListService.Handle(pageNumber, DefaultPageSize);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapGet(
            "/api/dataView/stations/map",
            async (HttpContext context, [FromServices] IGetStationsListService getDevicesService) =>
            {
                var result = await getDevicesService.Handle(1, 1000);
                await context.HandleResult(result);
            }).AllowAnonymous();
    }
}
