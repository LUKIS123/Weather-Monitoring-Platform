using Microsoft.Extensions.DependencyInjection;
using WeatherMonitor.Server.DeviceManagement.Features.GetCredentials;
using WeatherMonitor.Server.DeviceManagement.Features.GetDevices;
using WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;
using WeatherMonitor.Server.DeviceManagement.Features.RemoveDevice;

namespace WeatherMonitor.Server.DeviceManagement;

public static class DeviceManagementModule
{
    public static IServiceCollection AddDeviceManagementModule(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGetDevicesService, GetDevicesService>();
        serviceCollection.AddTransient<IRegisterDeviceService, RegisterDeviceService>();
        serviceCollection.AddTransient<IGetMqttCredentialsService, GetMqttCredentialsService>();
        serviceCollection.AddTransient<IRemoveDeviceService, RemoveDeviceService>();

        return serviceCollection;
    }
}
