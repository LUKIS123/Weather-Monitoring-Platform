namespace WeatherMonitorCore.Contract.Auth;

public class AuthenticateRequest
{
    public required string IdToken { get; set; }
}