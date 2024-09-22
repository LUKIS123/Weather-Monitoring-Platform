using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json.Serialization;

namespace WeatherMonitorCore.Controllers;

[ApiController]
[Route("api/mqttauth")]
[AllowAnonymous]
public class MqttAuthController : ControllerBase
{
    [HttpPost("user")]
    public IActionResult AuthenticateUser([FromBody] AuthenticationRequest authenticationRequest)
    {
        if (authenticationRequest.Username.StartsWith("mqtt_user") && authenticationRequest.Password.StartsWith("secure_password"))
        {
            return Ok();
        }
        return Forbid();
    }

    [HttpPost("superuser")]
    public IActionResult CheckSuperuser([FromBody] SuperUserCheckRequest superUserCheckRequest)
    {
        if (superUserCheckRequest.Username.StartsWith("mqtt_user"))
        {
            return Ok();
        }
        return Forbid();
    }

    [HttpPost("acl")]
    public IActionResult CheckAcl([FromBody] AclCheckRequest aclCheckRequest)
    {
        // acc could be publish/subscribe, depending on your setup
        if (aclCheckRequest.Username.StartsWith("mqtt_user") && aclCheckRequest.Topic.StartsWith("allowed/topic"))
        {
            return Ok();
        }
        return Forbid();
    }
}

// /auth/user: This endpoint verifies if the user exists and returns a success/failure status.
// /auth/superuser: This checks if the user has superuser privileges.
// /auth/acl: This checks the user's permissions (e.g., topic access).
//  acc = 1 is read, 2 is write, 3 is readwrite, 4 is subscribe

public class AuthenticationRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
    [JsonPropertyName("password")]
    public required string Password { get; set; }
    [JsonPropertyName("clientid")]
    public required string ClientId { get; set; }
}

public class SuperUserCheckRequest
{
    [JsonPropertyName("username")]
    public required string Username { get; set; }
}

public class AclCheckRequest
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

public enum ActionType
{
    Read = 1,
    Write = 2,
    ReadWrite = 3,
    Subscribe = 4
}
