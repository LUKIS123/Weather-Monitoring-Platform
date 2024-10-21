using System.Text.Json.Serialization;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitorCore.MqttAuth.Features.AclCheck;
internal class AclCheckRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    [JsonPropertyName("clientid")]
    public required string ClientId { get; set; }
    [JsonPropertyName("topic")]
    public required string Topic { get; set; }
    [JsonPropertyName("acc")]
    public ActionType Action { get; set; }
}
