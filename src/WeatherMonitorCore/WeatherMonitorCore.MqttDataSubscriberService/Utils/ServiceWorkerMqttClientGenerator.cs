using PasswordGenerator;
using WeatherMonitorCore.MqttDataSubscriberService.Configuration;

namespace WeatherMonitorCore.MqttDataSubscriberService.Utils;

internal interface IServiceWorkerMqttClientGenerator
{
    SuperUserCredentialsDto GenerateSuperUserCredentials();
}

internal class ServiceWorkerMqttClientGenerator : IServiceWorkerMqttClientGenerator
{
    private readonly IPassword _passwordGenerator;
    private const int PasswordLength = 16;
    private const string SuperUserNameSuffix = "WORKER";
    private const string SuperUserClientIdSuffix = "WORKER";

    public ServiceWorkerMqttClientGenerator()
    {
        _passwordGenerator = new Password()
            .IncludeLowercase()
            .IncludeUppercase()
            .IncludeNumeric()
            .LengthRequired(PasswordLength);
    }

    public SuperUserCredentialsDto GenerateSuperUserCredentials()
    {
        return new SuperUserCredentialsDto
        {
            Id = WorkerMqttClientConfig.MqttDataServiceGuid,
            Username = $"{SuperUserNameSuffix.ToUpperInvariant()}-{Guid.NewGuid().ToString().ToUpper()}",
            Password = _passwordGenerator.Next(),
            ClientId = $"{SuperUserClientIdSuffix.ToUpperInvariant()}-{Guid.NewGuid().ToString().ToUpper()}",
        };
    }
}
