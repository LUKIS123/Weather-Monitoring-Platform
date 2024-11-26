using FluentAssertions;
using NSubstitute;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Features.GetDeviceMqttCredentials;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.DeviceManagement.Tests.GetDeviceMqttCredentials;

[TestFixture]
public class GetMqttCredentialsServiceTests
{
    private IDeviceManagementRepository _deviceManagementRepository = null!;
    private IAesEncryptionHelper _aesEncryptionHelper = null!;
    private GetMqttCredentialsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _deviceManagementRepository = Substitute.For<IDeviceManagementRepository>();
        _aesEncryptionHelper = Substitute.For<IAesEncryptionHelper>();
        _service = new GetMqttCredentialsService(_deviceManagementRepository, _aesEncryptionHelper);
    }

    [Test]
    public async Task Handle_ShouldReturnNotFoundException_WhenDeviceHasMissingFields()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto
        {
            Id = deviceId,
            ClientId = null!, // Missing required field
            Username = "test-user",
            Password = "encrypted-password",
            Topic = "test/topic"
        };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);

        // Act
        var result = await _service.Handle(deviceId);

        // Assert
        result.Error?.Exception.Should().BeOfType<NotFoundException>();
        result.Error?.Exception.As<NotFoundException>().Message.Should().Contain($"Device with id:{deviceId} not found");
    }

    [Test]
    public async Task Handle_ShouldReturnDeviceMqttCredentialsResponse_WhenDeviceIsValid()
    {
        // Arrange
        var deviceId = 1;
        var encryptedPassword = "encrypted-password";
        var decryptedPassword = "plain-text-password";
        var device = new MqttCredentialsDto
        {
            Id = deviceId,
            ClientId = "client-id",
            Username = "test-user",
            Password = encryptedPassword,
            Topic = "test/topic"
        };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);
        _aesEncryptionHelper.Decrypt(encryptedPassword).Returns(decryptedPassword);

        // Act
        var result = (await _service.Handle(deviceId)).Value;

        // Assert
        result.Should().BeOfType<DeviceMqttCredentialsResponse>();
        var response = result.As<DeviceMqttCredentialsResponse>();
        response.Id.Should().Be(deviceId);
        response.ClientId.Should().Be("client-id");
        response.Username.Should().Be("test-user");
        response.Password.Should().Be(decryptedPassword);
        response.Topic.Should().Be("test/topic");
    }

    [Test]
    public async Task Handle_ShouldCallGetDeviceByIdAsync_Once()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto
        {
            Id = deviceId,
            ClientId = "client-id",
            Username = "test-user",
            Password = "encrypted-password",
            Topic = "test/topic"
        };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);

        // Act
        await _service.Handle(deviceId);

        // Assert
        await _deviceManagementRepository.Received(1).GetDeviceByIdAsync(deviceId);
    }

    [Test]
    public async Task Handle_ShouldCallDecrypt_Once_WhenDevicePasswordIsNotNull()
    {
        // Arrange
        var deviceId = 1;
        var encryptedPassword = "encrypted-password";
        var device = new MqttCredentialsDto
        {
            Id = deviceId,
            ClientId = "client-id",
            Username = "test-user",
            Password = encryptedPassword,
            Topic = "test/topic"
        };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);

        // Act
        await _service.Handle(deviceId);

        // Assert
        _aesEncryptionHelper.Received(1).Decrypt(encryptedPassword);
    }

    [Test]
    public async Task Handle_ShouldNotCallDecrypt_WhenDevicePasswordIsNull()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto
        {
            Id = deviceId,
            ClientId = "client-id",
            Username = "test-user",
            Password = null!, // Password is null
            Topic = "test/topic"
        };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);

        // Act
        var result = await _service.Handle(deviceId);

        // Assert
        _aesEncryptionHelper.DidNotReceive().Decrypt(Arg.Any<string>());
    }
}