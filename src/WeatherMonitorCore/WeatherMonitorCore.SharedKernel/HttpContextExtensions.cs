using Microsoft.AspNetCore.Http;
using WeatherMonitorCore.SharedKernel.Exceptions;

namespace WeatherMonitorCore.SharedKernel;
public static class HttpContextExtensions
{
    public static async Task HandleResult(this HttpContext httpContext, Result result)
    {
        if (result.IsSuccess)
        {
            return;
        }

        var exception = result.Error?.Exception;
        httpContext.Response.StatusCode = exception switch
        {
            BadHttpRequestException => StatusCodes.Status400BadRequest,
            MicroserviceApiException => StatusCodes.Status400BadRequest,
            UnauthorizedException => StatusCodes.Status401Unauthorized,
            NotFoundException => StatusCodes.Status404NotFound,
            ForbidException => StatusCodes.Status403Forbidden,
            _ => StatusCodes.Status500InternalServerError
        };

        await httpContext.Response.WriteAsJsonAsync(new { message = exception?.Message });
    }

    public static async Task HandleResult<T>(this HttpContext httpContext, Result<T> result)
    {
        if (result.IsSuccess)
        {
            await httpContext.Response.WriteAsJsonAsync(result.Value);
            return;
        }

        await httpContext.HandleResult((Result)result);
    }
}
