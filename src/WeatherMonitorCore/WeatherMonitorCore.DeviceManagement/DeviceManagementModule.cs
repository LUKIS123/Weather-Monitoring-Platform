using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;

namespace WeatherMonitorCore.DeviceManagement;

public static class DeviceManagementModule
{
    public static IServiceCollection AddDeviceManagementModule(this IServiceCollection services)
    {
        services.AddTransient<IRegisterDeviceService, RegisterDeviceService>();


        return services;
    }
}
