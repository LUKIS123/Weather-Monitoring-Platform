using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel;

namespace WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;

internal interface IRegisterDeviceService
{
    Task<Result<CreateDeviceResponse>> Handle(RegisterDeviceRequest request);
}

internal class RegisterDeviceService : IRegisterDeviceService
{
    private readonly IDeviceManagementRepository _deviceManagementRepository;

    public RegisterDeviceService(IDeviceManagementRepository deviceManagementRepository)
    {
        _deviceManagementRepository = deviceManagementRepository;
    }

    public async Task<Result<CreateDeviceResponse>> Handle(RegisterDeviceRequest request)
    {
        var device = await _deviceManagementRepository.RegisterDeviceAsync(new RegisterDeviceDto());
        return new CreateDeviceResponse();
    }
}
