using System.Runtime.InteropServices;
using TimeZoneConverter;
using WeatherMonitorCore.Interfaces;

namespace WeatherMonitorCore.Infrastructure.Utility;
internal class TimeZoneProvider : ITimeZoneProvider
{
    private readonly TimeZoneInfo _timeZoneInfo;

    public TimeZoneProvider(string timeZoneId)
    {
        var platformSpecificTimeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
            ? TZConvert.IanaToWindows(timeZoneId)
            : TZConvert.WindowsToIana(timeZoneId);
        _timeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(platformSpecificTimeZoneId);
    }

    public TimeZoneInfo GetTimeZoneInfo() => _timeZoneInfo;
}
