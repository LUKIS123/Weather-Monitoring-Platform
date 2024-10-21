namespace WeatherMonitorCore.SharedKernel.Exceptions;
public class ForbidException : Exception
{
    public ForbidException()
    {
    }

    public ForbidException(string? message) : base(message)
    {
    }
}
