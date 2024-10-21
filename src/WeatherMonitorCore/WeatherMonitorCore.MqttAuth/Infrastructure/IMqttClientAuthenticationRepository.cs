using WeatherMonitorCore.MqttAuth.Infrastructure.Models;

namespace WeatherMonitorCore.MqttAuth.Infrastructure;
public interface IMqttClientAuthenticationRepository
{
    Task<AclCheckDto> GetMqttAclAsync(string username, string clientId);
    Task<BrokerClientAuthenticationDto> GetMqttCredentialsAsync(string username, string clientId);
    Task<SuperUserCheckDto> GetMqttSuperuserAsync(string username);
}
