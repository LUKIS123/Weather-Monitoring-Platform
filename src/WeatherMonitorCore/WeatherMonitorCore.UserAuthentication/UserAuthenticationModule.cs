using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using WeatherMonitorCore.UserAuthentication.Features.Authentication;
using WeatherMonitorCore.UserAuthentication.Features.SignIn;
using WeatherMonitorCore.UserAuthentication.Features.UpdateRole;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Jwt;

namespace WeatherMonitorCore.UserAuthentication;

public static class UserAuthenticationModule
{
    private const string JwtKey = "Jwt:Key";

    public static void AddUserModule(this IServiceCollection services, IConfiguration configuration)
    {
        var encryptionKey = configuration[JwtKey] ?? throw new ArgumentNullException(nameof(configuration), JwtKey);

        services.AddTransient<IExternalSignInService, GoogleSignInService>();
        services.AddTransient<IJwtAuthorizationService, JwtAuthorizationService>();
        services.AddTransient<IAuthenticationService, AuthenticationService>();
        services.AddTransient<IUpdateRoleService, UpdateRoleService>();

        services.AddAuthentication(opt =>
        {
            opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(cfg =>
        {
            cfg.RequireHttpsMetadata = false;
            cfg.SaveToken = true;
            cfg.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(encryptionKey)),
                ValidateIssuer = false,
                ValidateAudience = false,
            };
        });
    }
}
