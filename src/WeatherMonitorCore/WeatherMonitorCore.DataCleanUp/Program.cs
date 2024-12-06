using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using WeatherMonitorCore.DataCleanUp;
using WeatherMonitorCore.DataCleanUp.Settings;
using WeatherMonitorCore.DataCleanUp.Utils;


var hostBuilder = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults(builder =>
    {
        builder.UseMiddleware<ExceptionHandlingMiddleware>();
    })
    .ConfigureServices((context, services) =>
    {
        services.AddLogging();

        var sqlConnectionString = context.Configuration.GetConnectionString("MSSQL")
                                  ?? throw new ArgumentNullException(nameof(context.Configuration), "MSSQL");
        services.AddTransient(_ => new ConnectionSettings { ConnectionString = sqlConnectionString });
        services.AddTransient<DataCleanupRepository>();

        var appTimeZoneId = context.Configuration.GetSection("AppTimeZone")
                             ?? throw new ArgumentNullException(nameof(context.Configuration), "AppTimeZone");
        var timeZoneSettings = new TimeZoneSettings();
        appTimeZoneId.Bind(timeZoneSettings);
        services.AddTransient(_ => timeZoneSettings);
    });

var host = hostBuilder.Build();

host.Run();
