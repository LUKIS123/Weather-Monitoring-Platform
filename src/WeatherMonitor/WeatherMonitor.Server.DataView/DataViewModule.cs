using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DataView.Features.GetStationsList;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;

namespace WeatherMonitor.Server.DataView;

public static class DataViewModule
{
    public static IServiceCollection AddDataViewModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGetStationsListService, GetStationsListService>();
        serviceCollection.AddTransient<IGetWeatherDataLastDayService, GetWeatherDataLastDayService>();

        return serviceCollection;
    }
}
