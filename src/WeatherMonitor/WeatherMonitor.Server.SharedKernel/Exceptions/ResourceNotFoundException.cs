namespace WeatherMonitor.Server.SharedKernel.Exceptions;
public class ResourceNotFoundException : Exception
{
    public ResourceNotFoundException()
    {
    }

    public ResourceNotFoundException(string? message) : base(message)
    {
    }
}
