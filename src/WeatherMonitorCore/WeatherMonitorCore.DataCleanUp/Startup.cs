using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using WeatherMonitorCore.DataCleanUp.Settings;
using WeatherMonitorCore.DataCleanUp.Utils;

[assembly: FunctionsStartup(typeof(WeatherMonitorCore.DataCleanUp.Startup))]

namespace WeatherMonitorCore.DataCleanUp;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        builder.ConfigurationBuilder.AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
               .AddEnvironmentVariables()
               .Build();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        var context = builder.GetContext();

        builder.Services.AddLogging();
        builder.Services.AddSingleton(TimeProvider.System);

        var sqlConnectionString = context.Configuration.GetConnectionString("MSSQL")
                                  ?? throw new ArgumentNullException(nameof(context.Configuration), "MSSQL");
        builder.Services.AddSingleton(new ConnectionSettings { ConnectionString = sqlConnectionString });
        builder.Services.AddTransient<IDataCleanupRepository, DataCleanupRepository>();

        var appTimeZoneId = context.Configuration.GetValue<string>("AppTimeZone")
                            ?? throw new ArgumentNullException(nameof(context.Configuration), "AppTimeZone");
        var timeZoneSettings = new TimeZoneSettings { TimeZoneId = appTimeZoneId };
        builder.Services.AddSingleton(timeZoneSettings);

        builder.Services.AddTransient<ITimeZoneProvider, TimeZoneProvider>();
    }
}
