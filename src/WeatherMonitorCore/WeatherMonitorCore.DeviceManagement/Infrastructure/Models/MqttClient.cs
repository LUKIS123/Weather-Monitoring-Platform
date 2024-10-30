namespace WeatherMonitorCore.DeviceManagement.Infrastructure.Models;
internal class MqttClient
{
    public Guid Id { get; set; }
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string ClientId { get; set; } = null!;
    public bool IsSuperUser { get; set; }
}
