using System.Text.Json.Serialization;

namespace WeatherMonitorCore.MqttAuth.Features.BrokerClientAuthentication;
internal class AuthenticationRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
    [JsonPropertyName("clientid")]
    public required string ClientId { get; set; }
}
