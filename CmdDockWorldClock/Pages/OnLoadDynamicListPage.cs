using System;
using System.Threading;
using Microsoft.CommandPalette.Extensions;
using Microsoft.CommandPalette.Extensions.Toolkit;
using Windows.Foundation;

namespace CmdDockWorldClock;

internal abstract partial class OnLoadDynamicListPage : Page, IDynamicListPage
{
    private readonly Lock _loadLock = new();
    private int _loadCount;
    private string _searchText = string.Empty;

#pragma warning disable CS0067 // Raised by the inherited WinRT event implementation.
    private event TypedEventHandler<object, IItemsChangedEventArgs>? InternalItemsChanged;
#pragma warning restore CS0067

    public virtual string PlaceholderText { get; set => SetProperty(ref field, value); } = string.Empty;

    public virtual string SearchText
    {
        get => _searchText;
        set
        {
            var oldSearch = _searchText;
            _searchText = value;
            UpdateSearchText(oldSearch, value);
        }
    }

    public virtual bool ShowDetails { get; set => SetProperty(ref field, value); }

    public virtual bool HasMoreItems { get; set => SetProperty(ref field, value); }

    public virtual IFilters? Filters { get; set => SetProperty(ref field, value); }

    public virtual IGridProperties? GridProperties { get; set => SetProperty(ref field, value); }

    public virtual ICommandItem? EmptyContent { get; set => SetProperty(ref field, value); }

    public event TypedEventHandler<object, IItemsChangedEventArgs> ItemsChanged
    {
        add
        {
            InternalItemsChanged += value;
            lock (_loadLock)
            {
                if (_loadCount++ == 0)
                {
                    Loaded();
                }
            }
        }

        remove
        {
            InternalItemsChanged -= value;
            lock (_loadLock)
            {
                _loadCount = Math.Max(0, _loadCount - 1);
                if (_loadCount == 0)
                {
                    Unloaded();
                }
            }
        }
    }

    public void LoadMore()
    {
    }

    public abstract IListItem[] GetItems();

    public abstract void UpdateSearchText(string oldSearch, string newSearch);

    protected void RaiseItemsChanged(int totalItems = -1)
        => InternalItemsChanged?.Invoke(this, new ItemsChangedEventArgs(totalItems));

    protected abstract void Loaded();

    protected abstract void Unloaded();
}
