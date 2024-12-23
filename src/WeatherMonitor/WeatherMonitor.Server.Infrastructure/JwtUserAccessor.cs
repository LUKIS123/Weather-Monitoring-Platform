﻿using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using WeatherMonitor.Server.Interfaces;
using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.Infrastructure;

internal class JwtUserAccessor : IUserAccessor
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    private const string UserIdClaim = "UserId";
    private const string UserNameClaim = "UserName";
    private const string PhotoUrlClaim = "PhotoUrl";
    private const string EmailClaim = ClaimTypes.Email;
    private const string RoleClaim = ClaimTypes.Role;
    private const string TokenCookie = "AuthToken";

    public JwtUserAccessor(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public string? UserId
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == UserIdClaim);

            return claim?.Value;
        }
    }

    public string? UserName
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == UserNameClaim);

            return claim?.Value;
        }
    }

    public string? PhotoUrl
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == PhotoUrlClaim);

            return claim?.Value;
        }
    }

    public string? Email
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == EmailClaim);

            return claim?.Value;
        }
    }

    public Role Role
    {
        get
        {
            var claims = _httpContextAccessor.HttpContext?.User.Claims;
            var claim = claims?.FirstOrDefault(x => x.Type == RoleClaim);

            if (int.TryParse(claim?.Value, out var result))
            {
                return (Role)result;
            }

            return Role.None;
        }
    }

    public string? Token => _httpContextAccessor.HttpContext?.Request.Cookies[TokenCookie];
}