using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitorCore.Contract.DeviceManagementModule;

namespace WeatherMonitor.Server.DataView.Features.GetStationsList;

internal interface IGetStationsListService
{
    Task<Result<PageResult<GetStationResponse>>> Handle(int pageNumber, int pageSize);
}

internal class GetStationsListService : IGetStationsListService
{
    private readonly IWeatherStationsRepository _repository;
    private readonly ICoreMicroserviceHttpClientWrapper _coreMicroserviceHttpClientWrapper;

    public GetStationsListService(
        IWeatherStationsRepository repository,
        ICoreMicroserviceHttpClientWrapper coreMicroserviceHttpClientWrapper)
    {
        _repository = repository;
        _coreMicroserviceHttpClientWrapper = coreMicroserviceHttpClientWrapper;
    }

    public async Task<Result<PageResult<GetStationResponse>>> Handle(int pageNumber, int pageSize)
    {
        var (weatherStations, itemCount) = await _repository.GetStationsAsync(pageSize, pageNumber);
        var getStationResponses = weatherStations as GetStationResponse[] ?? weatherStations.ToArray();

        var stationsToSetActive = getStationResponses
            .Where(x => x is { IsActive: false, ReceivedAt: not null })
            .Select(x =>
                new UpdateDeviceStatus(
                x.DeviceId,
                true))
            .ToArray();

        if (stationsToSetActive.Length > 0)
        {
            await _coreMicroserviceHttpClientWrapper.PostHttpRequest(
                "api/deviceManagement/statusUpdate",
                new UpdateDeviceStatusRequest(stationsToSetActive));
        }

        return new PageResult<GetStationResponse>(getStationResponses, itemCount, pageSize, pageNumber);
    }
}
