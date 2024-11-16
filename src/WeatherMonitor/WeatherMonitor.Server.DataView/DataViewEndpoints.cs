using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.DataView.Features.GetStationsList;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastMonth;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastWeek;
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
        //..RequireAuthorization(policy => policy.RequireRole("PowerUser", "ControlPanelUser"));

        routes.MapGet(
            "/api/dataView/stations/map",
            async (HttpContext context, [FromServices] IGetStationsListService getDevicesService) =>
            {
                var result = await getDevicesService.Handle(1, 1000);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapGet(
            "/api/dataView/history/day",
            async (HttpContext context, [FromServices] IGetWeatherDataLastDayService getWeatherDataLastDayService,
                [FromQuery] int? deviceId = null) =>
            {
                var result = await getWeatherDataLastDayService.Handle(deviceId);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapGet(
            "/api/dataView/history/week",
            async (HttpContext context, [FromServices] IGetWeatherDataLastWeekService getWeatherDataLastWeekService,
                [FromQuery] int? deviceId = null) =>
            {
                var result = await getWeatherDataLastWeekService.Handle(deviceId);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapGet(
            "/api/dataView/history/month",
            async (HttpContext context, [FromServices] IGetWeatherDataLastMonthService getWeatherDataLastMonthService,
                [FromQuery] int? deviceId = null) =>
            {
                var result = await getWeatherDataLastMonthService.Handle(deviceId);
                await context.HandleResult(result);
            }).AllowAnonymous();
    }
}
