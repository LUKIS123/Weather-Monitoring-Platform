namespace WeatherMonitorCore.SharedKernel.Exceptions;
internal class UnauthorizedException : Exception
{
    public UnauthorizedException()
    {
    }

    public UnauthorizedException(string? message) : base(message)
    {
    }
}
