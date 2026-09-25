# Mitwirken an ProcessSet Manager

Beiträge sind willkommen.

## Fehler melden

Bitte möglichst angeben:

- verwendete ProcessSet-Manager-Version
- Windows-Version
- Schritte zum Reproduzieren
- erwartetes Verhalten
- tatsächliches Verhalten
- bei Abstürzen relevante Zeilen aus `%LOCALAPPDATA%\ProcessSetManager\crash.log`

Bei Darstellungsproblemen bitte zusätzlich die Windows-Anzeigeskalierung nennen, z. B. 100 %, 125 % oder 150 %.

## Pull Requests

Bitte Pull Requests möglichst klein und thematisch klar halten. Neue Funktionen sollten zum Kernziel des Projekts passen: nachvollziehbare Prozessprofile, Modi und Session-Wiederherstellung.

Nicht zum Projektziel gehören aggressive Registry-Tweaks, RAM-Cleaner, automatische Deaktivierung wichtiger Windows-Dienste oder unbelegte „Performance-Booster“-Funktionen.

## Code-Stil

- vorhandenen C#-Stil beibehalten
- nullable reference types berücksichtigen
- Fehler eines einzelnen Prozesses dürfen möglichst nicht die komplette Anwendung abbrechen
- UI-Änderungen DPI-freundlich und ohne unnötige feste Pixelpositionierung umsetzen
