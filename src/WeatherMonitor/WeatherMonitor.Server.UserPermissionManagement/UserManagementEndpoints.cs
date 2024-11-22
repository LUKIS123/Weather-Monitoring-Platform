using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitor.Server.SharedKernel.HttpContextExtensions;
using WeatherMonitor.Server.UserPermissionManagement.Features.GetPendingPermissionRequests;
using WeatherMonitor.Server.UserPermissionManagement.Features.SetUserPermission;

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
            async (HttpContext context, [FromServices] IGetPendingRequestsService getPendingRequestsService,
                [FromBody] UpdatePermissionRequest request) =>
            {
                var result = await getPendingRequestsService.Handle(pageNumber);
                await context.HandleResult(result);
            }).RequireAuthorization("IsAdminPolicy");
    }
}