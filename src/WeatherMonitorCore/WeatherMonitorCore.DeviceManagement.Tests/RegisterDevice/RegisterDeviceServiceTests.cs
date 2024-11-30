using FluentAssertions;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using WeatherMonitorCore.Contract.DeviceManagementModule;
using WeatherMonitorCore.DeviceManagement.Features.RegisterDevice;
using WeatherMonitorCore.DeviceManagement.Infrastructure;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
using WeatherMonitorCore.DeviceManagement.Infrastructure.Utils;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.Shared.MqttClient.Interfaces;

namespace WeatherMonitorCore.DeviceManagement.Tests.RegisterDevice;

[TestFixture]
public class RegisterDeviceServiceTests
{
    private IDeviceManagementRepository _deviceManagementRepository = null!;
    private IPasswordGeneratorService _passwordGeneratorService = null!;
    private IDeviceCredentialsGenerator _credentialsGenerator = null!;
    private ISubscriptionsManagingService _subscriptionsManagingService = null!;
    private IAesEncryptionHelper _aesEncryptionHelper = null!;
    private RegisterDeviceService _service = null!;

    [SetUp]
    public void SetUp()
    {
        _deviceManagementRepository = Substitute.For<IDeviceManagementRepository>();
        _passwordGeneratorService = Substitute.For<IPasswordGeneratorService>();
        _credentialsGenerator = Substitute.For<IDeviceCredentialsGenerator>();
        _subscriptionsManagingService = Substitute.For<ISubscriptionsManagingService>();
        _aesEncryptionHelper = Substitute.For<IAesEncryptionHelper>();
        _service = new RegisterDeviceService(
            _deviceManagementRepository,
            _passwordGeneratorService,
            _credentialsGenerator,
            _subscriptionsManagingService,
            _aesEncryptionHelper
        );
    }

    [TearDown]
    public void TearDown()
    {
        _subscriptionsManagingService.Dispose();
    }

    [Test]
    public async Task Handle_ShouldReturnCreateDeviceResponse_WhenRequestIsValid()
    {
        // Arrange
        var request = new RegisterDeviceRequest
        {
            GoogleMapsPlusCode = "1234+56 Example",
            DeviceExtraInfo = "TestDevice",
            MqttUsername = "test-user"
        };
        var plainTextPassword = "plaintext-password";
        var encryptedPassword = "encrypted-password";
        var generatedClientId = "generated-client-id";
        var generatedTopic = "generated-topic";

        var credentials = new GeneratedDeviceCredentials
        {
            ClientId = generatedClientId,
            Topic = generatedTopic
        };

        _passwordGeneratorService.GeneratePassword().Returns(plainTextPassword);
        _aesEncryptionHelper.Encrypt(plainTextPassword).Returns(encryptedPassword);
        _credentialsGenerator.GenerateDeviceCredentials(request.MqttUsername, Arg.Any<Guid>()).Returns(credentials);
        _deviceManagementRepository.RegisterDeviceAsync(Arg.Any<RegisterDeviceDto>()).Returns(1);
        _subscriptionsManagingService.AddTopicAsync(generatedTopic, Arg.Any<CancellationToken>()).Returns(Task.CompletedTask);

        // Act
        var result = (await _service.Handle(request)).Value;

        // Assert
        result.Should().BeOfType<CreateDeviceResponse>();
        var response = result.As<CreateDeviceResponse>();
        response.Id.Should().Be(1);
        response.GoogleMapsPlusCode.Should().Be(request.GoogleMapsPlusCode);
        response.DeviceExtraInfo.Should().Be(request.DeviceExtraInfo);
        response.IsActivate.Should().BeFalse();
        response.Username.Should().Be(request.MqttUsername);
        response.Password.Should().Be(plainTextPassword);
        response.ClientId.Should().Be(generatedClientId);
        response.Topic.Should().Be(generatedTopic);
    }

    [Test]
    public async Task Handle_ShouldCallDependenciesCorrectly()
    {
        // Arrange
        var request = new RegisterDeviceRequest
        {
            GoogleMapsPlusCode = "1234+56 Example",
            DeviceExtraInfo = "TestDevice",
            MqttUsername = "test-user"
        };
        var plainTextPassword = "plaintext-password";
        var encryptedPassword = "encrypted-password";

        _passwordGeneratorService.GeneratePassword().Returns(plainTextPassword);
        _aesEncryptionHelper.Encrypt(plainTextPassword).Returns(encryptedPassword);

        // Act
        await _service.Handle(request);

        // Assert
        _passwordGeneratorService.Received(1).GeneratePassword();
        _aesEncryptionHelper.Received(1).Encrypt(plainTextPassword);
        await _deviceManagementRepository.Received(1).RegisterDeviceAsync(Arg.Any<RegisterDeviceDto>());
        await _subscriptionsManagingService.Received(1).AddTopicAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
    }

    [Test]
    public void Handle_ShouldThrowException_WhenDependenciesFail()
    {
        // Arrange
        var request = new RegisterDeviceRequest
        {
            GoogleMapsPlusCode = "1234+56 Example",
            DeviceExtraInfo = "TestDevice",
            MqttUsername = "test-user"
        };

        _deviceManagementRepository.RegisterDeviceAsync(Arg.Any<RegisterDeviceDto>())
            .Throws(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.Handle(request);

        // Assert
        act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Test]
    public async Task Handle_ShouldUseDefaultPlusCode_WhenGoogleMapsPlusCodeIsNullOrEmpty()
    {
        // Arrange
        var request = new RegisterDeviceRequest
        {
            GoogleMapsPlusCode = null,
            DeviceExtraInfo = "TestDevice",
            MqttUsername = "test-user"
        };

        _passwordGeneratorService.GeneratePassword().Returns("plaintext-password");
        _aesEncryptionHelper.Encrypt(Arg.Any<string>()).Returns("encrypted-password");

        // Act
        var result = (await _service.Handle(request)).Value;

        // Assert
        result.Should().BeOfType<CreateDeviceResponse>();
        result.As<CreateDeviceResponse>().GoogleMapsPlusCode.Should().Be("4356+2Q Wrocław");
    }
}