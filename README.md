# ProcessSet Manager

**English** | [Deutsch](README.de.md)

[![Version](https://img.shields.io/badge/Version-0.6.0-2ea44f)](#)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D4?logo=windows11&logoColor=white)](#)
[![C%23](https://img.shields.io/badge/C%23-WinForms-239120?logo=csharp&logoColor=white)](#)
[![UI Languages](https://img.shields.io/badge/UI-Deutsch%20%7C%20English-F7DF1E)](#)
[![License](https://img.shields.io/badge/License-MIT-blue)](LICENSE)
[![Status](https://img.shields.io/badge/Status-In%20Development-orange)](#)
[![GitHub stars](https://img.shields.io/github/stars/netdaemon91/ProcessSetManager?style=flat)](https://github.com/netdaemon91/ProcessSetManager/stargazers)
[![Last commit](https://img.shields.io/github/last-commit/netdaemon91/ProcessSetManager)](https://github.com/netdaemon91/ProcessSetManager/commits/main)

ProcessSet Manager is a lightweight open-source session and process profile manager for Windows. It lets you group running applications into reusable profiles, close them in a controlled order, and later restore the previous application state as far as possible.

> Current version: **0.6.0**  
> Platform: **Windows x64**  
> Framework: **.NET 10 / WinForms**  
> License: **MIT**

## Concept

Instead of manually closing the same applications every time you switch between work, gaming, or streaming, ProcessSet Manager lets you save those applications as a profile.

Example: `Gaming`

1. Photoshop, Thunderbird, and OneDrive are running.
2. The `Gaming` profile is activated.
3. ProcessSet shows which processes would be closed and which can be restored later.
4. The selected applications are closed one after another.
5. After gaming, you end the mode.
6. ProcessSet restarts the applications it was able to save for restoration.

ProcessSet Manager is **not a PC booster**. It deliberately avoids aggressive registry tweaks, RAM cleaners, service manipulation, and scheduler tweaks. The focus is on predictable process profiles and reproducible session changes.

## Features

- list currently running Windows processes
- search by process name, window title, path, and type
- optionally show only regular applications with windows
- select processes via checkboxes
- define the shutdown order
- activate the current selection directly as a mode
- save, load, rename, duplicate, and delete profiles
- activate profiles from the system tray
- persist the currently active mode
- restore previously closed applications when possible
- maintain a personal protection list
- built-in protection for critical Windows processes
- preview every mode activation before anything is closed
- show whether a process is automatically restorable (`↻`)
- result summaries after activation and restore
- export profiles as TXT, PowerShell, or Batch files
- integrated help and tooltips
- German and English interface
- dark UI
- Per-Monitor-V2 DPI support
- crash log for unexpected errors

## Restore behavior

Before activating a mode, ProcessSet checks which profile processes are currently running and whether a readable EXE path is available.

A process marked `↻ Yes` can generally be restarted automatically later. `↻ No` does **not** mean that closing the process will fail; it only means automatic restoration cannot be guaranteed.

### Important limitation

ProcessSet restarts applications, but it does **not** reconstruct their internal state itself. Examples include:

- open browser tabs
- unsaved documents
- open Photoshop projects
- window positions
- unsaved input

If an application provides its own session recovery, that recovery may of course take effect when ProcessSet starts the application again.

## Protection list

Additional process names can be protected under `Settings`. Protected processes are never terminated by ProcessSet, even when they are included in an older profile.

Critical Windows processes are also protected by built-in rules.

## System tray

By default, ProcessSet can move to the Windows notification area when minimized. From the tray menu you can:

- activate saved profiles
- see the currently active mode
- end the active mode and restore its session
- reopen the main window
- exit ProcessSet Manager

## Export

Profiles can also be exported as:

- `.txt` — plain process names
- `.ps1` — PowerShell script for sequential process termination
- `.bat` — Batch script for terminating the processes

This keeps the original process-list concept usable even outside the GUI.

## Integrated help

Version 0.6 includes a dedicated `Help` tab that explains profile creation, mode activation, restoration, the protection list, tray mode, exports, and language selection directly inside the application.

## Languages

The interface supports **German and English**. On a fresh installation, ProcessSet automatically selects German or English based on the Windows UI language.

You can change the language at any time under `Settings → Language / Sprache`. The new language takes effect after restarting the application.

Profiles, protection lists, and restore sessions remain language-independent and can be reused in either UI language.

## Data locations

ProcessSet stores user data in the standard Windows application data directories:

| Data | Location |
|---|---|
| Profiles | `%APPDATA%\ProcessSetManager\Profiles` |
| Last restore session | `%APPDATA%\ProcessSetManager\last-session.json` |
| Active mode | `%APPDATA%\ProcessSetManager\mode-state.json` |
| Settings / protection list | `%APPDATA%\ProcessSetManager\settings.json` |
| Crash log | `%LOCALAPPDATA%\ProcessSetManager\crash.log` |

No external database is required.

## Build requirements

- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Build

Clone the repository:

```powershell
git clone https://github.com/netdaemon91/ProcessSetManager.git
cd ProcessSetManager
```

Regular release build:

```text
build.bat
```

Or publish a single self-contained executable:

```text
publish-single-exe.bat
```

The resulting executable is written to:

```text
bin\Release\net10.0-windows\win-x64\publish\ProcessSetManager.exe
```

The published build is `self-contained`, so the target machine does not need a separate .NET installation.

## Administrator rights

Normal user processes can usually be managed without elevation. If you want ProcessSet to terminate an application that itself runs as administrator, ProcessSet Manager may also need to be started with administrator rights.

## Project structure

```text
ProcessSetManager/
├─ Assets/
│  └─ ProcessSetManager.ico
├─ Models/
│  ├─ ProcessInfoRow.cs
│  └─ Profile.cs
├─ Services/
│  ├─ ExportService.cs
│  ├─ ProcessService.cs
│  └─ ProfileStore.cs
├─ AboutForm.cs
├─ MainForm.cs
├─ ModePreviewForm.cs
├─ Localization.cs
├─ Program.cs
├─ ProcessSetManager.csproj
├─ build.bat
└─ publish-single-exe.bat
```

## Contributing

Issues and pull requests are welcome. See [CONTRIBUTING.md](CONTRIBUTING.md) for details.

The project intentionally keeps a narrow scope: process profiles, modes, transparent previews, and restoration. Unsubstantiated “performance booster” features, aggressive registry tweaks, RAM cleaners, or automatic disabling of important Windows services are outside the project goals.

## License

ProcessSet Manager is released under the [MIT License](LICENSE).
