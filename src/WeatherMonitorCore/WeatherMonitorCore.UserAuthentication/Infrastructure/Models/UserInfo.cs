namespace WeatherMonitorCore.UserAuthentication.Infrastructure.Models
{
    internal class UserInfo
    {
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string PhotoUrl { get; set; }
    }
}
