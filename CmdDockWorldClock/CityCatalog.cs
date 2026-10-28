using System;
using System.Collections.Generic;

namespace CmdDockWorldClock;

internal static class CityCatalog
{
    internal const int MaximumSelectedCities = 5;

    internal static IReadOnlyList<WorldClockDefinition> All { get; } =
    [
        new("seattle", "Seattle", "🏔️", "Pacific Standard Time"),
        new("vancouver", "Vancouver", "🍁", "Pacific Standard Time"),
        new("los-angeles", "Los Angeles", "🌴", "Pacific Standard Time"),
        new("denver", "Denver", "⛰️", "Mountain Standard Time"),
        new("chicago", "Chicago", "🌆", "Central Standard Time"),
        new("mexico-city", "Mexico City", "🌮", "Central Standard Time (Mexico)"),
        new("new-york", "New York", "🗽", "Eastern Standard Time"),
        new("toronto", "Toronto", "🇨🇦", "Eastern Standard Time"),
        new("honolulu", "Honolulu", "🌺", "Hawaiian Standard Time"),
        new("anchorage", "Anchorage", "🐻", "Alaskan Standard Time"),
        new("sao-paulo", "São Paulo", "🇧🇷", "E. South America Standard Time"),
        new("buenos-aires", "Buenos Aires", "🇦🇷", "Argentina Standard Time"),
        new("london", "London", "💂", "GMT Standard Time"),
        new("dublin", "Dublin", "☘️", "GMT Standard Time"),
        new("paris", "Paris", "🥐", "Romance Standard Time"),
        new("berlin", "Berlin", "🐻", "W. Europe Standard Time"),
        new("madrid", "Madrid", "🇪🇸", "Romance Standard Time"),
        new("rome", "Rome", "🏛️", "W. Europe Standard Time"),
        new("amsterdam", "Amsterdam", "🚲", "W. Europe Standard Time"),
        new("cairo", "Cairo", "🏺", "Egypt Standard Time"),
        new("johannesburg", "Johannesburg", "🦁", "South Africa Standard Time"),
        new("dubai", "Dubai", "🏙️", "Arabian Standard Time"),
        new("delhi", "Delhi", "🕌", "India Standard Time"),
        new("mumbai", "Mumbai", "🛕", "India Standard Time"),
        new("singapore", "Singapore", "🦁", "Singapore Standard Time"),
        new("hong-kong", "Hong Kong", "🌃", "China Standard Time"),
        new("beijing", "Beijing", "🐉", "China Standard Time"),
        new("tokyo", "Tokyo", "🗼", "Tokyo Standard Time"),
        new("seoul", "Seoul", "🇰🇷", "Korea Standard Time"),
        new("sydney", "Sydney", "🦘", "AUS Eastern Standard Time"),
        new("melbourne", "Melbourne", "☕", "AUS Eastern Standard Time"),
        new("auckland", "Auckland", "🥝", "New Zealand Standard Time"),
    ];

    internal static IReadOnlyList<WorldClockDefinition> Resolve(IEnumerable<string?> cityIds)
    {
        var selected = new List<WorldClockDefinition>(MaximumSelectedCities);
        var seen = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var cityId in cityIds)
        {
            if (selected.Count == MaximumSelectedCities)
            {
                break;
            }

            if (string.IsNullOrWhiteSpace(cityId) || !seen.Add(cityId))
            {
                continue;
            }

            foreach (var city in All)
            {
                if (string.Equals(city.Id, cityId, StringComparison.OrdinalIgnoreCase))
                {
                    selected.Add(city);
                    break;
                }
            }
        }

        return selected;
    }
}
