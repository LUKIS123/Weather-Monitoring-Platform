using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitorCore.Contract.DeviceManagementModule;

namespace WeatherMonitor.Server.DeviceManagement.Features.GetCredentials;

internal interface IGetMqttCredentialsService
{
    Task<Result<DeviceMqttCredentialsResponse>> Handle(int deviceId);
}

internal class GetMqttCredentialsService : IGetMqttCredentialsService
{
    private readonly ICoreMicroserviceHttpClientWrapper _httpClientWrapper;
    private readonly IUserAccessor _userAccessor;

    public GetMqttCredentialsService(ICoreMicroserviceHttpClientWrapper httpClientWrapper, IUserAccessor userAccessor)
    {
        _httpClientWrapper = httpClientWrapper;
        _userAccessor = userAccessor;
    }

    public async Task<Result<DeviceMqttCredentialsResponse>> Handle(int deviceId)
    {
        var token = _userAccessor.Token;

        var (response, success, message) = await _httpClientWrapper.GetHttpRequest<DeviceMqttCredentialsResponse>(
            $"api/deviceManagement/credentials?deviceId={deviceId}",
            token);

        if (!success)
        {
            return new MicroserviceApiException(message);
        }

        return response;
    }
}
