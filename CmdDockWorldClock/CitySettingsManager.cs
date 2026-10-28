using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdDockWorldClock;

internal sealed partial class CitySettingsManager : JsonSettingsManager
{
    private const string NoCity = "none";
    private readonly ChoiceSetSetting[] _citySlots;

    internal event EventHandler? CitiesChanged;

    internal CitySettingsManager()
    {
        FilePath = SettingsJsonPath();

        var choices = CreateChoices();
        string[] defaults = ["seattle", "london", "tokyo", NoCity, NoCity];
        _citySlots = new ChoiceSetSetting[CityCatalog.MaximumSelectedCities];

        for (var index = 0; index < _citySlots.Length; index++)
        {
            var slotNumber = index + 1;
            var slot = new ChoiceSetSetting(
                $"worldClock.city{slotNumber}",
                $"City {slotNumber}",
                $"City slot {slotNumber}: choose a city or select None to hide this clock.",
                choices)
            {
                Value = defaults[index],
            };
            _citySlots[index] = slot;
            Settings.Add(slot);
        }

        LoadSettings();
        Settings.SettingsChanged += SettingsChanged;
    }

    internal IReadOnlyList<WorldClockDefinition> SelectedCities
    {
        get
        {
            var selectedIds = new string?[_citySlots.Length];
            for (var index = 0; index < _citySlots.Length; index++)
            {
                selectedIds[index] = _citySlots[index].Value;
            }

            return CityCatalog.Resolve(selectedIds);
        }
    }

    private static List<ChoiceSetSetting.Choice> CreateChoices()
    {
        var choices = new List<ChoiceSetSetting.Choice>
        {
            new("None", NoCity),
        };

        foreach (var city in CityCatalog.All)
        {
            choices.Add(new($"{city.Emoji} {city.City}", city.Id));
        }

        return choices;
    }

    private static string SettingsJsonPath()
    {
        var directory = Utilities.BaseSettingsPath("CmdDockWorldClock");
        Directory.CreateDirectory(directory);
        return Path.Combine(directory, "settings.json");
    }

    private void SettingsChanged(object sender, Settings args)
    {
        SaveSettings();
        CitiesChanged?.Invoke(this, EventArgs.Empty);
    }
}
