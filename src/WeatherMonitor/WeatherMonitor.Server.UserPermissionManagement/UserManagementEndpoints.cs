using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetPendingPermissionRequests;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetUserPermissions;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetUsers;
using WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;
using WeatherMonitor.Server.UserPermissionManagement.Features.SetUserRole;

namespace WeatherMonitor.Server.UserPermissionManagement;

public static class UserManagementEndpoints
{
    public static void RegisterUserManagementEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet(
            "/api/userManagement/pending",
            async (HttpContext context, [FromServices] IGetPendingRequestsService getPendingRequestsService,
                [FromQuery] int pageNumber) =>
            {
                var result = await getPendingRequestsService.Handle(pageNumber);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapPost(
            "/api/userManagement/updatePermission",
            async (HttpContext context, [FromServices] ISetUsersStationPermissionService setUsersStationPermissionService,
                [FromBody] UpdatePermissionRequest request) =>
            {
                var result = await setUsersStationPermissionService.Handle(request);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapGet(
            "/api/userManagement/users",
            async (HttpContext context, [FromServices] IGetUsersService getUsersService,
                [FromQuery] int pageNumber, [FromQuery] string? nicknameSearch = null) =>
            {
                var result = await getUsersService.Handle(pageNumber, nicknameSearch);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapGet(
            "/api/userManagement/userPermissions",
            async (HttpContext context, [FromServices] IGetUsersPermissionsService getUsersPermissionsService,
                [FromQuery] int pageNumber, [FromQuery] string userId) =>
            {
                var result = await getUsersPermissionsService.Handle(pageNumber, userId);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");

        routes.MapPost(
            "/api/userManagement/grantAdminRole",
            async (HttpContext context, [FromServices] ISetUserRoleService setUserRoleService,
                [FromBody] SetUserRoleRequest setUserRoleRequest) =>
            {
                var result = await setUserRoleService.Handle(setUserRoleRequest);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");
    }
}