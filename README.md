# ProcessSet Manager

[![Version](https://img.shields.io/badge/Version-0.6.0-2ea44f)](#)
[![.NET](https://img.shields.io/badge/.NET-10-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![Windows](https://img.shields.io/badge/Windows-10%20%7C%2011-0078D4?logo=windows11&logoColor=white)](#)
[![C%23](https://img.shields.io/badge/C%23-WinForms-239120?logo=csharp&logoColor=white)](#)
[![Sprache](https://img.shields.io/badge/UI-Deutsch%20%7C%20English-F7DF1E)](#)
[![Lizenz](https://img.shields.io/badge/Lizenz-MIT-blue)](LICENSE)
[![Status](https://img.shields.io/badge/Status-in%20Entwicklung-orange)](#)

ProcessSet Manager ist ein schlanker Open-Source-Session- und Prozessprofil-Manager für Windows. Die Anwendung ermöglicht es, laufende Programme zu einem Profil zusammenzufassen, sie kontrolliert zu beenden und den vorherigen Zustand später soweit möglich wiederherzustellen.

> Aktueller Stand: **0.6.0**  
> Plattform: **Windows x64**  
> Framework: **.NET 10 / WinForms**  
> Lizenz: **MIT**

## Idee

Statt bei jedem Wechsel zwischen Arbeit, Gaming oder Streaming dieselben Programme einzeln zu schließen, können diese in ProcessSet Manager als Profil gespeichert werden.

Beispiel `Gaming`:

1. Photoshop, Thunderbird und OneDrive laufen.
2. Das Profil `Gaming` wird aktiviert.
3. ProcessSet zeigt vorab, welche Prozesse beendet und später wiederhergestellt werden können.
4. Die ausgewählten Programme werden nacheinander geschlossen.
5. Nach dem Spielen beendet man den Modus.
6. ProcessSet startet die zuvor gespeicherten Programme erneut.

ProcessSet Manager ist **kein PC-Booster** und nimmt keine aggressiven Registry-, RAM-, Dienst- oder Scheduler-Tweaks vor. Der Schwerpunkt liegt auf nachvollziehbaren Prozessprofilen und reproduzierbaren Session-Wechseln.

## Funktionen

- laufende Windows-Prozesse anzeigen
- Suche nach Prozessname, Fenstertitel, Pfad und Typ
- nur klassische Fenster-Apps oder auch Hintergrundprozesse anzeigen
- Programme per Checkbox auswählen
- Beendigungsreihenfolge frei festlegen
- Auswahl direkt als Modus aktivieren
- Profile speichern, laden, umbenennen, duplizieren und löschen
- Profile direkt über das System-Tray aktivieren
- aktiven Modus persistent speichern
- zuletzt beendete Programme soweit möglich wiederherstellen
- persönliche Schutzliste für Prozesse
- fest eingebaute Schutzregeln für kritische Windows-Prozesse
- Vorschau vor jeder Modus-Aktivierung
- Anzeige, ob ein Prozess automatisch wiederherstellbar ist (`↻`)
- Ergebnisübersicht nach Aktivierung und Wiederherstellung
- TXT-, PowerShell- und Batch-Export
- integrierte Hilfe und Tooltips
- Dark UI
- Per-Monitor-V2-DPI-Unterstützung
- Crash-Log bei unerwarteten Fehlern

## Wiederherstellung

Vor dem Aktivieren eines Modus prüft ProcessSet, welche betroffenen Programme tatsächlich laufen und ob ein lesbarer EXE-Pfad verfügbar ist.

Ein Prozess mit `↻ Ja` kann grundsätzlich später automatisch neu gestartet werden. `↻ Nein` bedeutet nicht, dass das Beenden fehlschlägt – lediglich die automatische Wiederherstellung ist nicht garantiert.

### Wichtige Grenze

ProcessSet startet Programme neu, rekonstruiert aber **nicht selbst deren internen Zustand**. Dazu gehören beispielsweise:

- offene Browser-Tabs
- nicht gespeicherte Dokumente
- geöffnete Photoshop-Projekte
- Fensterpositionen
- nicht gespeicherte Eingaben

Wenn die jeweilige Anwendung eine eigene Sitzungswiederherstellung besitzt, kann diese beim Neustart natürlich greifen.

## Schutzliste

Unter `Einstellungen` können zusätzliche Prozessnamen geschützt werden. Geschützte Prozesse werden von ProcessSet nicht beendet – auch dann nicht, wenn sie bereits in einem älteren Profil enthalten sind.

Kritische Windows-Prozesse sind zusätzlich fest im Programm geschützt.

## System-Tray

Standardmäßig kann ProcessSet beim Minimieren in den Windows-Infobereich wechseln. Über das Tray-Menü können unter anderem:

- gespeicherte Profile aktiviert werden
- der aktuelle Modus angezeigt werden
- der aktive Modus beendet und wiederhergestellt werden
- das Hauptfenster geöffnet werden
- die Anwendung beendet werden

## Export

Profile können zusätzlich exportiert werden als:

- `.txt` – reine Prozessnamen
- `.ps1` – PowerShell-Skript zum sequenziellen Beenden
- `.bat` – Batch-Skript zum Beenden der Prozesse

Damit bleibt die ursprüngliche Idee des Projekts auch unabhängig von der GUI nutzbar.

## Integrierte Hilfe

Version 0.6 enthält einen eigenen `Hilfe`-Tab. Dort werden Profil-Erstellung, Modus-Aktivierung, Wiederherstellung, Schutzliste, Tray-Modus und Export direkt in der Anwendung erklärt.

## Mehrsprachigkeit

Die Oberfläche unterstützt **Deutsch und Englisch**. Bei einer neuen Installation wird anhand der Windows-Sprache automatisch Deutsch oder Englisch gewählt. Unter `Einstellungen → Sprache / Language` kann die Sprache jederzeit geändert werden; die Änderung wird nach einem Neustart der Anwendung aktiv.

Profile, Schutzlisten und Wiederherstellungssitzungen bleiben dabei sprachunabhängig und können in beiden Oberflächensprachen weiterverwendet werden.

## Daten und Speicherorte

ProcessSet speichert Benutzerdaten im normalen Windows-Anwendungsdatenverzeichnis:

| Inhalt | Speicherort |
|---|---|
| Profile | `%APPDATA%\ProcessSetManager\Profiles` |
| letzte Wiederherstellungssitzung | `%APPDATA%\ProcessSetManager\last-session.json` |
| aktiver Modus | `%APPDATA%\ProcessSetManager\mode-state.json` |
| Einstellungen / Schutzliste | `%APPDATA%\ProcessSetManager\settings.json` |
| Crash-Log | `%LOCALAPPDATA%\ProcessSetManager\crash.log` |

Es wird keine externe Datenbank benötigt.

## Voraussetzungen zum Bauen

- Windows 10/11 x64
- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)

## Build

Repository klonen:

```powershell
git clone https://github.com/netdaemon91/ProcessSetManager.git
cd ProcessSetManager
```

Normaler Release-Build:

```text
build.bat
```

Oder direkt eine einzelne selbständige EXE erstellen:

```text
publish-single-exe.bat
```

Die veröffentlichte EXE liegt anschließend unter:

```text
bin\Release\net10.0-windows\win-x64\publish\ProcessSetManager.exe
```

Die Veröffentlichung ist `self-contained`; auf dem Zielsystem muss .NET daher nicht separat installiert sein.

## Administratorrechte

Normale Benutzerprozesse können in der Regel ohne erhöhte Rechte verwaltet werden. Soll ein Programm beendet werden, das selbst als Administrator läuft, muss ProcessSet Manager gegebenenfalls ebenfalls erhöht gestartet werden.

## Projektstruktur

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
├─ Program.cs
├─ ProcessSetManager.csproj
├─ build.bat
└─ publish-single-exe.bat
```

## Mitwirken

Issues und Pull Requests sind willkommen. Details stehen in [CONTRIBUTING.md](CONTRIBUTING.md).

Der Fokus des Projekts soll bewusst eng bleiben: Prozessprofile, Modi, transparente Vorschau und Wiederherstellung. Unbelegte „Performance-Booster“-Funktionen, aggressive Registry-Tweaks, RAM-Cleaner oder automatisches Abschalten wichtiger Windows-Dienste gehören nicht zum Projektziel.

## Lizenz

ProcessSet Manager steht unter der [MIT License](LICENSE).
