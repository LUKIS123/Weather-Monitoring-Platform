using WeatherMonitorCore.Infrastructure;
using WeatherMonitorCore.MqttDataSubscriberService;
using WeatherMonitorCore.SharedKernel.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

builder.Logging.AddApplicationInsights();

builder.Services.AddHostedService<MqttSubscriptionsHandlingWorker>();
builder.Services.AddInfrastructureModule(builder.Configuration, InfrastructureType.WorkerService);

builder.Services.AddApplicationInsightsTelemetryWorkerService();

var host = builder.Build();
await host.StartAsync();
