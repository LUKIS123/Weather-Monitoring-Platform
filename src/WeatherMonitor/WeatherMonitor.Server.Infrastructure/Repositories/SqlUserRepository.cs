using Dapper;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Models;
using WeatherMonitor.Server.SharedKernel.Repositories;

namespace WeatherMonitor.Server.Infrastructure.Repositories;
internal class SqlUserRepository : IUserAuthorizationRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqlUserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<BasicUserAuthorizationDto?> GetUserAuthorizationInfoAsync(string userId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<BasicUserAuthorizationDto>(@$"
SELECT TOP 1
    Id AS {nameof(BasicUserAuthorizationDto.Id)},
    Role AS {nameof(BasicUserAuthorizationDto.Role)}
FROM [identity].[Users]
WHERE Id = @userId
", new { userId });

        return result;
    }
}
