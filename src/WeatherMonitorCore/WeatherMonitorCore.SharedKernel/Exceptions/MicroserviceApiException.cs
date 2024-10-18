namespace WeatherMonitorCore.SharedKernel.Exceptions;
internal class MicroserviceApiException : Exception
{
    public MicroserviceApiException()
    {
    }

    public MicroserviceApiException(string? message) : base(message)
    {
    }
}
