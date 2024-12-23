﻿using WeatherMonitor.Server.SharedKernel.Models;

namespace WeatherMonitor.Server.UserPermissionManagement.Infrastructure.Models;

public readonly record struct UserPermissionRequestDto(
    int Id,
    string UserId,
    int DeviceId,
    string? DeviceName,
    string? GoogleMapsPlusCode,
    PermissionStatus PermissionStatus,
    DateTime DateTime);
