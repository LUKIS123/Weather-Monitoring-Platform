using System.Data;

namespace WeatherMonitor.Server.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection GetOpenConnection();
    Task<IDbConnection> GetOpenConnectionAsync();
}
