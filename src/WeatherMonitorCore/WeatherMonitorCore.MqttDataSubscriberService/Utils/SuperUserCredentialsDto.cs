namespace WeatherMonitorCore.MqttDataSubscriberService.Utils;
internal class SuperUserCredentialsDto
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public required string Username { get; set; }
    public required string Password { get; set; }
    public required string ClientId { get; set; }
    public bool IsSuperUser { get; set; } = true;
}
