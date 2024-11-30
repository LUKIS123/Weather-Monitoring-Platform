using FluentAssertions;
using NSubstitute;
using WeatherMonitorCore.MqttAuth.Features.SuperUserCheck;
using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.MqttAuth.Tests.SuperUserCheck;

[TestFixture]
public class SuperUserCheckServiceTests
{
    private IMqttClientAuthenticationRepository _mqttClientAuthenticationRepository = null!;
    private ISuperUserCheckService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mqttClientAuthenticationRepository = Substitute.For<IMqttClientAuthenticationRepository>();
        _service = new SuperUserCheckService(_mqttClientAuthenticationRepository);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var request = new SuperUserCheckRequest
        {
            Username = "nonexistentUser"
        };

        _mqttClientAuthenticationRepository
            .GetMqttSuperuserAsync(request.Username)
            .Returns(default(SuperUserCheckDto));

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<ForbidException>();
        result.Error?.Exception.Message.Should().Contain($"User:{request.Username} not found");
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserIsNotSuperUser()
    {
        // Arrange
        var request = new SuperUserCheckRequest
        {
            Username = "user1"
        };

        var superUserDto = new SuperUserCheckDto
        {
            Username = "user1",
            IsSuperUser = false
        };

        _mqttClientAuthenticationRepository
            .GetMqttSuperuserAsync(request.Username)
            .Returns(superUserDto);

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<ForbidException>();
        result.Error?.Exception.Message.Should().Contain($"User:{request.Username} is not a superuser");
    }

    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenUserIsSuperUser()
    {
        // Arrange
        var request = new SuperUserCheckRequest
        {
            Username = "superUser"
        };

        var superUserDto = new SuperUserCheckDto
        {
            Username = "superUser",
            IsSuperUser = true
        };

        _mqttClientAuthenticationRepository
            .GetMqttSuperuserAsync(request.Username)
            .Returns(superUserDto);

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldCallRepositoryWithCorrectUsername()
    {
        // Arrange
        var request = new SuperUserCheckRequest
        {
            Username = "user1"
        };

        _mqttClientAuthenticationRepository
            .GetMqttSuperuserAsync(request.Username)
            .Returns(default(SuperUserCheckDto));

        // Act
        await _service.Handle(request);

        // Assert
        await _mqttClientAuthenticationRepository
            .Received(1)
            .GetMqttSuperuserAsync(request.Username);
    }
}