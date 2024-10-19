using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;
using WeatherMonitor.Server.UserAuthentication.Features.Authentication;
using WeatherMonitor.Server.UserAuthentication.Features.SignIn;
using WeatherMonitor.Server.UserAuthentication.Features.UserSettings;

namespace WeatherMonitor.Server.UserAuthentication;
public static class UserAuthenticationEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/api/user/google-sign-in",
            async (HttpContext context, [FromServices] IExternalSignInService externalSignInService) =>
            {
                var result = await externalSignInService.Handle(
                    context.Request.Headers.Authorization.ToString().Replace("Bearer ", ""));

                if (result is { IsSuccess: true, Value.Token: not null })
                {
                    context.Response.Cookies.Append("AuthToken", result.Value.Token, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict
                    });
                }
                else
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                }
                context.Response.Headers.Append("Cross-Origin-Embedder-Policy", "unsafe-none");
                context.Response.Headers.Append("Cross-Origin-Opener-Policy", "same-origin-allow-popups");
                context.Response.Headers.Append(
                    "Content-Security-Policy",
                    "default-src 'self'; script-src 'self' https://accounts.google.com https://apis.google.com; connect-src 'self' https://accounts.google.com");
            }).AllowAnonymous();

        routes.MapPost(
            "/api/user/logout",
            (HttpContext context) => { context.Response.Cookies.Delete("AuthToken"); })
            .RequireAuthorization();

        routes.MapGet(
            "/api/user/user-info",
            async (HttpContext context, [FromServices] IAuthService service) =>
            {
                var userInfo = service.Handle();

                await context.Response.WriteAsJsonAsync(new
                {
                    isAuthorized = !string.IsNullOrEmpty(userInfo.UserId),
                    userId = userInfo.UserId,
                    userName = userInfo.UserName,
                    userPhotoUrl = userInfo.PhotoUrl,
                    email = userInfo.Email,
                    role = userInfo.Role
                });
            }).AllowAnonymous();

        routes.MapGet(
            "/api/user/user-settings",
            async (HttpContext context, [FromServices] IGetUserSettingsService service) =>
            {
                var result = await service.Handle();
                await context.HandleResult(result);
            }).RequireAuthorization();

        routes.MapPost(
            "api/user/set-dark-theme",
            async (HttpContext context) =>
            {
                var darkTheme = await context.Request.ReadFromJsonAsync<bool>();
                context.Response.Cookies.Append("DarkTheme", darkTheme.ToString(), new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });
            }).AllowAnonymous();

        routes.MapGet(
            "api/user/dark-theme-status",
            async (HttpContext context) =>
            {
                var darkTheme = context.Request.Cookies["DarkTheme"];
                await context.Response.WriteAsJsonAsync(darkTheme == "True");
            }).AllowAnonymous();
    }
}
