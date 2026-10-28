using System;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdDockWorldClock;

public partial class WorldClockCommandsProvider : CommandProvider
{
    private readonly ICommandItem[] _commands;
    private readonly ICommandItem[] _dockBands;
    private readonly ClockUpdateService _clockUpdateService = new();
    private readonly CitySettingsManager _settingsManager = new();
    private readonly LiveWorldClockPage _mainPage;
    private readonly LiveWorldClockPage _dockPage;
    private bool _disposed;

    public WorldClockCommandsProvider()
    {
        DisplayName = "World Clock";
        Id = "com.shench.worldclock";
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");

        _mainPage = new(
            CreateClockItems(_settingsManager.SelectedCities),
            _clockUpdateService,
            "com.shench.worldclock.page",
            "World Clock",
            "Open");
        _dockPage = new(
            CreateClockItems(_settingsManager.SelectedCities),
            _clockUpdateService,
            "com.shench.worldclock.dock",
            "World Clock",
            "World Clock");

        _commands = [
            new CommandItem(_mainPage)
            {
                Title = DisplayName,
                Icon = Icon,
                MoreCommands = [new CommandContextItem(_settingsManager.Settings.SettingsPage)],
            },
            new CommandItem(_settingsManager.Settings.SettingsPage)
            {
                Title = "Choose world clock cities",
                Subtitle = "Select up to five cities for the Dock",
                Icon = new IconInfo("\uE713"),
            },
        ];
        _dockBands = [
            new CommandItem(_dockPage)
            {
                Title = DisplayName,
                Icon = Icon,
            },
        ];

        Settings = _settingsManager.Settings;
        _settingsManager.CitiesChanged += CitiesChanged;
    }

    public override ICommandItem[] TopLevelCommands() => _commands;

    public override ICommandItem[] GetDockBands() => _dockBands;

    public override void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _settingsManager.CitiesChanged -= CitiesChanged;
        _mainPage.Dispose();
        _dockPage.Dispose();
        _clockUpdateService.Dispose();
        GC.SuppressFinalize(this);
        base.Dispose();
    }

    private void CitiesChanged(object? sender, EventArgs e)
    {
        var selectedCities = _settingsManager.SelectedCities;
        _mainPage.SetItems(CreateClockItems(selectedCities));
        _dockPage.SetItems(CreateClockItems(selectedCities));
    }

    private static WorldClockItem[] CreateClockItems(
        System.Collections.Generic.IReadOnlyList<WorldClockDefinition> cities)
    {
        var items = new WorldClockItem[cities.Count];
        for (var index = 0; index < cities.Count; index++)
        {
            items[index] = new WorldClockItem(cities[index]);
        }

        return items;
    }
}
