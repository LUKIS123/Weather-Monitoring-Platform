using FluentAssertions;
using NSubstitute;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure;
using WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserPermissionManagement.Tests.SetUserPermission;

[TestFixture]
public class SetUsersStationPermissionServiceTests
{
    private ISetUsersStationPermissionService _service = null!;
    private IUserAccessor _userAccessor = null!;
    private IUserAuthorizationRepository _userAuthorizationRepository = null!;
    private IUserManagementRepository _userManagementRepository = null!;
    private ITimeZoneProvider _timeZoneProvider = null!;
    private TimeProvider _timeProvider = null!;

    [SetUp]
    public void SetUp()
    {
        _userAccessor = Substitute.For<IUserAccessor>();
        _userAuthorizationRepository = Substitute.For<IUserAuthorizationRepository>();
        _userManagementRepository = Substitute.For<IUserManagementRepository>();
        _timeZoneProvider = Substitute.For<ITimeZoneProvider>();
        _timeProvider = Substitute.For<TimeProvider>();

        _timeZoneProvider.GetTimeZoneInfo().Returns(TimeZoneInfo.Utc);

        _service = new SetUsersStationPermissionService(
            _userAccessor,
            _userAuthorizationRepository,
            _userManagementRepository,
            _timeZoneProvider,
            _timeProvider);
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenUserIdIsNullOrWhiteSpace()
    {
        // Arrange
        _userAccessor.UserId.Returns((string?)null);

        var request = new UpdatePermissionRequest
        {
            UserId = "user1",
            DeviceId = 1,
            Status = PermissionStatus.Granted
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenUserIsNotAdmin()
    {
        // Arrange
        _userAccessor.UserId.Returns("admin");
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync("admin")!
            .Returns(Task.FromResult(new BasicUserAuthorizationDto { Id = "admin", Role = Role.User }));

        var request = new UpdatePermissionRequest
        {
            UserId = "user1",
            DeviceId = 1,
            Status = PermissionStatus.Granted
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>().Which.Message.Should().Be("User must be administrator");
    }

    [Test]
    public async Task Handle_ShouldReturnUnauthorizedException_WhenPermissionRequestDoesNotExist()
    {
        // Arrange
        _userAccessor.UserId.Returns("admin");
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync("admin")!
            .Returns(Task.FromResult(new BasicUserAuthorizationDto { Id = "admin", Role = Role.Admin }));
        _userManagementRepository.GetPermissionRequestAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult(default(UserPermissionRequestDto?)));

        var request = new UpdatePermissionRequest
        {
            UserId = "user1",
            DeviceId = 1,
            Status = PermissionStatus.Granted
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>().Which.Message.Should().Be("Permission request does not exist");
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequestException_WhenUserAlreadyHasPermission()
    {
        // Arrange
        _userAccessor.UserId.Returns("admin");
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync("admin")!
            .Returns(Task.FromResult(new BasicUserAuthorizationDto { Id = "admin", Role = Role.Admin }));
        var permissionRequest = new UserPermissionRequestDto { PermissionStatus = PermissionStatus.Granted };
        _userManagementRepository.GetPermissionRequestAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult((UserPermissionRequestDto?)permissionRequest));

        var request = new UpdatePermissionRequest
        {
            UserId = "user1",
            DeviceId = 1,
            Status = PermissionStatus.Granted
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.Error?.Exception.Should().BeOfType<BadRequestException>().Which.Message.Should().Be("User already has permission to the station");
    }

    [Test]
    public async Task Handle_ShouldReturnBadRequestException_WhenUserDoesNotHavePermissionToRemove()
    {
        // Arrange
        _userAccessor.UserId.Returns("admin");
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync("admin")!
            .Returns(Task.FromResult(new BasicUserAuthorizationDto { Id = "admin", Role = Role.Admin }));
        _userManagementRepository.GetPermissionRequestAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult((UserPermissionRequestDto?)new UserPermissionRequestDto { PermissionStatus = PermissionStatus.Granted }));
        _userManagementRepository.GetUserPermissionAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult(default(UserPermissionDto?))); // user has no permission

        var request = new UpdatePermissionRequest
        {
            UserId = "user1",
            DeviceId = 1,
            Status = PermissionStatus.Denied
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.Error?.Exception.Should().BeOfType<BadRequestException>().Which.Message.Should().Be("User does not have permission to the station");
    }

    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenPermissionStatusIsUpdatedToGranted()
    {
        // Arrange
        _userAccessor.UserId.Returns("admin");
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync("admin")!
            .Returns(Task.FromResult(new BasicUserAuthorizationDto { Id = "admin", Role = Role.Admin }));
        var permissionRequest = new UserPermissionRequestDto { PermissionStatus = PermissionStatus.Pending };
        var userPermission = new UserPermissionDto();
        _userManagementRepository.GetPermissionRequestAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult((UserPermissionRequestDto?)permissionRequest));
        _userManagementRepository.GetUserPermissionAsync(Arg.Any<string>(), Arg.Any<int>())
            .Returns(Task.FromResult((UserPermissionDto?)userPermission));

        var request = new UpdatePermissionRequest
        {
            UserId = "user1",
            DeviceId = 1,
            Status = PermissionStatus.Granted
        };

        var result = (new UserPermissionRequestDto(), new UserPermissionDto());
        _userManagementRepository.AddUserStationPermissionAsync(Arg.Any<string>(), Arg.Any<int>(), Arg.Any<PermissionStatus>(), Arg.Any<DateTime>())
            .Returns(Task.FromResult(result));

        // Act
        var response = await _service.Handle(request);

        // Assert
        response.Value.Should().BeOfType<UpdatePermissionResponse>();
    }
}