using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DeviceManagement.Features.GetDevices;

namespace WeatherMonitor.Server.DeviceManagement;

public static class DeviceManagementModule
{
    public static IServiceCollection AddDeviceManagementModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGetDevicesService, GetDevicesService>();

        return serviceCollection;
    }
}
