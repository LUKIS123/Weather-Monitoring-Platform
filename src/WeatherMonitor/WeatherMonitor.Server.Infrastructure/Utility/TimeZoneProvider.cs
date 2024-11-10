using System.Runtime.InteropServices;
using TimeZoneConverter;
using WeatherMonitor.Server.Interfaces;

namespace WeatherMonitor.Server.Infrastructure.Utility;
internal class TimeZoneProvider : ITimeZoneProvider
{
    private readonly TimeZoneInfo _timeZoneInfo;

    public TimeZoneProvider(string timeZoneId)
    {
        var platformSpecificTimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TZConvert.IanaToWindows(timeZoneId)
            : timeZoneId;
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(platformSpecificTimeZoneId);
    }

    public TimeZoneInfo GetTimeZoneInfo() => _timeZoneInfo;
}
