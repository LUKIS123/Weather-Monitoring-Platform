using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Features.ListAvailableStations;

internal interface IListAvailableStationsService
{
    Task<Result<PageResult<AvailableStation>>> Handle(int pageNumber);
}

internal class ListAvailableStationsService : IListAvailableStationsService
{
    private readonly IStationsPermissionsRepository _stationsPermissionsRepository;
    private const int PageSize = 10;

    public ListAvailableStationsService(IStationsPermissionsRepository stationsPermissionsRepository)
    {
        _stationsPermissionsRepository = stationsPermissionsRepository;
    }

    public async Task<Result<PageResult<AvailableStation>>> Handle(int pageNumber)
    {
        var (stations, itemCount) = await _stationsPermissionsRepository.GetAvailableStationsAsync(pageNumber, PageSize);
        return new PageResult<AvailableStation>(stations, itemCount, PageSize, pageNumber);
    }
}
