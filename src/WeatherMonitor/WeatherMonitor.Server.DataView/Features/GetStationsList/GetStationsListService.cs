using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.SharedKernel;

namespace WeatherMonitor.Server.DataView.Features.GetStationsList;

internal interface IGetStationsListService
{
    Task<Result<PageResult<GetStationResponse>>> Handle(int pageNumber, int pageSize);
}

internal class GetStationsListService : IGetStationsListService
{
    private readonly IWeatherStationsRepository _repository;

    public GetStationsListService(IWeatherStationsRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result<PageResult<GetStationResponse>>> Handle(int pageNumber, int pageSize)
    {
        var (weatherStations, itemCount) = await _repository.GetStationsAsync(pageSize, pageNumber);
        var getStationResponses = weatherStations as GetStationResponse[] ?? weatherStations.ToArray();

        var stationIdsToSetActive = getStationResponses
            .Where(x => x is { IsActive: false, ReceivedAt: not null })
            .Select(x => x.DeviceId).ToArray();
        await _repository.SetStationsActiveAsync(stationIdsToSetActive);

        return new PageResult<GetStationResponse>(getStationResponses, itemCount, pageSize, pageNumber);
    }
}
