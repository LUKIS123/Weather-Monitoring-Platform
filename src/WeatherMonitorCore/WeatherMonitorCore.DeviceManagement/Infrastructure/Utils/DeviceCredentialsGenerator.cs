namespace WeatherMonitorCore.DeviceManagement.Infrastructure.Utils;

internal interface IDeviceCredentialsGenerator
{
    GeneratedDeviceCredentials GenerateDeviceCredentials(string username, Guid mqttTopicGuid);
}

internal class DeviceCredentialsGenerator : IDeviceCredentialsGenerator
{
    public GeneratedDeviceCredentials GenerateDeviceCredentials(string username, Guid mqttTopicGuid)
    {
        return new GeneratedDeviceCredentials(
            $"{username.ToUpperInvariant()}-{Guid.NewGuid().ToString().ToUpper()}",
            $"weather/{mqttTopicGuid.ToString().ToUpper()}"
        );
    }
}
