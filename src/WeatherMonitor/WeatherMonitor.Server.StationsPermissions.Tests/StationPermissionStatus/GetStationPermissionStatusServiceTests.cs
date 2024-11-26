using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Features.StationPermissionStatus;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.StationsPermissions.Tests.StationPermissionStatus;

[TestFixture]
public class GetStationPermissionStatusServiceTests
{
    private IUserAccessor _userAccessor = null!;
    private IStationsPermissionsRepository _stationPermissionRepository = null!;
    private GetStationPermissionStatusService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _userAccessor = Substitute.For<IUserAccessor>();
        _stationPermissionRepository = Substitute.For<IStationsPermissionsRepository>();
        _service = new GetStationPermissionStatusService(_userAccessor, _stationPermissionRepository);
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenUserIdIsNullOrWhiteSpace()
    {
        // Arrange
        _userAccessor.UserId.Returns(string.Empty);

        // Act
        var result = await _service.Handle(1);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
    }

    [Test]
    public async Task Handle_ShouldReturnCorrectResponse_ForAdminUser()
    {
        // Arrange
        var userId = "admin123";
        var stationId = 1;
        var permissionDto = new StationUserPermissionDto
        {
            UserRole = Role.Admin,
            PermissionStatus = null,
            PermissionRecordExists = false
        };

        _userAccessor.UserId.Returns(userId);
        _stationPermissionRepository.GetStationPermissionStatusAsync(stationId, userId).Returns(permissionDto);

        // Act
        var result = await _service.Handle(stationId);

        // Assert
        result.Value.Should().BeOfType<StationPermissionStatusResponse>();
        var response = result.Value.As<StationPermissionStatusResponse>();
        response.StationUserPermission.Should().Be(permissionDto);
        response.HasPermission.Should().BeTrue();
        response.CanRequestPermission.Should().BeFalse();
    }

    [Test]
    public async Task Handle_ShouldReturnCorrectResponse_WhenUserHasGrantedPermission()
    {
        // Arrange
        var userId = "user123";
        var stationId = 1;
        var permissionDto = new StationUserPermissionDto
        {
            UserRole = Role.User,
            PermissionStatus = PermissionStatus.Granted,
            PermissionRecordExists = true
        };

        _userAccessor.UserId.Returns(userId);
        _stationPermissionRepository.GetStationPermissionStatusAsync(stationId, userId).Returns(permissionDto);

        // Act
        var result = await _service.Handle(stationId);

        // Assert
        result.Value.Should().BeOfType<StationPermissionStatusResponse>();
        var response = result.Value.As<StationPermissionStatusResponse>();
        response.StationUserPermission.Should().Be(permissionDto);
        response.HasPermission.Should().BeTrue();
        response.CanRequestPermission.Should().BeFalse();
    }

    [Test]
    public async Task Handle_ShouldReturnCorrectResponse_WhenUserCanRequestPermission()
    {
        // Arrange
        var userId = "user123";
        var stationId = 1;
        var permissionDto = new StationUserPermissionDto
        {
            UserRole = Role.User,
            PermissionStatus = PermissionStatus.NotRequested,
            PermissionRecordExists = false
        };

        _userAccessor.UserId.Returns(userId);
        _stationPermissionRepository.GetStationPermissionStatusAsync(stationId, userId).Returns(permissionDto);

        // Act
        var result = await _service.Handle(stationId);

        // Assert
        result.Value.Should().BeOfType<StationPermissionStatusResponse>();
        var response = result.Value.As<StationPermissionStatusResponse>();
        response.StationUserPermission.Should().Be(permissionDto);
        response.HasPermission.Should().BeFalse();
        response.CanRequestPermission.Should().BeTrue();
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRepositoryCallFails()
    {
        // Arrange
        var userId = "user123";
        var stationId = 1;

        _userAccessor.UserId.Returns(userId);
        _stationPermissionRepository.GetStationPermissionStatusAsync(stationId, userId)
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(stationId);

        // Assert
        act.Should().ThrowAsync<System.Exception>().WithMessage("Database error");
    }
}