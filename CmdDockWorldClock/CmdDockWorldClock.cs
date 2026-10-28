// Copyright (c) Microsoft Corporation
// The Microsoft Corporation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;
using Microsoft.CommandPalette.Extensions;

namespace CmdDockWorldClock;

[Guid("A8023A80-0B87-45E9-895E-020B56A5BA9E")]
public sealed partial class CmdDockWorldClock : IExtension, IDisposable
{
    private readonly ManualResetEvent _extensionDisposedEvent;

    private readonly WorldClockCommandsProvider _provider = new();

    public CmdDockWorldClock(ManualResetEvent extensionDisposedEvent)
    {
        this._extensionDisposedEvent = extensionDisposedEvent;
    }

    public object? GetProvider(ProviderType providerType)
    {
        return providerType switch
        {
            ProviderType.Commands => _provider,
            _ => null,
        };
    }

    public void Dispose()
    {
        _provider.Dispose();
        _extensionDisposedEvent.Set();
    }
}
