using FluentAssertions;
using NSubstitute;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.MqttAuth.Features.AclCheck;
using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.MqttAuth.Tests.AclCheck;

[TestFixture]
public class AclCheckServiceTests
{
    private IAclCheckService _service = null!;
    private IMqttClientAuthenticationRepository _mqttClientAuthenticationRepository = null!;

    [SetUp]
    public void SetUp()
    {
        _mqttClientAuthenticationRepository = Substitute.For<IMqttClientAuthenticationRepository>();
        _service = new AclCheckService(_mqttClientAuthenticationRepository);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var request = new AclCheckRequest
        {
            Username = "nonexistentUser",
            ClientId = "client1",
            Action = ActionType.Write,
            Topic = "test/topic"
        };

        _mqttClientAuthenticationRepository
            .GetMqttAclAsync(request.Username, request.ClientId)
            .Returns(default(AclCheckDto));

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<ForbidException>();
        result.Error?.Exception.Message.Should().Be($"User:{request.Username} not found");
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenAclCheckFails()
    {
        // Arrange
        var request = new AclCheckRequest
        {
            Username = "user1",
            ClientId = "client1",
            Action = ActionType.Write,
            Topic = "restricted/topic"
        };

        var aclDto = new AclCheckDto
        {
            Username = "user1",
            ClientId = "client1",
            Topic = "allowed/topic",
            AllowedActions = [ActionType.Read]
        };

        _mqttClientAuthenticationRepository
            .GetMqttAclAsync(request.Username, request.ClientId)
            .Returns(aclDto);

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<ForbidException>();
        result.Error?.Exception.Message.Should().Contain($"User:{request.Username} not allowed to {request.Action} on {request.Topic}");
    }

    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenAclCheckPasses()
    {
        // Arrange
        var request = new AclCheckRequest
        {
            Username = "user1",
            ClientId = "client1",
            Action = ActionType.Write,
            Topic = "allowed/topic"
        };

        var aclDto = new AclCheckDto
        {
            Username = "user1",
            ClientId = "client1",
            Topic = "allowed/topic",
            AllowedActions = [ActionType.Write, ActionType.Read]
        };

        _mqttClientAuthenticationRepository
            .GetMqttAclAsync(request.Username, request.ClientId)
            .Returns(aclDto);

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var request = new AclCheckRequest
        {
            Username = "user1",
            ClientId = "client1",
            Action = ActionType.Write,
            Topic = "test/topic"
        };

        _mqttClientAuthenticationRepository
            .GetMqttAclAsync(request.Username, request.ClientId)
            .Returns(default(AclCheckDto));

        // Act
        await _service.Handle(request);

        // Assert
        await _mqttClientAuthenticationRepository
            .Received(1)
            .GetMqttAclAsync(request.Username, request.ClientId);
    }
}