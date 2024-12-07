using System.Runtime.InteropServices;
using TimeZoneConverter;
using WeatherMonitorCore.DataCleanUp.Settings;

namespace WeatherMonitorCore.DataCleanUp.Utils;

public interface ITimeZoneProvider
{
    TimeZoneInfo GetTimeZoneInfo();
}

public class TimeZoneProvider : ITimeZoneProvider
{
    private readonly TimeZoneInfo _timeZoneInfo;

    public TimeZoneProvider(TimeZoneSettings settings)
    {
        var platformSpecificTimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TZConvert.IanaToWindows(settings.TimeZoneId)
            : settings.TimeZoneId;
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(platformSpecificTimeZoneId);
    }

    public TimeZoneInfo GetTimeZoneInfo() => _timeZoneInfo;
}
