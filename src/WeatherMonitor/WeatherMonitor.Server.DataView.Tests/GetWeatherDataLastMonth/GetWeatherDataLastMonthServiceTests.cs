using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastMonth;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Tests.GetWeatherDataLastMonth;

[TestFixture]
public class GetWeatherDataLastMonthServiceTests
{
    private ITimeZoneProvider _timeZoneProvider = null!;
    private TimeProvider _timeProvider = null!;
    private IDataViewRepository _dataViewRepository = null!;
    private IUserAccessor _userAccessor = null!;
    private IUserAuthorizationRepository _userAuthorizationRepository = null!;
    private GetWeatherDataLastMonthService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _timeZoneProvider = Substitute.For<ITimeZoneProvider>();
        _timeProvider = Substitute.For<TimeProvider>();
        _dataViewRepository = Substitute.For<IDataViewRepository>();
        _userAccessor = Substitute.For<IUserAccessor>();
        _userAuthorizationRepository = Substitute.For<IUserAuthorizationRepository>();
        _service = new GetWeatherDataLastMonthService(
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
        _userAuthorizationRepository.GetUserAuthorizationInfoAsync(userId).Returns((BasicUserAuthorizationDto?)null);

        // Act
        var result = await _service.Handle(null, null);

        // Assert
        result.Error?.Exception.Should().BeOfType<UnauthorizedException>();
        result.Error?.Exception.As<UnauthorizedException>().Message.Should().Be("User not found");
    }

    [Test]
    public async Task Handle_ShouldCallDayAndNightTimeDataMethods_ForAdmin()
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

        _dataViewRepository.GetDayTimeLastMonthWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastMonthWeatherData { DailyData = new List<LastMonthDailyData>() }));
        _dataViewRepository.GetNightTimeLastMonthWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastMonthWeatherData { DailyData = new List<LastMonthDailyData>() }));

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        await _dataViewRepository.Received(1).GetDayTimeLastMonthWeatherDataAsync(
            Arg.Any<DateTime>(),
            deviceId,
            plusCodeSearch
        );
        await _dataViewRepository.Received(1).GetNightTimeLastMonthWeatherDataAsync(
            Arg.Any<DateTime>(),
            deviceId,
            plusCodeSearch
        );

        result.Value.Should().BeOfType<GetWeatherDataLastMonthResponse>();
    }

    [Test]
    public async Task Handle_ShouldCallDayAndNightTimeDataMethods_ForRegularUser()
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

        _dataViewRepository.GetDayTimeLastMonthWeatherDataAsync(Arg.Any<DateTime>(), userId, deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastMonthWeatherData { DailyData = new List<LastMonthDailyData>() }));
        _dataViewRepository.GetNightTimeLastMonthWeatherDataAsync(Arg.Any<DateTime>(), userId, deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastMonthWeatherData { DailyData = new List<LastMonthDailyData>() }));

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        await _dataViewRepository.Received(1).GetDayTimeLastMonthWeatherDataAsync(
            Arg.Any<DateTime>(),
            userId,
            deviceId,
            plusCodeSearch
        );
        await _dataViewRepository.Received(1).GetNightTimeLastMonthWeatherDataAsync(
            Arg.Any<DateTime>(),
            userId,
            deviceId,
            plusCodeSearch
        );

        result.Value.Should().BeOfType<GetWeatherDataLastMonthResponse>();
    }

    [Test]
    public void Handle_ShouldThrowException_WhenDayTimeDataMethodFails()
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

        _dataViewRepository.GetDayTimeLastMonthWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Throws(new Exception("Daytime data retrieval failed"));

        // Act
        Func<Task> act = async () => await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Daytime data retrieval failed");
    }

    [Test]
    public void Handle_ShouldThrowException_WhenNightTimeDataMethodFails()
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

        _dataViewRepository.GetNightTimeLastMonthWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Throws(new Exception("Nighttime data retrieval failed"));

        // Act
        Func<Task> act = async () => await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Nighttime data retrieval failed");
    }
}