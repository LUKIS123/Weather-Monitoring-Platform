using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitor.Server.SharedKernel;
using WeatherMonitor.Server.StationsPermissions.Features.ListAvailableStations;
using WeatherMonitor.Server.StationsPermissions.Infrastructure;
using WeatherMonitor.Server.StationsPermissions.Infrastructure.Models;

namespace WeatherMonitor.Server.StationsPermissions.Tests.ListAvailableStations;

[TestFixture]
public class ListAvailableStationsServiceTests
{
    private IStationsPermissionsRepository _stationsPermissionsRepository = null!;
    private ListAvailableStationsService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _stationsPermissionsRepository = Substitute.For<IStationsPermissionsRepository>();
        _service = new ListAvailableStationsService(_stationsPermissionsRepository);
    }

    [Test]
    public async Task Handle_ShouldReturnPageResult_WithCorrectStationsAndMetadata()
    {
        // Arrange
        var pageNumber = 1;
        var stations = new List<AvailableStation>
        {
            new() { Id = 1, DeviceName = "Station1" },
            new() { Id = 2, DeviceName = "Station2" }
        };

        var itemCount = 20;

        _stationsPermissionsRepository.GetAvailableStationsAsync(pageNumber, 10).Returns((stations, itemCount));

        // Act
        var result = await _service.Handle(pageNumber);

        // Assert
        result.Value.Should().BeOfType<PageResult<AvailableStation>>();
        var pageResult = result.Value.As<PageResult<AvailableStation>>();
        pageResult.Items.Should().BeEquivalentTo(stations);
        pageResult.TotalItemsCount.Should().Be(itemCount);
        pageResult.PageSize.Should().Be(10);
        pageResult.TotalPages.Should().Be(2);
    }

    [Test]
    public async Task Handle_ShouldCallGetAvailableStationsAsync_WithCorrectParameters()
    {
        // Arrange
        var pageNumber = 2;

        _stationsPermissionsRepository.GetAvailableStationsAsync(pageNumber, 10)
            .Returns((new List<AvailableStation>(), 0));

        // Act
        await _service.Handle(pageNumber);

        // Assert
        await _stationsPermissionsRepository.Received(1).GetAvailableStationsAsync(pageNumber, 10);
    }

    [Test]
    public void Handle_ShouldThrowException_WhenRepositoryCallFails()
    {
        // Arrange
        var pageNumber = 1;

        _stationsPermissionsRepository.GetAvailableStationsAsync(pageNumber, 10)
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(pageNumber);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyPageResult_WhenNoStationsAreAvailable()
    {
        // Arrange
        var pageNumber = 1;

        _stationsPermissionsRepository.GetAvailableStationsAsync(pageNumber, 10)
            .Returns((new List<AvailableStation>(), 0));

        // Act
        var result = await _service.Handle(pageNumber);

        // Assert
        result.Value.Should().BeOfType<PageResult<AvailableStation>>();
        var pageResult = result.Value.As<PageResult<AvailableStation>>();
        pageResult.Items.Should().BeEmpty();
        pageResult.TotalItemsCount.Should().Be(0);
        pageResult.PageSize.Should().Be(10);
        pageResult.TotalPages.Should().Be(0);
    }
}