using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using WeatherMonitor.Server.UserAuthentication.Features.Authentication;
using WeatherMonitor.Server.UserAuthentication.Features.SignIn;
using WeatherMonitor.Server.UserAuthentication.Features.UserSettings;

namespace WeatherMonitor.Server.UserAuthentication;

public static class UserAuthenticationModule
{
    private const string JwtKey = "Jwt:Key";
    private const string Auth0Domain = "Auth0:Domain";
    private const string Auth0Audience = "Auth0:Audience";

    public static void AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        var encryptionKey = configuration[JwtKey] ?? throw new ArgumentNullException(nameof(configuration), JwtKey);

        services.AddTransient<IExternalSignInService, ExternalSignInService>();
        services.AddTransient<IAuthService, AuthService>();
        services.AddTransient<IGetUserSettingsService, GetUserSettingsService>();

        // services.AddAuthentication(x =>
        //     {
        //         x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        //         x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        //         x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        //     })
        //     .AddJwtBearer(x =>
        //     {
        //         x.RequireHttpsMetadata = false;
        //         x.SaveToken = true;
        //         x.TokenValidationParameters = new TokenValidationParameters
        //         {
        //             ValidateIssuerSigningKey = true,
        //             IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey)),
        //             ValidateIssuer = false,
        //             ValidateAudience = false,
        //         };
        //
        //         x.Events = new JwtBearerEvents
        //         {
        //             OnMessageReceived = context =>
        //             {
        //                 context.Token = context.Request.Cookies["AuthToken"];
        //                 return Task.CompletedTask;
        //             }
        //         };
        //     });


        var authDomain = configuration[Auth0Domain] ?? throw new ArgumentNullException(nameof(configuration), Auth0Domain);
        var authAudience = configuration[Auth0Audience] ?? throw new ArgumentNullException(nameof(configuration), Auth0Audience);

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Authority = authDomain;
                options.Audience = authAudience;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = ClaimTypes.NameIdentifier
                };
            });
    }
}