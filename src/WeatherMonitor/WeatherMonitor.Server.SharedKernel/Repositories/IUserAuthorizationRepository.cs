using WeatherMonitor.Server.SharedKernel.Models;

namespace WeatherMonitor.Server.SharedKernel.Repositories;
public interface IUserAuthorizationRepository
{
    Task<BasicUserAuthorizationDto?> GetUserAuthorizationInfoAsync(string userId);
}
