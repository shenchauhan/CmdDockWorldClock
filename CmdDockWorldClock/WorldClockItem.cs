using System;
using System.Globalization;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdDockWorldClock;

internal sealed partial class WorldClockItem : ListItem
{
    private readonly WorldClockDefinition _definition;
    private readonly TimeZoneInfo _timeZone;
    private readonly CopyTextCommand _copyCommand;

    internal WorldClockItem(WorldClockDefinition definition)
    {
        _definition = definition;
        _timeZone = TimeZoneInfo.FindSystemTimeZoneById(definition.TimeZoneId);
        _copyCommand = new CopyTextCommand(string.Empty)
        {
            Id = $"com.shench.worldclock.copy.{definition.Id}",
            Name = $"Copy {definition.City} time",
        };

        Command = _copyCommand;
        Icon = new IconInfo(definition.Emoji);
        Update(DateTimeOffset.UtcNow);
    }

    internal void Update(DateTimeOffset utcNow)
    {
        var localTime = WorldClockFormatter.GetLocalTime(utcNow, _timeZone);
        var culture = CultureInfo.CurrentCulture;
        Title = WorldClockFormatter.FormatTitle(_definition, localTime, culture);
        Subtitle = WorldClockFormatter.FormatSubtitle(localTime, culture);
        _copyCommand.Text = WorldClockFormatter.FormatCopyText(_definition, localTime, culture);
    }
}
