using System.Diagnostics;

namespace WeatherMonitor.Server.Middleware;

internal class RequestTimeMiddleware : IMiddleware
{
    private readonly Stopwatch _stopwatch;
    private readonly ILogger<RequestTimeMiddleware> _logger;

    private const long WarningThreshold = 5000;

    public RequestTimeMiddleware(ILogger<RequestTimeMiddleware> logger)
    {
        _stopwatch = new Stopwatch();
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        _stopwatch.Start();
        await next.Invoke(context);
        _stopwatch.Stop();

        if (_stopwatch.ElapsedMilliseconds > WarningThreshold)
        {
            _logger.LogInformation(
                "Request [{Method}] at {Path} took {ElapsedMilliseconds} ms",
                context.Request.Method,
                context.Request.Path,
                _stopwatch.ElapsedMilliseconds);
        }

        _stopwatch.Reset();
    }
}
