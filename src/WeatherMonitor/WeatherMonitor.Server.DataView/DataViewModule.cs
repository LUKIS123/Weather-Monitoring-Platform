using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DataView.Features.GetStationsList;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastMonth;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastWeek;

namespace WeatherMonitor.Server.DataView;

public static class DataViewModule
{
    public static IServiceCollection AddDataViewModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGetStationsListService, GetStationsListService>();
        serviceCollection.AddTransient<IGetWeatherDataLastDayService, GetWeatherDataLastDayService>();
        serviceCollection.AddTransient<IGetWeatherDataLastWeekService, GetWeatherDataLastWeekService>();
        serviceCollection.AddTransient<IGetWeatherDataLastMonthService, GetWeatherDataLastMonthService>();

        return serviceCollection;
    }
}
