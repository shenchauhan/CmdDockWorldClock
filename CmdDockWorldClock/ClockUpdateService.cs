using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.CommandPalette.Extensions.Toolkit;

namespace CmdDockWorldClock;

internal sealed partial class ClockUpdateService : IDisposable
{
    private readonly Lock _lock = new();
    private readonly Dictionary<object, EventHandler> _clients = new(ReferenceEqualityComparer.Instance);
    private readonly System.Timers.Timer _timer = new() { AutoReset = false };
    private bool _disposed;

    internal ClockUpdateService()
    {
        _timer.Elapsed += TimerElapsed;
    }

    internal void Subscribe(object client, EventHandler handler)
    {
        lock (_lock)
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            _clients[client] = handler;
            ScheduleNextTick();
        }
    }

    internal void Unsubscribe(object client)
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _clients.Remove(client);
            ScheduleNextTick();
        }
    }

    private void TimerElapsed(object? sender, System.Timers.ElapsedEventArgs e)
    {
        EventHandler[] handlers;
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            handlers = [.. _clients.Values];
        }

        foreach (var handler in handlers)
        {
            try
            {
                handler(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                ExtensionHost.LogMessage($"World Clock update failed: {ex.Message}");
            }
        }

        lock (_lock)
        {
            if (!_disposed)
            {
                ScheduleNextTick();
            }
        }
    }

    private void ScheduleNextTick()
    {
        _timer.Stop();
        if (_clients.Count == 0)
        {
            return;
        }

        var now = DateTimeOffset.Now;
        var interval = TimeSpan.FromMinutes(1)
            - TimeSpan.FromSeconds(now.Second)
            - TimeSpan.FromMilliseconds(now.Millisecond);
        _timer.Interval = Math.Max(1, interval.TotalMilliseconds);
        _timer.Start();
    }

    public void Dispose()
    {
        lock (_lock)
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _clients.Clear();
            _timer.Stop();
        }

        _timer.Dispose();
    }
}
