using System.Net.Http.Json;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitor.Server.SharedKernel.Utility;

namespace WeatherMonitor.Server.Infrastructure;

internal class CoreMicroserviceHttpClientWrapper : ICoreMicroserviceHttpClientWrapper
{
    private readonly IHttpClientFactory _httpClientFactory;

    public CoreMicroserviceHttpClientWrapper(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<(TResponse?, bool Success, string Message)> GetHttpRequest<TResponse>(
        string url,
        string? token = null) =>
        await SendRequest<TResponse>(
            token,
            httpClient => httpClient.GetAsync(url));

    public async Task<(TResponse?, bool Success, string Message)> PostHttpRequest<TRequest, TResponse>(
        string url,
        TRequest payload,
        string? token = null) =>
        await SendRequest<TResponse>(
            token,
            httpClient => httpClient.PostAsJsonAsync(url, payload));

    public async Task<(bool Success, string Message)> PostHttpRequest<TRequest>(
        string url,
        TRequest payload,
        string? token = null) =>
        await SendRequest(
            token,
            httpClient => httpClient.PostAsJsonAsync(url, payload));

    public async Task<(TResponse?, bool Success, string Message)> PatchHttpRequest<TRequest, TResponse>(
        string url,
        TRequest payload,
        string? token = null) =>
        await SendRequest<TResponse>(
            token,
            httpClient => httpClient.PatchAsJsonAsync(url, payload));

    private async Task<(TResponse?, bool Success, string Message)> SendRequest<TResponse>(
        string? token,
        Func<HttpClient, Task<HttpResponseMessage>> messageSender)
    {
        var (httpResponse, success, message) = await MakeHttpRequest(token, messageSender);
        if (success && httpResponse is not null)
        {
            var content = await httpResponse.Content.ReadFromJsonAsync<TResponse>();
            return (content, success, message);
        }

        return (default, success, message);
    }

    private async Task<(bool Success, string Message)> SendRequest(
        string? token,
        Func<HttpClient, Task<HttpResponseMessage>> messageSender)
    {
        var (_, success, message) = await MakeHttpRequest(token, messageSender);
        return (success, message);
    }

    private async Task<(HttpResponseMessage? httpResponse, bool Success, string Message)> MakeHttpRequest(
        string? token,
        Func<HttpClient, Task<HttpResponseMessage>> messageSender)
    {
        using var httpClient = _httpClientFactory.CreateClient(Constants.CoreMicroserviceClient);
        if (token is not null)
        {
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
        }

        var httpResponse = await messageSender(httpClient);
        var (success, message) = await EnsureRequestSuccessful(httpResponse);
        return (httpResponse, success, message);
    }

    private static async Task<(bool Success, string Message)> EnsureRequestSuccessful(HttpResponseMessage? httpResponse)
    {
        if (httpResponse is null)
        {
            return (false, "Response is null");
        }

        if (!httpResponse.IsSuccessStatusCode)
        {
            var errorContent = await httpResponse.Content.ReadAsStringAsync();
            return (false, $"Request failed with status code {httpResponse.StatusCode}. Error: {errorContent}");
        }

        return (true, string.Empty);
    }
}
