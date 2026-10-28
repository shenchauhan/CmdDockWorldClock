using System;
using System.Globalization;

namespace CmdDockWorldClock;

internal static class WorldClockFormatter
{
    internal static DateTimeOffset GetLocalTime(DateTimeOffset utcNow, TimeZoneInfo timeZone)
        => TimeZoneInfo.ConvertTime(utcNow.ToUniversalTime(), timeZone);

    internal static string FormatTitle(WorldClockDefinition definition, DateTimeOffset localTime, CultureInfo culture)
        => $"{definition.City} {localTime.ToString("t", culture)}";

    internal static string FormatSubtitle(DateTimeOffset localTime, CultureInfo culture)
        => $"{localTime.ToString("ddd, MMM d", culture)} | {FormatUtcOffset(localTime.Offset)}";

    internal static string FormatCopyText(WorldClockDefinition definition, DateTimeOffset localTime, CultureInfo culture)
        => $"{definition.Emoji} {definition.City}: {localTime.ToString("f", culture)} ({FormatUtcOffset(localTime.Offset)})";

    internal static string FormatUtcOffset(TimeSpan offset)
    {
        if (offset == TimeSpan.Zero)
        {
            return "UTC";
        }

        var sign = offset < TimeSpan.Zero ? "-" : "+";
        var absoluteOffset = offset.Duration();
        return $"UTC{sign}{absoluteOffset.Hours:00}:{absoluteOffset.Minutes:00}";
    }
}
