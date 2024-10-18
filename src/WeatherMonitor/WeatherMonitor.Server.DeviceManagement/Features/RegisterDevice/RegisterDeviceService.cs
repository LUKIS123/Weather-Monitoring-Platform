using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitorCore.Contract.DeviceManagementModule;

namespace WeatherMonitor.Server.DeviceManagement.Features.RegisterDevice;

interface IRegisterDeviceService
{
    Task<Result<RegisterDeviceResponse>> Handle(RegisterDeviceRequest request);
}

internal class RegisterDeviceService : IRegisterDeviceService
{
    private readonly ICoreMicroserviceHttpClientWrapper _httpClientWrapper;
    private readonly IUserAccessor _userAccessor;

    public RegisterDeviceService(ICoreMicroserviceHttpClientWrapper httpClientWrapper, IUserAccessor userAccessor)
    {
        _httpClientWrapper = httpClientWrapper;
        _userAccessor = userAccessor;
    }

    public async Task<Result<RegisterDeviceResponse>> Handle(RegisterDeviceRequest request)
    {
        var token = _userAccessor.Token;

        var (response, success, message) = await _httpClientWrapper.PostHttpRequest<RegisterDeviceRequest, CreateDeviceResponse>(
            "api/deviceManagement/register",
            request,
            token);

        if (!success)
        {
            return new MicroserviceApiException(message);
        }

        return new RegisterDeviceResponse(
            response.Id,
            response.GoogleMapsPlusCode,
            response.DeviceExtraInfo,
            response.IsActivate,
            response.Username,
            response.Password,
            response.ClientId,
            response.Topic
            );
    }
}
