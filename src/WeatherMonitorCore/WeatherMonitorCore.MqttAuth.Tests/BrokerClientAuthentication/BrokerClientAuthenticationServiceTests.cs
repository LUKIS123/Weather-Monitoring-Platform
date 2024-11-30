using FluentAssertions;
using NSubstitute;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.MqttAuth.Features.BrokerClientAuthentication;
using WeatherMonitorCore.MqttAuth.Infrastructure;
using WeatherMonitorCore.MqttAuth.Infrastructure.Models;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.MqttAuth.Tests.BrokerClientAuthentication;

[TestFixture]
public class BrokerClientAuthenticationServiceTests
{
    private IMqttClientAuthenticationRepository _mqttClientAuthenticationRepository = null!;
    private IAesEncryptionHelper _aesEncryptionHelper = null!;
    private IBrokerClientAuthenticationService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _mqttClientAuthenticationRepository = Substitute.For<IMqttClientAuthenticationRepository>();
        _aesEncryptionHelper = Substitute.For<IAesEncryptionHelper>();
        _service = new BrokerClientAuthenticationService(_mqttClientAuthenticationRepository, _aesEncryptionHelper);
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenUserNotFound()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Username = "nonexistentUser",
            ClientId = "client1",
            Password = "password"
        };

        _mqttClientAuthenticationRepository
            .GetMqttCredentialsAsync(request.Username, request.ClientId)
            .Returns(default(BrokerClientAuthenticationDto));

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<ForbidException>();
        result.Error?.Exception.Message.Should().Contain($"User:{request.Username} not found");
    }

    [Test]
    public async Task Handle_ShouldReturnError_WhenAuthenticationFails()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Username = "user1",
            ClientId = "client1",
            Password = "wrongPassword"
        };

        var authDto = new BrokerClientAuthenticationDto
        {
            Username = "user1",
            ClientId = "client1",
            Password = "encryptedPassword"
        };

        _mqttClientAuthenticationRepository
            .GetMqttCredentialsAsync(request.Username, request.ClientId)
            .Returns(authDto);

        _aesEncryptionHelper.Decrypt(authDto.Password).Returns("decryptedPassword");

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<ForbidException>();
        result.Error?.Exception.Message.Should().Contain($"User:{request.Username} not authentication failed");
    }

    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenAuthenticationSucceeds()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Username = "user1",
            ClientId = "client1",
            Password = "correctPassword"
        };

        var authDto = new BrokerClientAuthenticationDto
        {
            Username = "user1",
            ClientId = "client1",
            Password = "encryptedPassword"
        };

        _mqttClientAuthenticationRepository
            .GetMqttCredentialsAsync(request.Username, request.ClientId)
            .Returns(authDto);

        _aesEncryptionHelper.Decrypt(authDto.Password).Returns("correctPassword");

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldCallRepositoryWithCorrectParameters()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Username = "user1",
            ClientId = "client1",
            Password = "password"
        };

        _mqttClientAuthenticationRepository
            .GetMqttCredentialsAsync(request.Username, request.ClientId)
            .Returns(default(BrokerClientAuthenticationDto));

        // Act
        await _service.Handle(request);

        // Assert
        await _mqttClientAuthenticationRepository
            .Received(1)
            .GetMqttCredentialsAsync(request.Username, request.ClientId);
    }

    [Test]
    public async Task Handle_ShouldDecryptPassword_WhenUserExists()
    {
        // Arrange
        var request = new AuthenticationRequest
        {
            Username = "user1",
            ClientId = "client1",
            Password = "password"
        };

        var authDto = new BrokerClientAuthenticationDto
        {
            Username = "user1",
            ClientId = "client1",
            Password = "encryptedPassword"
        };

        _mqttClientAuthenticationRepository
            .GetMqttCredentialsAsync(request.Username, request.ClientId)
            .Returns(authDto);

        // Act
        await _service.Handle(request);

        // Assert
        _aesEncryptionHelper.Received(1).Decrypt(authDto.Password);
    }
}