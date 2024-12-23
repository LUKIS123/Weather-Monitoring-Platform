﻿using WeatherMonitorCore.Contract.Shared;

namespace WeatherMonitor.Server.UserAuthentication.Infrastructure.Models;
internal class UserInfo
{
    public required string UserId { get; set; }
    public required string UserName { get; set; }
    public required string PhotoUrl { get; set; }
    public required string Email { get; set; }
    public Role Role { get; set; }
}
