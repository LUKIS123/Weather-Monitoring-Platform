using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastDay;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Tests.GetWeatherDataLastDay;

[TestFixture]
public class GetWeatherDataLastDayServiceTests
{
    private ITimeZoneProvider _timeZoneProvider = null!;
    private TimeProvider _timeProvider = null!;
    private IDataViewRepository _dataViewRepository = null!;
    private IUserAccessor _userAccessor = null!;
    private IUserAuthorizationRepository _userAuthorizationRepository = null!;
    private GetWeatherDataLastDayService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _timeZoneProvider = Substitute.For<ITimeZoneProvider>();
        _timeProvider = Substitute.For<TimeProvider>();
        _dataViewRepository = Substitute.For<IDataViewRepository>();
        _userAccessor = Substitute.For<IUserAccessor>();
        _userAuthorizationRepository = Substitute.For<IUserAuthorizationRepository>();
        _service = new GetWeatherDataLastDayService(
            _timeZoneProvider,
            _timeProvider,
            _dataViewRepository,
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
        var result = await _service.Handle(null, null);

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
        var result = await _service.Handle(null, null);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
        result.Error?.Exception.As<UnauthorizedException>().Message.Should().Be("User not found");
    }

    [Test]
    public async Task Handle_ShouldCallGetLastDayWeatherDataAsync_ForAdmin()
    {
        // Arrange
        var userId = "admin123";
        var deviceId = 1;
        var plusCodeSearch = "1234+56";
        var utcNow = new DateTime(2024, 11, 26, 12, 0, 0);
        var timeZone = TimeZoneInfo.Utc;

        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.Admin });
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(utcNow));
        _timeZoneProvider.GetTimeZoneInfo().Returns(timeZone);

        _dataViewRepository.GetLastDayWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Returns(new LastDayWeatherData { HourlyData = Array.Empty<LastDayHourlyData>() });

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        await _dataViewRepository.Received(1).GetLastDayWeatherDataAsync(
            Arg.Is<DateTime>(d => d == utcNow),
            deviceId,
            plusCodeSearch
        );

        result.Value.Should().BeOfType<GetWeatherDataLastDayResponse>();
    }

    [Test]
    public async Task Handle_ShouldCallGetLastDayWeatherDataAsync_ForRegularUser()
    {
        // Arrange
        var userId = "user123";
        var deviceId = 1;
        var plusCodeSearch = "1234+56";
        var utcNow = new DateTime(2024, 11, 26, 12, 0, 0);
        var timeZone = TimeZoneInfo.Utc;

        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.User });
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(utcNow));
        _timeZoneProvider.GetTimeZoneInfo().Returns(timeZone);

        _dataViewRepository.GetLastDayWeatherDataAsync(Arg.Any<DateTime>(), userId, deviceId, plusCodeSearch)
            .Returns(new LastDayWeatherData { HourlyData = Array.Empty<LastDayHourlyData>() });

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        await _dataViewRepository.Received(1).GetLastDayWeatherDataAsync(
            Arg.Is<DateTime>(d => d == utcNow),
            userId,
            deviceId,
            plusCodeSearch
        );

        result.Value.Should().BeOfType<GetWeatherDataLastDayResponse>();
    }

    [Test]
    public void Handle_ShouldThrowException_WhenGetLastDayWeatherDataAsyncFails()
    {
        // Arrange
        var userId = "admin123";
        var deviceId = 1;
        var plusCodeSearch = "1234+56";
        var utcNow = new DateTime(2024, 11, 26, 12, 0, 0);
        var timeZone = TimeZoneInfo.Utc;

        _userAccessor.UserId.Returns(userId);
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns(new BasicUserAuthorizationDto { Id = "1", Role = Role.Admin });
        _timeProvider.GetUtcNow().Returns(new DateTimeOffset(utcNow));
        _timeZoneProvider.GetTimeZoneInfo().Returns(timeZone);

        _dataViewRepository.GetLastDayWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}