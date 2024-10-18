﻿using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Utils;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Validators;

namespace WeatherMonitorCore.DeviceManagement;

public static class DeviceManagementModule
{
    public static IServiceCollection AddDeviceManagementModule(this IServiceCollection services)
    {
        services.AddTransient<IRegisterDeviceService, RegisterDeviceService>();

        services.AddTransient<IPasswordGeneratorService, PasswordGeneratorService>();
        services.AddTransient<IDeviceCredentialsGenerator, DeviceCredentialsGenerator>();

        services.AddScoped<IValidator<RegisterDeviceRequest>, RegisterDeviceRequestValidator>();

        return services;
    }
}
