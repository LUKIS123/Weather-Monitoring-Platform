using WeatherMonitorCore.UserAuthorization.Infrastructure.Models;

namespace WeatherMonitorCore.UserAuthorization.Infrastructure;

public interface IUserAuthenticationRepository
{
    Task<BasicUserAuthorizationDto> GetUserAuthorizationInfoAsync(string userId);
}