using FluentAssertions;
using NSubstitute;
using WeatherMonitor.Server.DataView.Features.GetStationsList;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Tests.GetStationsList;

[TestFixture]
public class GetStationsListServiceTests
{
    private IWeatherStationsRepository _repository = null!;
    private ICoreMicroserviceHttpClientWrapper _coreMicroserviceHttpClientWrapper = null!;
    private IUserAccessor _userAccessor = null!;
    private IUserAuthorizationRepository _userAuthorizationRepository = null!;
    private GetStationsListService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _repository = Substitute.For<IWeatherStationsRepository>();
        _coreMicroserviceHttpClientWrapper = Substitute.For<ICoreMicroserviceHttpClientWrapper>();
        _userAccessor = Substitute.For<IUserAccessor>();
        _userAuthorizationRepository = Substitute.For<IUserAuthorizationRepository>();
        _service = new GetStationsListService(
            _repository,
            _coreMicroserviceHttpClientWrapper,
            _userAccessor,
            _userAuthorizationRepository
        );
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenUserIdIsNullOrWhiteSpace()
    {
        // Arrange
        _userAccessor.UserId.Returns(string.Empty);

        // Act
        var result = await _service.Handle(1, 10);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
        result.Error?.Exception.As<UnauthorizedException>().Message.Should().Be("User not authenticated");
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenUserNotFound()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns((BasicUserAuthorizationDto)null!);

        // Act
        var result = await _service.Handle(1, 10);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
        result.Error?.Exception.As<UnauthorizedException>().Message.Should().Be("User not found");
    }

    [Test]
    public async Task Handle_ShouldRetrieveStationsForAdmin_WhenUserIsAdmin()
    {
        // Arrange
        var userId = "admin123";
        var stations = new List<GetStationResponse>
        {
            new GetStationResponse { DeviceId = 1, IsActive = true },
            new GetStationResponse { DeviceId = 2, IsActive = false }
        };
        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.Admin });
        _repository.GetStationsAsync(10, 1).Returns((stations, 2));

        // Act
        var result = await _service.Handle(1, 10);

        // Assert
        result.Value.Should().BeOfType<PageResult<GetStationResponse>>();
        var pageResult = result.Value.As<PageResult<GetStationResponse>>();
        pageResult.Items.Should().BeEquivalentTo(stations);
        pageResult.TotalItemsCount.Should().Be(2);

        await _repository.Received(1).GetStationsAsync(10, 1);
        await _repository.DidNotReceiveWithAnyArgs().GetStationsAsync(default, default, default!);
    }

    [Test]
    public async Task Handle_ShouldRetrieveStationsForRegularUser_WhenUserIsNotAdmin()
    {
        // Arrange
        var userId = "user123";
        var stations = new List<GetStationResponse>
        {
            new GetStationResponse { DeviceId = 1, IsActive = true },
            new GetStationResponse { DeviceId = 2, IsActive = false }
        };
        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.User });
        _repository.GetStationsAsync(10, 1, userId).Returns((stations, 2));

        // Act
        var result = await _service.Handle(1, 10);

        // Assert
        result.Value.Should().BeOfType<PageResult<GetStationResponse>>();
        var pageResult = result.Value.As<PageResult<GetStationResponse>>();
        pageResult.Items.Should().BeEquivalentTo(stations);
        pageResult.TotalItemsCount.Should().Be(2);

        await _repository.DidNotReceiveWithAnyArgs().GetStationsAsync(default, default);
        await _repository.Received(1).GetStationsAsync(10, 1, userId);
    }

    [Test]
    public async Task Handle_ShouldSendStatusUpdateRequest_WhenStationsToSetActiveAreFound()
    {
        // Arrange
        var userId = "admin123";
        var stations = new List<GetStationResponse>
        {
            new GetStationResponse { DeviceId = 1, IsActive = false, ReceivedAt = DateTime.UtcNow },
            new GetStationResponse { DeviceId = 2, IsActive = true }
        };
        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.Admin });
        _repository.GetStationsAsync(10, 1).Returns((stations, 2));

        // Act
        var result = await _service.Handle(1, 10);

        // Assert
        await _coreMicroserviceHttpClientWrapper.Received(1).PostHttpRequest(
            "api/deviceManagement/statusUpdate",
            Arg.Is<UpdateDeviceStatusRequest>(req =>
                req.StationsUpdates.Any(x => x.DeviceId == 1 && x.SetActive == true)
            )
        );

        result.Value.Should().BeOfType<PageResult<GetStationResponse>>();
    }

    [Test]
    public async Task Handle_ShouldNotSendStatusUpdateRequest_WhenNoStationsToSetActiveAreFound()
    {
        // Arrange
        var userId = "admin123";
        var stations = new List<GetStationResponse>
        {
            new GetStationResponse { DeviceId = 1, IsActive = true },
            new GetStationResponse { DeviceId = 2, IsActive = true }
        };
        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.Admin });
        _repository.GetStationsAsync(10, 1).Returns((stations, 2));

        // Act
        var result = await _service.Handle(1, 10);

        // Assert
        await _coreMicroserviceHttpClientWrapper.DidNotReceiveWithAnyArgs().PostHttpRequest<UpdateDeviceStatusRequest>(default!, default);
        result.Value.Should().BeOfType<PageResult<GetStationResponse>>();
    }
}