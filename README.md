# World Clock for PowerToys Command Palette Dock

A Command Palette extension that adds one Dock band with up to five live clocks.
Choose each city in the extension's Command Palette settings. The defaults are:

- 🏔️ Seattle (`Pacific Standard Time`)
- 💂 London (`GMT Standard Time`)
- 🗼 Tokyo (`Tokyo Standard Time`)

Times use the current Windows regional short-time format and update at the start
of each minute. Windows time-zone data handles daylight-saving changes. Selecting
a clock copies its current local date and time.

## Choose cities

1. Open Command Palette and run **Choose world clock cities**.
2. Choose a city in each of the five slots. Select **None** for unused slots.

Duplicate selections are shown once, in slot order.

## Build and deploy

Prerequisites: Windows 11, PowerToys with Command Palette Dock support, Developer
Mode, .NET 10, and Visual Studio's Windows application development workload.

1. Open `CmdDockWorldClock.sln` in Visual Studio.
2. Select the `ARM64` or `x64` platform that matches Windows.
3. Choose **Build > Deploy CmdDockWorldClock**.
4. Open Command Palette and run **Reload Command Palette extensions**.
5. Enable the Dock, enter Dock edit mode, and add the **World Clock** band.

The extension requires Command Palette Extension SDK 0.11 or later.
