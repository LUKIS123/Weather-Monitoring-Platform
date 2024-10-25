using WeatherMonitorCore.Infrastructure;
using WeatherMonitorCore.MqttDataSubscriberService;
using WeatherMonitorCore.MqttDataSubscriberService.Configuration;
using WeatherMonitorCore.MqttDataSubscriberService.Utils;
using WeatherMonitorCore.SharedKernel.Infrastructure;

var builder = Host.CreateApplicationBuilder(args);

var mqttBrokerSection = builder.Configuration.GetSection("MqttBroker")
                        ?? throw new ArgumentNullException(nameof(builder.Configuration), "MqttBroker");
var brokerConnectionSettings = new MqttBrokerConnection();
mqttBrokerSection.Bind(brokerConnectionSettings);
builder.Services.AddSingleton(brokerConnectionSettings);

builder.Logging.AddApplicationInsights();

builder.Services.AddTransient<IServiceWorkerMqttClientGenerator, ServiceWorkerMqttClientGenerator>();
builder.Services.AddTransient<IMqttDataService, MqttDataService>();
builder.Services.AddHostedService<MqttSubscriptionsHandlingWorker>();
builder.Services.AddInfrastructureModule(builder.Configuration, InfrastructureType.WorkerService);

builder.Services.AddApplicationInsightsTelemetryWorkerService();

var host = builder.Build();
await host.StartAsync();
