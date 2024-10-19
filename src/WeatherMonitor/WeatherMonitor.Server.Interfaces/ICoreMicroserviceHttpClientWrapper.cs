namespace WeatherMonitor.Server.Interfaces;

public interface ICoreMicroserviceHttpClientWrapper
{
    Task<(TResponse?, bool Success, string Message)> GetHttpRequest<TResponse>(string url, string? token = null);

    Task<(TResponse?, bool Success, string Message)> PostHttpRequest<TRequest, TResponse>(string url, TRequest payload, string? token = null);

    Task<(bool Success, string Message)> PostHttpRequest<TRequest>(string url, TRequest payload, string? token = null);

    Task<(bool Success, string Message)> DeleteHttpRequest(string url, string? token = null);

    Task<(TResponse?, bool Success, string Message)> PatchHttpRequest<TRequest, TResponse>(string url, TRequest payload, string? token = null);
}
