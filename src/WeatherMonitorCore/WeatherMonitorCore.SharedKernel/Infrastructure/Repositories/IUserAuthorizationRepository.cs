using WeatherMonitorCore.SharedKernel.Infrastructure.Models;

namespace WeatherMonitorCore.SharedKernel.Infrastructure.Repositories;
public interface IUserAuthorizationRepository
{
    Task<BasicUserAuthorizationDto?> GetUserAuthorizationInfoAsync(string userId);
}
