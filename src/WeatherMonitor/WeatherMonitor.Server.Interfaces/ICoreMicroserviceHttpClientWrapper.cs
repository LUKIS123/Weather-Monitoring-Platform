namespace WeatherMonitor.Server.Interfaces;

public interface ICoreMicroserviceHttpClientWrapper
{
    Task<(TResponse?, bool Success, string Message)> GetHttpRequest<TResponse>(string token, string url);

    Task<(TResponse?, bool Success, string Message)> PostHttpRequest<TRequest, TResponse>(string token, string url, TRequest payload);

    Task<(bool Success, string Message)> PostHttpRequest<TRequest>(string userId, string refreshToken, string url, TRequest payload);

    Task<(TResponse?, bool Success, string Message)> PatchHttpRequest<TRequest, TResponse>(string token, string url, TRequest payload);
}