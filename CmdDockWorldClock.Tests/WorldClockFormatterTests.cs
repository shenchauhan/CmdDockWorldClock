using System.Globalization;

namespace CmdDockWorldClock.Tests;

[TestClass]
public sealed class WorldClockFormatterTests
{
    private static readonly CultureInfo UsEnglish = CultureInfo.GetCultureInfo("en-US");

    [TestMethod]
    [DataRow("Pacific Standard Time", 5, 0, -7)]
    [DataRow("GMT Standard Time", 13, 0, 1)]
    [DataRow("Tokyo Standard Time", 21, 0, 9)]
    public void GetLocalTimeAppliesSummerOffsets(
        string timeZoneId,
        int expectedHour,
        int expectedMinute,
        int expectedOffsetHours)
    {
        var utc = new DateTimeOffset(2026, 7, 1, 12, 0, 0, TimeSpan.Zero);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var local = WorldClockFormatter.GetLocalTime(utc, timeZone);

        Assert.AreEqual(expectedHour, local.Hour);
        Assert.AreEqual(expectedMinute, local.Minute);
        Assert.AreEqual(TimeSpan.FromHours(expectedOffsetHours), local.Offset);
    }

    [TestMethod]
    [DataRow("Pacific Standard Time", 4, -8)]
    [DataRow("GMT Standard Time", 12, 0)]
    [DataRow("Tokyo Standard Time", 21, 9)]
    public void GetLocalTimeAppliesWinterOffsets(
        string timeZoneId,
        int expectedHour,
        int expectedOffsetHours)
    {
        var utc = new DateTimeOffset(2026, 1, 1, 12, 0, 0, TimeSpan.Zero);
        var timeZone = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);

        var local = WorldClockFormatter.GetLocalTime(utc, timeZone);

        Assert.AreEqual(expectedHour, local.Hour);
        Assert.AreEqual(TimeSpan.FromHours(expectedOffsetHours), local.Offset);
    }

    [TestMethod]
    public void FormatTitleUsesRegionalShortTime()
    {
        var definition = new WorldClockDefinition("seattle", "Seattle", "🏔️", "Pacific Standard Time");
        var local = new DateTimeOffset(2026, 7, 1, 5, 7, 0, TimeSpan.FromHours(-7));

        var title = WorldClockFormatter.FormatTitle(definition, local, UsEnglish);

        Assert.AreEqual("Seattle 5:07 AM", title);
    }

    [TestMethod]
    [DataRow(0, "UTC")]
    [DataRow(-8, "UTC-08:00")]
    [DataRow(9, "UTC+09:00")]
    public void FormatUtcOffsetUsesSignedHours(int hours, string expected)
    {
        Assert.AreEqual(expected, WorldClockFormatter.FormatUtcOffset(TimeSpan.FromHours(hours)));
    }

    [TestMethod]
    public void FormatCopyTextIncludesEmojiCityAndOffset()
    {
        var definition = new WorldClockDefinition("london", "London", "💂", "GMT Standard Time");
        var local = new DateTimeOffset(2026, 7, 1, 13, 0, 0, TimeSpan.FromHours(1));

        var text = WorldClockFormatter.FormatCopyText(definition, local, UsEnglish);

        Assert.AreEqual("💂 London: Wednesday, July 1, 2026 1:00 PM (UTC+01:00)", text);
    }
}
