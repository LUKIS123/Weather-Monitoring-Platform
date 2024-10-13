namespace WeatherMonitor.Server.SharedKernel.Exceptions;

public class MicroserviceApiException : Exception
{
    public MicroserviceApiException()
    {
    }

    public MicroserviceApiException(string? message) : base(message)
    {
    }
}