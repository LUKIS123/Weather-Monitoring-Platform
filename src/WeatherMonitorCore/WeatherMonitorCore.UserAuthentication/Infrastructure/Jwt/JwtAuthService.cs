using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WeatherMonitorCore.UserAuthentication.Infrastructure.Models;

namespace WeatherMonitorCore.UserAuthentication.Infrastructure.Jwt
{
    internal interface IJwtAuthorizationService
    {
        string GenerateJwtToken(UserInfo userInfo);
    }

    internal class JwtAuthorizationService : IJwtAuthorizationService
    {
        private readonly IConfiguration _configuration;

        private const string JwtKey = "Jwt:Key";

        public JwtAuthorizationService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(UserInfo userInfo)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = _configuration[JwtKey] ?? throw new ArgumentNullException(JwtKey);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity([
                    new Claim("UserId", userInfo.UserId),
                    new Claim("UserName", userInfo.UserName),
                    new Claim("PhotoUrl", userInfo.PhotoUrl),
                    new Claim(ClaimTypes.Email, userInfo.Email)
                ]),
                Expires = DateTime.UtcNow.AddDays(3),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
