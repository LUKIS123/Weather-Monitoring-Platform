using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DataView.Features.GetStationsList;

namespace WeatherMonitor.Server.DataView;

public static class DataViewModule
{
    public static IServiceCollection AddDataViewModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGetStationsListService, GetStationsListService>();

        return serviceCollection;
    }
}
