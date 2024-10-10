namespace WeatherMonitorCore.Contract.UserAuthenticationModule;

public class AuthenticateRequest
{
    public required string IdToken { get; set; }
}