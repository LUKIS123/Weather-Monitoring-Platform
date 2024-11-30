using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.DataView.Features.GetWeatherDataLastWeek;
using WeatherMonitor.Server.DataView.Infrastructure;
using WeatherMonitor.Server.DataView.Infrastructure.Models;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Exceptions;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.DataView.Tests.GetWeatherDataLastWeek;

[TestFixture]
public class GetWeatherDataLastWeekServiceTests
{
    private ITimeZoneProvider _timeZoneProvider = null!;
    private TimeProvider _timeProvider = null!;
    private IDataViewRepository _dataViewRepository = null!;
    private IUserAccessor _userAccessor = null!;
    private IUserAuthorizationRepository _userAuthorizationRepository = null!;
    private GetWeatherDataLastWeekService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _timeZoneProvider = Substitute.For<ITimeZoneProvider>();
        _timeProvider = Substitute.For<TimeProvider>();
        _dataViewRepository = Substitute.For<IDataViewRepository>();
        _userAccessor = Substitute.For<IUserAccessor>();
        _userAuthorizationRepository = Substitute.For<IUserAuthorizationRepository>();
        _service = new GetWeatherDataLastWeekService(
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
    public async Task Handle_ShouldCallGetLastWeekWeatherDataAsync_ForAdmin()
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

        _dataViewRepository.GetLastWeekWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastWeekWeatherData { LastWeekHourlyData = [] }));

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        await _dataViewRepository.Received(1).GetLastWeekWeatherDataAsync(
            Arg.Any<DateTime>(),
            deviceId,
            plusCodeSearch
        );

        result.Value.Should().BeOfType<GetWeatherDataLastWeekResponse>();
    }

    [Test]
    public async Task Handle_ShouldCallGetLastWeekWeatherDataAsync_ForRegularUser()
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

        _dataViewRepository.GetLastWeekWeatherDataAsync(Arg.Any<DateTime>(), userId, deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastWeekWeatherData { LastWeekHourlyData = [] }));

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        await _dataViewRepository.Received(1).GetLastWeekWeatherDataAsync(
            Arg.Any<DateTime>(),
            userId,
            deviceId,
            plusCodeSearch
        );

        result.Value.Should().BeOfType<GetWeatherDataLastWeekResponse>();
    }

    [Test]
    public async Task Handle_ShouldCalculateCorrectStartDateTime_WhenDataIsAvailable()
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

        var lastWeekData = new[]
        {
            new LastWeekHourlyData { HourDateTime = new DateTime(2024, 11, 19, 12, 0, 0) }
        };

        _dataViewRepository.GetLastWeekWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastWeekWeatherData { LastWeekHourlyData = lastWeekData }));

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        result.Value.Should().BeOfType<GetWeatherDataLastWeekResponse>();
        var response = result.Value.As<GetWeatherDataLastWeekResponse>();
        response.StartDateTime.Should().Be(lastWeekData.First().HourDateTime);
    }

    [Test]
    public async Task Handle_ShouldCalculateStartDateTimeAsOneWeekAgo_WhenNoDataAvailable()
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

        _dataViewRepository.GetLastWeekWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Returns(Task.FromResult(new LastWeekWeatherData { LastWeekHourlyData = [] }));

        // Act
        var result = await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        result.Value.Should().BeOfType<GetWeatherDataLastWeekResponse>();
        var response = result.Value.As<GetWeatherDataLastWeekResponse>();
        response.StartDateTime.Should().Be(utcNow.AddDays(-7));
    }

    [Test]
    public void Handle_ShouldThrowException_WhenDataRetrievalFails()
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

        _dataViewRepository.GetLastWeekWeatherDataAsync(Arg.Any<DateTime>(), deviceId, plusCodeSearch)
            .Throws(new Exception("Data retrieval failed"));

        // Act
        Func<Task> act = async () => await _service.Handle(deviceId, plusCodeSearch);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Data retrieval failed");
    }
}