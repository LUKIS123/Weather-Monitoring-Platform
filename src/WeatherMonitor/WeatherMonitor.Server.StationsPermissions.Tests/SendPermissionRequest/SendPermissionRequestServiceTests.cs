using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Features.SendPermissionRequest;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.StationsPermissions.Tests.SendPermissionRequest;

[TestFixture]
public class SendPermissionRequestServiceTests
{
    private IUserAccessor _userAccessor = null!;
    private IStationsPermissionsRepository _stationsPermissionsRepository = null!;
    private ITimeZoneProvider _timeZoneProvider = null!;
    private TimeProvider _timeProvider = null!;
    private SendPermissionRequestService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _userAccessor = Substitute.For<IUserAccessor>();
        _stationsPermissionsRepository = Substitute.For<IStationsPermissionsRepository>();
        _timeZoneProvider = Substitute.For<ITimeZoneProvider>();
        _timeProvider = Substitute.For<TimeProvider>();
        _service = new SendPermissionRequestService(
            _userAccessor,
            _stationsPermissionsRepository,
            _timeZoneProvider,
            _timeProvider
        );
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenUserIdIsNullOrWhiteSpace()
    {
        // Arrange
        _userAccessor.UserId.Returns(string.Empty);

        // Act
        var result = await _service.Handle(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenUserIsAdmin()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);
        _stationsPermissionsRepository.GetStationPermissionStatusAsync(1, userId)
            .Returns(new StationUserPermissionDto { UserRole = Role.Admin });

        // Act
        var result = await _service.Handle(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<BadRequestException>();
        result.Error?.Exception.As<BadRequestException>().Message.Should().Be("Admins already have permission to access the station");
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenPermissionRequestAlreadySent()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);
        _stationsPermissionsRepository.GetStationPermissionStatusAsync(1, userId)
            .Returns(new StationUserPermissionDto { PermissionStatus = PermissionStatus.Pending });

        // Act
        var result = await _service.Handle(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<BadRequestException>();
        result.Error?.Exception.As<BadRequestException>().Message.Should().Be("Permission request already sent");
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequest_WhenUserAlreadyHasPermission()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);
        _stationsPermissionsRepository.GetStationPermissionStatusAsync(1, userId)
            .Returns(new StationUserPermissionDto { PermissionStatus = PermissionStatus.None });
        _stationsPermissionsRepository.GetUsersPermissionAsync(userId, 1)
            .Returns(new UsersPermissionDto { DeviceId = 1, UserId = userId });

        // Act
        var result = await _service.Handle(1);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<BadRequestException>();
        result.Error?.Exception.As<BadRequestException>().Message.Should().Be("User has permission to station");
    }

    [Test]
    public async Task Handle_ShouldAddPermissionRequest_WhenValid()
    {
        // Arrange
        var userId = "user123";
        var stationId = 1;
        var currentUtcTime = new DateTime(2024, 11, 26, 12, 0, 0);
        var adjustedTime = currentUtcTime.AddHours(1);

        _userAccessor.UserId.Returns(userId);
        _stationsPermissionsRepository.GetStationPermissionStatusAsync(stationId, userId)
            .Returns(new StationUserPermissionDto { PermissionStatus = PermissionStatus.None });
        _stationsPermissionsRepository.GetUsersPermissionAsync(userId, stationId)
            .Returns((UsersPermissionDto?)null);
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(currentUtcTime));
        _timeZoneProvider.GetTimeZoneInfo().Returns(TimeZoneInfo.CreateCustomTimeZone("TestZone", TimeSpan.FromHours(1), "TestZone", "TestZone"));

        // Act
        var result = await _service.Handle(stationId);

        // Assert
        await _stationsPermissionsRepository.Received(1).AddPermissionRequestAsync(stationId, userId, PermissionStatus.Pending, adjustedTime);
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRepositoryCallFails()
    {
        // Arrange
        var userId = "user123";
        var stationId = 1;

        _userAccessor.UserId.Returns(userId);
        _stationsPermissionsRepository.GetStationPermissionStatusAsync(stationId, userId)
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(stationId);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}