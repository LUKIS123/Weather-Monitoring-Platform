using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.StationsPermissions.Features.GetUsersPermissions;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Tests.GetUsersPermissions;

[TestFixture]
public class GetUsersPermissionsServiceTests
{
    private IStationsPermissionsRepository _stationsPermissionsRepository = null!;
    private IUserAccessor _userAccessor = null!;
    private GetUsersPermissionsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _stationsPermissionsRepository = Substitute.For<IStationsPermissionsRepository>();
        _userAccessor = Substitute.For<IUserAccessor>();
        _service = new GetUsersPermissionsService(_stationsPermissionsRepository, _userAccessor);
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
    public async Task Handle_ShouldReturnPageResult_WhenUserIdIsValid()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);

        var stations = new List<UsersPermissionRequestDto>
        {
            new() { PermissionStatus = PermissionStatus.Pending, UserId = "user1" },
            new() { PermissionStatus = PermissionStatus.Granted, UserId = "user2" }
        };

        var totalItems = 20;
        _stationsPermissionsRepository.GetPermissionRequestsAsync(1, 10, userId).Returns((stations, totalItems));

        // Act
        var result = await _service.Handle(1);

        // Assert
        result.Value.Should().BeOfType<PageResult<UsersPermissionRequestDto>>();
        var pageResult = result.Value.As<PageResult<UsersPermissionRequestDto>>();
        pageResult.Items.Should().BeEquivalentTo(stations);
        pageResult.TotalItemsCount.Should().Be(totalItems);
        pageResult.PageSize.Should().Be(10);
        pageResult.TotalPages.Should().Be(2);
    }

    [Test]
    public async Task Handle_ShouldCallGetPermissionRequestsAsync_WithCorrectParameters()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);

        var stations = new List<UsersPermissionRequestDto>
        {
            new() { PermissionStatus = PermissionStatus.Pending, UserId = "user1" },
            new() { PermissionStatus = PermissionStatus.Granted, UserId = "user2" }
        };

        _stationsPermissionsRepository.GetPermissionRequestsAsync(1, 10, userId).Returns((stations, 20));

        // Act
        await _service.Handle(1);

        // Assert
        await _stationsPermissionsRepository.Received(1).GetPermissionRequestsAsync(1, 10, userId);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRepositoryCallFails()
    {
        // Arrange
        var userId = "user123";
        _userAccessor.UserId.Returns(userId);

        _stationsPermissionsRepository.GetPermissionRequestsAsync(1, 10, userId)
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(1);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}