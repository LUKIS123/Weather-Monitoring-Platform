using System.Data;

namespace WeatherMonitorCore.Interfaces;

public interface IDbConnectionFactory
{
    IDbConnection GetOpenConnection();
    Task<IDbConnection> GetOpenConnectionAsync();
}