using WeatherMonitor.Server.UserAuthorization.Infrastructure.Models;

namespace WeatherMonitor.Server.UserAuthorization.Infrastructure;
public interface IUserAuthorizationRepository
{
    Task<BasicUserAuthorizationDto?> GetUserAuthorizationInfoAsync(string userId);
}
