using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherMonitor.Server.UserAuthentication.Features.Authentication;
using WeatherMonitor.Server.UserAuthentication.Features.SignIn;

namespace WeatherMonitor.Server.UserAuthentication;
public static class UserAuthenticationModule
{
    private const string JwtKey = "Jwt:Key";

    public static void AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        var encryptionKey = configuration[JwtKey] ?? throw new ArgumentNullException(nameof(configuration), JwtKey);

        services.AddTransient<IExternalSignInService, ExternalSignInService>();
        services.AddTransient<IAuthService, AuthService>();

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey)),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                };

                x.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        context.Token = context.Request.Cookies["AuthToken"];
                        return Task.CompletedTask;
                    }
                };
            });
    }
}
