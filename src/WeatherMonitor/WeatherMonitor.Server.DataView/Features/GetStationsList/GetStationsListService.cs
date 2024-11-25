using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Features.GetStationsList;

internal interface IGetStationsListService
{
    Task<Result<PageResult<GetStationResponse>>> Handle(int pageNumber, int pageSize);
}

internal class GetStationsListService : IGetStationsListService
{
    private readonly IWeatherStationsRepository _repository;
    private readonly ICoreMicroserviceHttpClientWrapper _coreMicroserviceHttpClientWrapper;
    private readonly IUserAccessor _userAccessor;
    private readonly IUserAuthorizationRepository _userAuthorizationRepository;

    public GetStationsListService(
        IWeatherStationsRepository repository,
        ICoreMicroserviceHttpClientWrapper coreMicroserviceHttpClientWrapper,
        IUserAccessor userAccessor,
        IUserAuthorizationRepository userAuthorizationRepository)
    {
        _repository = repository;
        _coreMicroserviceHttpClientWrapper = coreMicroserviceHttpClientWrapper;
        _userAccessor = userAccessor;
        _userAuthorizationRepository = userAuthorizationRepository;
    }

    public async Task<Result<PageResult<GetStationResponse>>> Handle(int pageNumber, int pageSize)
    {
        var userId = _userAccessor.UserId;
        if (string.IsNullOrWhiteSpace(userId))
        {
            return new UnauthorizedException("User not authenticated");
        }

        var user = await _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId);
        if (user is null)
        {
            return new UnauthorizedException("User not found");
        }

        var (weatherStations, itemCount) = user.Role == Role.Admin
            ? await _repository.GetStationsAsync(pageSize, pageNumber)
            : await _repository.GetStationsAsync(pageSize, pageNumber, userId);

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
