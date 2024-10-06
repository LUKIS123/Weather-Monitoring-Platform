namespace WeatherMonitorCore.Interfaces
{
    public interface IUserAccessor
    {
        string? UserId { get; }
        string? UserName { get; }
        string? PhotoUrl { get; }
    }
}
