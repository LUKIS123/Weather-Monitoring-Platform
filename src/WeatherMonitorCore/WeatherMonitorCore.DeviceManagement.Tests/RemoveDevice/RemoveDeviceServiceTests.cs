using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitorCore.DeviceManagement.Features.RemoveDevice;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;
using WeatherMonitorCore.SharedKernel;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.DeviceManagement.Tests.RemoveDevice;

[TestFixture]
public class RemoveDeviceServiceTests
{
    private IDeviceManagementRepository _deviceManagementRepository;
    private ISubscriptionsManagingService _subscriptionsManagingService;
    private RemoveDeviceService _service;

    [SetUp]
    public void SetUp()
    {
        _deviceManagementRepository = Substitute.For<IDeviceManagementRepository>();
        _subscriptionsManagingService = Substitute.For<ISubscriptionsManagingService>();
        _service = new RemoveDeviceService(_deviceManagementRepository, _subscriptionsManagingService);
    }

    [TearDown]
    public void TearDown()
    {
        _subscriptionsManagingService.Dispose();
    }

    [Test]
    public async Task Handle_ShouldReturnErrorResult_WhenDeviceNotFound()
    {
        // Arrange
        var deviceId = 1;
        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(default(MqttCredentialsDto));

        // Act
        var result = await _service.Handle(deviceId);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeFalse();
        result.Error?.Exception.Should().BeOfType<NotFoundException>();
        result.Error?.Exception.As<NotFoundException>().Message.Should().Contain($"Device with id:{deviceId} not found");
    }

    [Test]
    public async Task Handle_ShouldReturnSuccessResult_WhenDeviceIsRemoved()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto { Id = deviceId, Topic = "test/topic" };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);
        _subscriptionsManagingService.RemoveTopicAsync(device.Topic, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);
        _deviceManagementRepository.RemoveDeviceAsync(deviceId).Returns(Task.CompletedTask);

        // Act
        var result = await _service.Handle(deviceId);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldCallRemoveTopicAsync_WhenDeviceExists()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto { Id = deviceId, Topic = "test/topic" };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);

        // Act
        await _service.Handle(deviceId);

        // Assert
        await _subscriptionsManagingService.Received(1).RemoveTopicAsync(device.Topic, Arg.Any<CancellationToken>());
    }

    [Test]
    public async Task Handle_ShouldCallRemoveDeviceAsync_WhenDeviceExists()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto { Id = deviceId, Topic = "test/topic" };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);

        // Act
        await _service.Handle(deviceId);

        // Assert
        await _deviceManagementRepository.Received(1).RemoveDeviceAsync(deviceId);
    }

    [Test]
    public async Task Handle_ShouldNotCallRemoveDeviceAsync_WhenDeviceNotFound()
    {
        // Arrange
        var deviceId = 1;
        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(default(MqttCredentialsDto));

        // Act
        await _service.Handle(deviceId);

        // Assert
        await _deviceManagementRepository.DidNotReceive().RemoveDeviceAsync(deviceId);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRemoveTopicAsyncFails()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto { Id = deviceId, Topic = "test/topic" };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);
        _subscriptionsManagingService.RemoveTopicAsync(device.Topic, Arg.Any<CancellationToken>())
            .Throws(new Exception("Topic removal failed"));

        // Act
        Func<Task> act = async () => await _service.Handle(deviceId);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Topic removal failed");
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRemoveDeviceAsyncFails()
    {
        // Arrange
        var deviceId = 1;
        var device = new MqttCredentialsDto { Id = deviceId, Topic = "test/topic" };

        _deviceManagementRepository.GetDeviceByIdAsync(deviceId).Returns(device);
        _deviceManagementRepository.RemoveDeviceAsync(deviceId).Throws(new Exception("Device removal failed"));

        // Act
        Func<Task> act = async () => await _service.Handle(deviceId);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Device removal failed");
    }
}