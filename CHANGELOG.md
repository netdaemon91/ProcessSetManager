# Changelog

Alle nennenswerten Änderungen an ProcessSet Manager werden in dieser Datei dokumentiert.

## [0.5.0] - 2026-09-25

### Neu

- integrierte Bedienungsanleitung im Tab **Hilfe**
- Hilfe-Button im Kopfbereich
- Tooltips für zentrale Aktionen
- Vorschau vor der Aktivierung eines Profils oder einer manuellen Auswahl
- Vorschau zeigt Laufstatus, Schutzstatus, Instanzzahl und Wiederherstellbarkeit
- Prozessspalte `↻` für die automatische Wiederherstellbarkeit
- Ergebnisübersicht nach Modus-Aktivierung
- Ergebnisübersicht nach Wiederherstellung
- erzwungene Beendigungen werden separat gezählt
- kein leerer Modus, wenn kein beendbarer Prozess läuft

## [0.4.0] - 2026-09-25

### Neu

- Profile werden als aktive Modi behandelt
- aktiver Modus blockiert weitere Aktivierungen bis zur Wiederherstellung
- aktiver Modus wird persistent gespeichert
- System-Tray mit Profilen, Status, Wiederherstellung, Fenster öffnen und Beenden
- optionales Minimieren in den Infobereich
- persönliche Schutzliste für Prozesse
- Schutzstatus in der Prozessübersicht
- Profile umbenennen und duplizieren
- Dashboard zeigt Programmzahl und letzte Aktivierung

## [0.3.0] - 2026-09-25

### Behoben

- möglicher Sofortabsturz beim Erzeugen von `SplitContainer`-Controls
- robustere initiale Splitterpositionierung
- eingebettetes Multi-Resolution-Programmicon
- globale Fehlerbehandlung und Crash-Log

## [0.2.0] - 2026-09-25

### Geändert

- responsive WinForms-Layouts
- Per-Monitor-V2-DPI-Skalierung
- flexiblere Buttons und Aktionsleisten
- bessere Darstellung bei Windows-Skalierung über 100 %

## [0.1.0] - 2026-09-25

### Erstversion

- C#/.NET-MVP auf Basis des ursprünglichen PowerShell-Prototyps
- Prozessauswahl und Reihenfolge
- Profile
- Beenden und Wiederherstellen
- TXT-, PowerShell- und Batch-Export
- Dark UI
- About-Dialog
