using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using WeatherMonitorCore.MqttAuth.Features.AclCheck;
using WeatherMonitorCore.MqttAuth.Features.BrokerClientAuthentication;
using WeatherMonitorCore.MqttAuth.Features.SuperUserCheck;
using WeatherMonitorCore.SharedKernel;

namespace WeatherMonitorCore.MqttAuth;

public static class MqttAuthEndpoints
{
    public static void RegisterMqttBrokerAuthEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapPost(
            "/api/mqttauth/user",
            async (HttpContext context, [FromServices] IBrokerClientAuthenticationService authenticationService,
                [FromBody] AuthenticationRequest authenticationRequest) =>
            {
                var result = await authenticationService.Handle(authenticationRequest);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapPost(
            "/api/mqttauth/superuser",
            async (HttpContext context, [FromServices] ISuperUserCheckService superUserCheckService,
                [FromBody] SuperUserCheckRequest superUserCheckRequest) =>
            {
                var result = await superUserCheckService.Handle(superUserCheckRequest);
                await context.HandleResult(result);
            }).AllowAnonymous();

        routes.MapPost(
            "/api/mqttauth/acl",
            async (HttpContext context, [FromServices] IAclCheckService aclCheckService,
                [FromBody] AclCheckRequest aclCheckRequest) =>
            {
                var result = await aclCheckService.Handle(aclCheckRequest);
                await context.HandleResult(result);
            }).AllowAnonymous();
    }

}
