using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using WeatherMonitorCore.UserAuthentication.Features.Authentication;
using WeatherMonitorCore.UserAuthentication.Features.SignIn;

namespace WeatherMonitorCore.UserAuthentication;

public static class UserAuthenticationEndpoints
{
    public static void RegisterUserEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/api/user/google-sign-in",
            async (HttpContext context, IExternalSignInService externalSignInService) =>
            {
                var tokenResponse = await externalSignInService.Handle(
                    context.Request.Headers.Authorization.ToString().Replace("Bearer ", "")
                );

                if (!string.IsNullOrEmpty(tokenResponse.Token))
                {
                    context.Response.Cookies.Append("AuthToken", tokenResponse.Token, new CookieOptions
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
            });

        routes.MapPost(
            "/api/user/logout",
            (HttpContext context) => { context.Response.Cookies.Delete("AuthToken"); });

        routes.MapGet(
            "/api/user/user-info",
            async (HttpContext context, IAuthenticationService service) =>
            {
                var userInfo = service.Handle();

                await context.Response.WriteAsJsonAsync(new
                {
                    isAuthorized = !string.IsNullOrEmpty(userInfo.UserId),
                    userId = userInfo.UserId,
                    userName = userInfo.UserName,
                    userPhotoUrl = userInfo.PhotoUrl
                });
            });

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
            });

        routes.MapGet(
            "api/user/dark-theme-status",
            async (HttpContext context) =>
            {
                var darkTheme = context.Request.Cookies["DarkTheme"];
                await context.Response.WriteAsJsonAsync(darkTheme == "True");
            });
    }
}
