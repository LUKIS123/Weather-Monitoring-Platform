using WeatherMonitorCore.UserAuthorization.Infrastructure.Models;

namespace WeatherMonitorCore.UserAuthorization.Infrastructure;

public interface IUserAuthorizationRepository
{
    Task<BasicUserAuthorizationDto?> GetUserAuthorizationInfoAsync(string userId);
}
