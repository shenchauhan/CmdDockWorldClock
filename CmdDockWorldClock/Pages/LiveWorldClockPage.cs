using System;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdDockWorldClock;

internal sealed partial class LiveWorldClockPage : OnLoadDynamicListPage, IDisposable
{
    private readonly Lock _itemsLock = new();
    private readonly ClockUpdateService _clockUpdateService;
    private WorldClockItem[] _items;
    private bool _disposed;

    internal LiveWorldClockPage(
        WorldClockItem[] items,
        ClockUpdateService clockUpdateService,
        string id,
        string title,
        string name)
    {
        _items = items;
        _clockUpdateService = clockUpdateService;
        Id = id;
        Title = title;
        Name = name;
        Icon = IconHelpers.FromRelativePath("Assets\\StoreLogo.png");
    }

    public override IListItem[] GetItems()
    {
        lock (_itemsLock)
        {
            return _items;
        }
    }

    internal void SetItems(WorldClockItem[] items)
    {
        lock (_itemsLock)
        {
            _items = items;
        }

        UpdateClocks();
        RaiseItemsChanged(items.Length);
    }

    public override void UpdateSearchText(string oldSearch, string newSearch)
    {
    }

    protected override void Loaded()
    {
        if (_disposed)
        {
            return;
        }

        _clockUpdateService.Subscribe(this, ClockUpdateServiceTick);
        UpdateClocks();
    }

    protected override void Unloaded() => _clockUpdateService.Unsubscribe(this);

    private void ClockUpdateServiceTick(object? sender, EventArgs e) => UpdateClocks();

    private void UpdateClocks()
    {
        var utcNow = DateTimeOffset.UtcNow;
        WorldClockItem[] items;
        lock (_itemsLock)
        {
            items = _items;
        }

        foreach (var item in items)
        {
            item.Update(utcNow);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _clockUpdateService.Unsubscribe(this);
    }
}
