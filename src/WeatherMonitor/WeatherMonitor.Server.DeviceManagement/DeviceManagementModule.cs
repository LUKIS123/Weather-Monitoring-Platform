using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DeviceManagement.Features.GetDevices;
using WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;

namespace WeatherMonitor.Server.DeviceManagement;

public static class DeviceManagementModule
{
    public static IServiceCollection AddDeviceManagementModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGetDevicesService, GetDevicesService>();
        serviceCollection.AddTransient<IRegisterDeviceService, RegisterDeviceService>();

        return serviceCollection;
    }
}
