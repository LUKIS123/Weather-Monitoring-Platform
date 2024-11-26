using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Features.UpdateDeviceStatus;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.SharedKernel;

namespace WeatherMonitorCore.DeviceManagement.Tests.UpdateDeviceStatus;

[TestFixture]
public class UpdateDeviceStatusServiceTests
{
    private IDeviceManagementRepository _deviceManagementRepository;
    private UpdateDeviceStatusService _service;

    [SetUp]
    public void SetUp()
    {
        _deviceManagementRepository = Substitute.For<IDeviceManagementRepository>();
        _service = new UpdateDeviceStatusService(_deviceManagementRepository);
    }

    [Test]
    public async Task Handle_ShouldReturnSuccess_WhenNoStationsUpdatesProvided()
    {
        // Arrange
        var request = new UpdateDeviceStatusRequest
        {
            StationsUpdates = new List<Contract.DeviceManagementModule.UpdateDeviceStatus>()
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeTrue();
        await _deviceManagementRepository.DidNotReceiveWithAnyArgs()
            .BulkUpdateDevicesStatusAsync(default, default);
    }

    [Test]
    public async Task Handle_ShouldCallBulkUpdateDevicesStatusAsync_ForEachGroup()
    {
        // Arrange
        var request = new UpdateDeviceStatusRequest
        {
            StationsUpdates = new List<Contract.DeviceManagementModule.UpdateDeviceStatus>
            {
                new() { DeviceId = 1, SetActive = true },
                new() { DeviceId = 2, SetActive = false },
                new() { DeviceId = 3, SetActive = true }
            }
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        await _deviceManagementRepository.Received(1)
            .BulkUpdateDevicesStatusAsync(Arg.Is<int[]>(ids => ids.SequenceEqual(new[] { 1, 3 })), true);
        await _deviceManagementRepository.Received(1)
            .BulkUpdateDevicesStatusAsync(Arg.Is<int[]>(ids => ids.SequenceEqual(new[] { 2 })), false);
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldNotCallBulkUpdateDevicesStatusAsync_WhenStationsUpdatesIsEmpty()
    {
        // Arrange
        var request = new UpdateDeviceStatusRequest
        {
            StationsUpdates = new List<Contract.DeviceManagementModule.UpdateDeviceStatus>()
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        await _deviceManagementRepository.DidNotReceiveWithAnyArgs().BulkUpdateDevicesStatusAsync(default, default);
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public async Task Handle_ShouldHandleMultipleGroupsCorrectly()
    {
        // Arrange
        var request = new UpdateDeviceStatusRequest
        {
            StationsUpdates = new List<Contract.DeviceManagementModule.UpdateDeviceStatus>
            {
                new() { DeviceId = 1, SetActive = true },
                new() { DeviceId = 2, SetActive = true },
                new() { DeviceId = 3, SetActive = false },
                new() { DeviceId = 4, SetActive = false }
            }
        };

        // Act
        var result = await _service.Handle(request);

        // Assert
        await _deviceManagementRepository.Received(1)
            .BulkUpdateDevicesStatusAsync(Arg.Is<int[]>(ids => ids.SequenceEqual(new[] { 1, 2 })), true);
        await _deviceManagementRepository.Received(1)
            .BulkUpdateDevicesStatusAsync(Arg.Is<int[]>(ids => ids.SequenceEqual(new[] { 3, 4 })), false);
        result.Should().BeOfType<Result>();
        result.IsSuccess.Should().BeTrue();
    }

    [Test]
    public void Handle_ShouldThrowException_WhenBulkUpdateDevicesStatusAsyncFails()
    {
        // Arrange
        var request = new UpdateDeviceStatusRequest
        {
            StationsUpdates = new List<Contract.DeviceManagementModule.UpdateDeviceStatus>
            {
                new() { DeviceId = 1, SetActive = true }
            }
        };

        _deviceManagementRepository.BulkUpdateDevicesStatusAsync(Arg.Any<int[]>(), Arg.Any<bool>())
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(request);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}
