using System.Text.Json.Serialization;

namespace WeatherMonitorCore.MqttAuth.Features.SuperUserCheck;
internal class SuperUserCheckRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}
