using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;

namespace WeatherMonitor.Server.DeviceManagement.Features.RemoveDevice;

internal interface IRemoveDeviceService
{
    Task<Result> Handle(int deviceId);
}

internal class RemoveDeviceService : IRemoveDeviceService
{
    private readonly ICoreMicroserviceHttpClientWrapper _httpClientWrapper;
    private readonly IUserAccessor _userAccessor;

    public RemoveDeviceService(ICoreMicroserviceHttpClientWrapper httpClientWrapper, IUserAccessor userAccessor)
    {
        _httpClientWrapper = httpClientWrapper;
        _userAccessor = userAccessor;
    }

    public async Task<Result> Handle(int deviceId)
    {
        var token = _userAccessor.Token;

        var (success, message) = await _httpClientWrapper.DeleteHttpRequest(
            $"api/deviceManagement/remove?deviceId={deviceId}",
            token);

        if (!success)
        {
            return Result.OnError(new MicroserviceApiException(message));
        }

        return Result.OnSuccess();
    }
}
