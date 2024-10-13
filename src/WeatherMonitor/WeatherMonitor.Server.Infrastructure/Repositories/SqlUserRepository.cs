using WeatherMonitor.Server.UserAuthorization.Infrastructure;
using WeatherMonitor.Server.UserAuthorization.Infrastructure.Models;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class SqlUserRepository : IUserAuthorizationRepository
{
    public Task<BasicUserAuthorizationDto?> GetUserAuthorizationInfoAsync(string userId)
    {
        throw new NotImplementedException();
    }
}
