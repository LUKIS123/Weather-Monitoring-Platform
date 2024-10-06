

using WeatherMonitorCore.UserAuthorization.Infrastructure;
using WeatherMonitorCore.UserAuthorization.Infrastructure.Models;

namespace WeatherMonitorCore.Infrastructure.Repositories;

internal class SqlUserRepository : IUserAuthenticationRepository
{
    public Task<BasicUserAuthorizationDto> GetUserAuthorizationInfoAsync(string userId)
    {
        throw new NotImplementedException();
    }
}