using Dapper;
using WeatherMonitorCore.Contract.Shared;
using WeatherMonitorCore.Interfaces;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Repositories;

namespace WeatherMonitorCore.Infrastructure.Repositories;

internal class SqlUserRepository : IUserSettingsRepository
{
    private readonly IDbConnectionFactory _dbConnectionFactory;

    public SqlUserRepository(IDbConnectionFactory dbConnectionFactory)
    {
        _dbConnectionFactory = dbConnectionFactory;
    }

    public async Task<UserSettingsDto?> GetUser(string userId)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var result = await connection.QueryFirstOrDefaultAsync<UserSettingsDto>(@$"
SELECT TOP 1
    Id AS {nameof(UserSettingsDto.UserId)},
    Role AS {nameof(UserSettingsDto.Role)}
FROM [identity].[Users]
WHERE Id = @userId
", new { userId });

        return result;
    }

    public async Task<UserSettingsDto> GetOrCreateUser(string userId, Role role = Role.User)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        var result = await connection.QueryFirstAsync<UserSettingsDto>(@$"
DECLARE @output TABLE (
    Id NVARCHAR(255),
    Role INT
);

INSERT INTO @output (Id, Role)
SELECT TOP 1
    Id, 
    Role 
FROM [identity].[Users] 
WHERE Id = @userId;

IF NOT EXISTS (SELECT 1 FROM @output)
BEGIN
    INSERT INTO [identity].[Users] (Id, Role)
    VALUES (@userId, @role);

    INSERT INTO @output (Id, Role)
    SELECT Id, Role 
    FROM [identity].[Users] 
    WHERE Id = @userId;
END

SELECT
    Id AS {nameof(UserSettingsDto.UserId)},
    Role AS {nameof(UserSettingsDto.Role)}
FROM @output
", new { userId, role });

        return result;
    }

    public async Task SetUserRole(string userId, Role role)
    {
        using var connection = await _dbConnectionFactory.GetOpenConnectionAsync();
        await connection.ExecuteAsync(@"
UPDATE [identity].[Users]
SET Role = @role
WHERE Id = @userId;
", new { role, userId });
    }
}
