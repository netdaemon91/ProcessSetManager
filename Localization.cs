using System.Globalization;

namespace ProcessSetManager;

public static class Localization
{
    private static readonly Dictionary<string, string> English = new(StringComparer.Ordinal)
    {
        ["Prozessprofile aktivieren, den aktuellen PC-Zustand reduzieren und später wiederherstellen."] = "Activate process profiles, reduce the current PC workload, and restore it later.",
        ["Hilfe"] = "Help",
        ["Öffnet die integrierte Kurzanleitung."] = "Opens the integrated quick guide.",
        ["Versions- und Projektinformationen."] = "Version and project information.",
        ["Schnellprofile"] = "Quick profiles",
        ["Modus & Aktionen"] = "Mode & actions",
        ["Ausgewähltes Profil aktivieren"] = "Activate selected profile",
        ["Prüft den aktuellen Zustand und zeigt vor dem Beenden eine Vorschau."] = "Checks the current state and shows a preview before closing applications.",
        ["Modus beenden & wiederherstellen"] = "End mode & restore",
        ["Startet die beim Aktivieren gespeicherten Programme wieder und beendet den aktiven Modus."] = "Restarts the applications saved during activation and ends the active mode.",
        ["Letzte Sitzung erneut wiederherstellen"] = "Restore last session again",
        ["Neues Profil aus Prozessen erstellen"] = "Create new profile from processes",
        ["Ein aktiver Modus besitzt genau eine Wiederherstellungssitzung. Solange er aktiv ist, kann kein zweites Profil gestartet werden. So wird der vorherige Zustand nicht versehentlich überschrieben."] = "An active mode has exactly one restore session. While it is active, no second profile can be started. This prevents the previous state from being overwritten accidentally.",
        ["Prozesse"] = "Processes",
        ["Suche:"] = "Search:",
        ["Nur Programme mit Fenster"] = "Only applications with a window",
        ["Aktualisieren"] = "Refresh",
        ["Auswahl / Reihenfolge"] = "Selection / order",
        ["▲ Hoch"] = "▲ Up",
        ["▼ Runter"] = "▼ Down",
        ["Entfernen"] = "Remove",
        ["Als Profil speichern"] = "Save as profile",
        ["Speichert die aktuelle Auswahl und Reihenfolge als wiederverwendbares Profil."] = "Saves the current selection and order as a reusable profile.",
        ["Auswahl als Modus aktivieren"] = "Activate selection as mode",
        ["Zeigt zuerst eine Vorschau und beendet danach die ausgewählten Programme."] = "Shows a preview first and then closes the selected applications.",
        ["Profile"] = "Profiles",
        ["Gespeicherte Profile"] = "Saved profiles",
        ["In Prozessauswahl laden"] = "Load into process selection",
        ["Aktivieren"] = "Activate",
        ["Umbenennen"] = "Rename",
        ["Duplizieren"] = "Duplicate",
        ["Löschen"] = "Delete",
        ["Profil bearbeiten / exportieren"] = "Edit / export profile",
        ["Profilname"] = "Profile name",
        ["Aktuelle Auswahl speichern"] = "Save current selection",
        ["Export"] = "Export",
        ["Die Exporte verwenden weiterhin das ursprüngliche Prinzip: Prozessnamen werden gespeichert bzw. als direkt ausführbares PowerShell-/Batch-Skript ausgegeben."] = "Exports keep the original principle: process names are saved or written as directly executable PowerShell/Batch scripts.",
        ["Einstellungen"] = "Settings",
        ["Schutzliste"] = "Protection list",
        ["Diese Prozessnamen werden von ProcessSet niemals beendet. \".exe\" darf beim Eingeben mit angegeben werden."] = "ProcessSet will never terminate these process names. You may include \".exe\" when entering a name.",
        ["Hinzufügen"] = "Add",
        ["Aktuelle Prozessauswahl schützen"] = "Protect current process selection",
        ["Nimmt die momentan ausgewählten Prozessnamen in die Schutzliste auf."] = "Adds the currently selected process names to the protection list.",
        ["Allgemein"] = "General",
        ["Beim Minimieren in den Infobereich verschieben"] = "Minimize to notification area",
        ["Immer geschützte Windows-Prozesse"] = "Always protected Windows processes",
        ["Sprache / Language"] = "Language / Sprache",
        ["Sprachänderungen werden nach einem Neustart der Anwendung wirksam."] = "Language changes take effect after restarting the application.",
        ["Verlauf"] = "History",
        ["Bereit"] = "Ready",
        ["Profil aktivieren"] = "Activate profile",
        ["(keine Profile)"] = "(no profiles)",
        ["Fenster öffnen"] = "Open window",
        ["Beenden"] = "Exit",
        ["Prozess"] = "Process",
        ["Typ"] = "Type",
        ["Schutz"] = "Protection",
        ["Fenstertitel"] = "Window title",
        ["Pfad"] = "Path",
        ["Hintergrund"] = "Background",
        ["Ja"] = "Yes",
        ["Nein"] = "No",
        ["Ja = ein lesbarer EXE-Pfad ist vorhanden und der Prozess kann grundsätzlich automatisch neu gestartet werden."] = "Yes = a readable EXE path is available and the process can generally be restarted automatically.",
        ["Geschützte Prozesse werden von ProcessSet niemals beendet."] = "Protected processes are never terminated by ProcessSet.",
        ["Keine Auswahl"] = "No selection",
        ["Profil speichern"] = "Save profile",
        ["Profil"] = "Profile",
        ["Modus bereits aktiv"] = "Mode already active",
        ["Nichts zu beenden"] = "Nothing to close",
        ["Modus aktivieren"] = "Activate mode",
        ["Manuelle Auswahl"] = "Manual selection",
        ["Wiederherstellen"] = "Restore",
        ["Modus beenden"] = "End mode",
        ["Modus"] = "Mode",
        ["Profil aktiv"] = "Profile active",
        ["Profil laden"] = "Load profile",
        ["Profil umbenennen"] = "Rename profile",
        ["Profil duplizieren"] = "Duplicate profile",
        ["Profil löschen"] = "Delete profile",
        ["Bereits geschützt"] = "Already protected",
        ["Modus aktiviert – Ergebnis"] = "Mode activated – result",
        ["Wiederherstellung – Ergebnis"] = "Restore – result",
        ["Fehlerdetails:"] = "Error details:",
        ["Details:"] = "Details:",
        ["Profilname:"] = "Profile name:",
        ["Abbrechen"] = "Cancel",
        ["Speichern"] = "Save",
        ["Aktueller Zustand"] = "Current state",
        ["Wiederherstellung"] = "Restore",
        ["Instanzen"] = "Instances",
        ["Geschützt – wird übersprungen"] = "Protected – skipped",
        ["Nicht nötig"] = "Not needed",
        ["Läuft nicht"] = "Not running",
        ["Wird beendet"] = "Will be closed",
        ["Nein – kein lesbarer EXE-Pfad"] = "No – no readable EXE path",
        ["Deutsch"] = "Deutsch",
        ["English"] = "English",
        ["ProcessSet Manager gestartet."] = "ProcessSet Manager started.",
        ["ProcessSet Manager läuft im Infobereich weiter."] = "ProcessSet Manager continues running in the notification area.",
        ["Prozesse werden geladen …"] = "Loading processes …",
        ["Bitte zuerst Programme auswählen."] = "Please select applications first.",
        ["Keine Profile gespeichert."] = "No profiles saved.",
        ["noch nie"] = "never",
        ["Bitte zuerst ein Profil auswählen."] = "Please select a profile first.",
        ["Beende und stelle diesen Modus zuerst wieder her, bevor ein anderes Profil aktiviert wird."] = "End and restore this mode before activating another profile.",
        ["Von diesem Profil läuft aktuell kein beendbarer Prozess. Der Modus wurde daher nicht aktiviert."] = "No closable process from this profile is currently running. The mode was not activated.",
        ["Es ist bereits ein Modus aktiv. Beende ihn zuerst, damit die Wiederherstellungssitzung nicht überschrieben wird."] = "A mode is already active. End it first so the restore session is not overwritten.",
        ["Von der aktuellen Auswahl läuft kein beendbarer Prozess. Der Modus wurde daher nicht aktiviert."] = "No closable process from the current selection is running. The mode was not activated.",
        ["Es ist keine Wiederherstellungssitzung gespeichert."] = "No restore session is saved.",
        ["Für diesen Modus konnten keine Programme mit einem wiederstartbaren EXE-Pfad gespeichert werden.\n\nDen Modus trotzdem beenden?"] = "No applications with a restartable EXE path could be saved for this mode.\n\nEnd the mode anyway?",
        ["Die letzte Sitzung enthält keine wiederherstellbaren Programme."] = "The last session contains no restorable applications.",
        ["Wähle links ein Profil und aktiviere es, oder erstelle im Tab „Prozesse“ eine manuelle Auswahl."] = "Select a profile on the left and activate it, or create a manual selection in the “Processes” tab.",
        ["Das aktuell aktive Profil kann erst nach dem Beenden des Modus umbenannt werden."] = "The currently active profile can only be renamed after ending the mode.",
        ["Das aktuell aktive Profil kann erst nach dem Beenden des Modus gelöscht werden."] = "The currently active profile can only be deleted after ending the mode.",
        ["Kein Profil bzw. keine Auswahl zum Exportieren vorhanden."] = "No profile or selection is available for export.",
        ["Es sind aktuell keine Programme ausgewählt."] = "No applications are currently selected.",
        ["Wiederherstellung abgeschlossen."] = "Restore completed.",
        ["Bitte einen Profilnamen eingeben."] = "Please enter a profile name.",
        ["Hinweis: Programme ohne lesbaren EXE-Pfad können beendet, aber von ProcessSet danach nicht automatisch neu gestartet werden."] = "Note: Applications without a readable EXE path can be closed, but ProcessSet cannot restart them automatically afterwards.",
        ["Die Wiederherstellung startet Programme neu. Offene Dokumente, Tabs oder interne Sitzungen kann nur die jeweilige Anwendung selbst wiederherstellen."] = "Restore restarts applications. Only the application itself can restore open documents, tabs, or internal sessions.",
        ["Sprache geändert"] = "Language changed",
        ["Die neue Sprache wird nach einem Neustart verwendet. ProcessSet Manager jetzt neu starten?"] = "The new language will be used after a restart. Restart ProcessSet Manager now?",
        ["Textdatei (*.txt)|*.txt"] = "Text file (*.txt)|*.txt",
        ["Batch-Datei (*.bat)|*.bat"] = "Batch file (*.bat)|*.bat",
        ["PowerShell-Skript (*.ps1)|*.ps1"] = "PowerShell script (*.ps1)|*.ps1",
        ["Das Profil benötigt einen Namen."] = "The profile requires a name.",
        ["Das Profil wurde nicht gefunden."] = "The profile was not found.",
        ["Das Profil konnte nicht gelesen werden."] = "The profile could not be read.",
        ["Der neue Profilname darf nicht leer sein."] = "The new profile name must not be empty.",
        ["Ein Profil mit diesem Namen existiert bereits."] = "A profile with this name already exists.",
        ["Ungültiger Profilname."] = "Invalid profile name.",
        ["ProcessSet Manager konnte nicht gestartet bzw. weiter ausgeführt werden.\n\n"] = "ProcessSet Manager could not be started or continue running.\n\n",
        ["ProcessSet Manager – Fehler"] = "ProcessSet Manager – Error",

        ["{0}: geschützt, übersprungen"] = "{0}: protected, skipped",
        ["{0}: läuft nicht"] = "{0}: not running",
        ["Beende {0} (PID {1}) …"] = "Closing {0} (PID {1}) …",
        ["{0} (PID {1}): Fehler – {2}"] = "{0} (PID {1}): error – {2}",
        ["{0}: Fehler – {1}"] = "{0}: error – {1}",
        ["{0}: Prozess läuft weiterhin."] = "{0}: process is still running.",
        ["{0}: läuft bereits"] = "{0}: already running",
        ["{0}: Datei nicht gefunden"] = "{0}: file not found",
        ["{0}: gestartet"] = "{0}: started",
        ["Vorschau – {0}"] = "Preview – {0}",
        ["Folgendes würde beim Aktivieren von „{0}“ passieren:"] = "The following would happen when activating “{0}”:",
        ["{0} laufende Programme würden beendet · {1} davon automatisch wiederherstellbar · {2} nicht automatisch wiederherstellbar · {3} geschützt · {4} laufen derzeit nicht"] = "{0} running applications would be closed · {1} automatically restorable · {2} not automatically restorable · {3} protected · {4} currently not running",
        ["\n\nFehlerdetails wurden gespeichert unter:\n{0}"] = "\n\nError details were saved to:\n{0}",
        ["Vibe Coded by NetDaemon  ·  Version {0}"] = "Vibe Coded by NetDaemon  ·  Version {0}",
        ["Konfiguration:\n{0}"] = "Configuration:\n{0}",
        ["● Aktiv: {0}"] = "● Active: {0}",
        ["○ Kein aktiver Modus"] = "○ No active mode",
        ["{0} Prozesse geladen | {1} ausgewählt"] = "{0} processes loaded | {1} selected",
        ["{0} Prozesse eingelesen."] = "{0} processes read.",
        ["{0} steht auf der Schutzliste."] = "{0} is on the protection list.",
        ["{0} Programme ausgewählt"] = "{0} applications selected",
        ["Profil '{0}' gespeichert"] = "Profile '{0}' saved",
        ["Profil '{0}' gespeichert."] = "Profile '{0}' saved.",
        ["{0} Programme · zuletzt aktiviert: {1}"] = "{0} applications · last activated: {1}",
        ["Der Modus '{0}' ist bereits aktiv.\n\n"] = "Mode '{0}' is already active.\n\n",
        ["Modus '{0}' ist aktiv"] = "Mode '{0}' is active",
        ["Modus '{0}' aktiviert. Beendet: {1}, geschützt: {2}, Fehler: {3}, wiederherstellbar: {4}."] = "Mode '{0}' activated. Closed: {1}, protected: {2}, errors: {3}, restorable: {4}.",
        ["Temporärer Modus aktiviert. Beendet: {0}, Fehler: {1}, wiederherstellbar: {2}."] = "Temporary mode activated. Closed: {0}, errors: {1}, restorable: {2}.",
        ["Sitzung vom {0:g} wiederherstellen?\n\n"] = "Restore session from {0:g}?\n\n",
        ["Wiederherstellung abgeschlossen. Gestartet: {0}, bereits aktiv: {1}, Fehler: {2}."] = "Restore completed. Started: {0}, already running: {1}, errors: {2}.",
        ["{0} beendet"] = "{0} ended",
        ["{0} beendet."] = "{0} ended.",
        ["● {0} aktiv"] = "● {0} active",
        ["Aktiv seit {0} · {1} Programme wiederherstellbar"] = "Active since {0} · {1} applications restorable",
        ["unbekannt"] = "unknown",
        [" · zuletzt {0:g}"] = " · last {0:g}",
        ["{0}: {1} Programme{2}"] = "{0}: {1} applications{2}",
        ["{0}: wegen Schutzliste nicht in die Auswahl übernommen."] = "{0}: not added to the selection because it is protected.",
        ["Profil '{0}' in Auswahl geladen"] = "Profile '{0}' loaded into selection",
        ["Profil '{0}' in '{1}' umbenannt."] = "Profile '{0}' renamed to '{1}'.",
        ["Kopie"] = "Copy",
        ["Profil '{0}' als '{1}' dupliziert."] = "Profile '{0}' duplicated as '{1}'.",
        ["Profil '{0}' wirklich löschen?"] = "Really delete profile '{0}'?",
        ["Profil '{0}' gelöscht."] = "Profile '{0}' deleted.",
        ["Exportiert: {0}"] = "Exported: {0}",
        ["Profil '{0}' als {1} exportiert."] = "Profile '{0}' exported as {1}.",
        ["{0} ist bereits fest durch ProcessSet geschützt."] = "{0} is already permanently protected by ProcessSet.",
        ["{0} zur Schutzliste hinzugefügt."] = "{0} added to the protection list.",
        ["{0} aus Schutzliste entfernt."] = "{0} removed from the protection list.",
        ["{0} Prozessnamen aus der aktuellen Auswahl geschützt."] = "{0} process names from the current selection protected.",
        ["Einstellungen konnten nicht gespeichert werden: {0}"] = "Settings could not be saved: {0}",
        ["Modus „{0}“ ist aktiv."] = "Mode “{0}” is active.",
        ["Beendet: {0}"] = "Closed: {0}",
        ["Davon erzwungen beendet: {0}"] = "Force-closed: {0}",
        ["Liefen nicht: {0}"] = "Not running: {0}",
        ["Geschützt / übersprungen: {0}"] = "Protected / skipped: {0}",
        ["Fehler: {0}"] = "Errors: {0}",
        ["Für Wiederherstellung gespeichert: {0}"] = "Saved for restore: {0}",
        ["… und {0} weitere."] = "… and {0} more.",
        ["Gestartet: {0}"] = "Started: {0}",
        ["Liefen bereits: {0}"] = "Already running: {0}",
        ["EXE nicht gefunden: {0}"] = "EXE not found: {0}",
        ["Der aktuelle Modus ist bereits aktiv."] = "The current mode is already active."
    };

    public static string CurrentCode { get; private set; } = DetectDefaultLanguage();
    public static bool IsEnglish => CurrentCode == "en";

    public static string NormalizeLanguage(string? language) =>
        string.Equals(language, "en", StringComparison.OrdinalIgnoreCase) ? "en" : "de";

    public static void SetLanguage(string? language)
    {
        CurrentCode = NormalizeLanguage(language);
        var culture = IsEnglish
            ? CultureInfo.GetCultureInfo("en-US")
            : CultureInfo.GetCultureInfo("de-DE");

        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
    }

    public static string T(string german)
    {
        if (!IsEnglish)
            return german;

        return English.TryGetValue(german, out var english) ? english : german;
    }

    public static string F(string germanFormat, params object?[] args) =>
        string.Format(CultureInfo.CurrentCulture, T(germanFormat), args);

    public static string HelpText => IsEnglish ? HelpEnglish : HelpGerman;

    private static string DetectDefaultLanguage() =>
        CultureInfo.CurrentUICulture.TwoLetterISOLanguageName.Equals("de", StringComparison.OrdinalIgnoreCase)
            ? "de"
            : "en";

    private const string HelpGerman =
        "PROCESSSET MANAGER – KURZANLEITUNG\r\n\r\n" +
        "1. Profil erstellen\r\n" +
        "Im Tab „Prozesse“ die gewünschten Programme anhaken. Rechts wird die Reihenfolge angezeigt. Mit Hoch/Runter kannst du sie ändern. Danach „Als Profil speichern“ wählen.\r\n\r\n" +
        "2. Modus aktivieren\r\n" +
        "Im Dashboard oder unter „Profile“ ein Profil auswählen und aktivieren. Vor dem Beenden erscheint eine Vorschau. Dort siehst du, welche Programme gerade laufen, welche geschützt sind und welche später automatisch wieder gestartet werden können.\r\n\r\n" +
        "3. Modus beenden & wiederherstellen\r\n" +
        "Solange ein Modus aktiv ist, speichert ProcessSet genau eine Wiederherstellungssitzung. „Modus beenden & wiederherstellen“ startet die zuvor erkannten Programme wieder. Offene Dokumente, Tabs oder interne Sitzungen kann nur das jeweilige Programm selbst wiederherstellen.\r\n\r\n" +
        "4. Bedeutung der Spalte ↻\r\n" +
        "„Ja“ bedeutet: ProcessSet kann für diesen laufenden Prozess einen lesbaren EXE-Pfad ermitteln und ihn daher grundsätzlich später neu starten. „Nein“ bedeutet nicht, dass das Beenden fehlschlägt – nur die automatische Wiederherstellung ist nicht garantiert.\r\n\r\n" +
        "5. Schutzliste\r\n" +
        "Unter „Einstellungen“ kannst du Prozessnamen schützen. Geschützte Programme werden von ProcessSet nicht beendet – auch dann nicht, wenn sie in einem älteren Profil gespeichert sind.\r\n\r\n" +
        "6. System-Tray\r\n" +
        "Wenn „Beim Minimieren in den Infobereich verschieben“ aktiv ist, läuft ProcessSet neben der Uhr weiter. Per Rechtsklick auf das Tray-Icon kannst du Profile aktivieren, den aktuellen Modus wiederherstellen oder das Hauptfenster öffnen.\r\n\r\n" +
        "7. Export\r\n" +
        "Profile können weiterhin als TXT, PowerShell- oder Batch-Datei exportiert werden. Diese Exporte verwenden das ursprüngliche Prinzip und arbeiten mit Prozessnamen.\r\n\r\n" +
        "8. Sprache\r\n" +
        "Unter „Einstellungen“ kann zwischen Deutsch und English gewechselt werden. Die neue Sprache wird nach einem Neustart aktiv.\r\n\r\n" +
        "SICHERHEIT / GRENZEN\r\n" +
        "ProcessSet versucht Programme zunächst normal zu schließen und erzwingt das Beenden erst nach einem Timeout. Programme mit ungespeicherten Daten können beim erzwungenen Beenden Daten verlieren. Kritische Windows-Prozesse sind grundsätzlich geschützt. Für erhöht gestartete Programme kann ProcessSet selbst Administratorrechte benötigen.\r\n\r\n" +
        "Tipp: Wenn du unsicher bist, prüfe die Vorschau. Besonders wichtig ist die Zeile „nicht automatisch wiederherstellbar“.";

    private const string HelpEnglish =
        "PROCESSSET MANAGER – QUICK GUIDE\r\n\r\n" +
        "1. Create a profile\r\n" +
        "In the “Processes” tab, check the applications you want. Their order is shown on the right. Use Up/Down to change it, then choose “Save as profile”.\r\n\r\n" +
        "2. Activate a mode\r\n" +
        "Select a profile on the Dashboard or in “Profiles” and activate it. A preview appears before applications are closed. It shows which applications are running, protected, and automatically restorable later.\r\n\r\n" +
        "3. End mode & restore\r\n" +
        "While a mode is active, ProcessSet keeps exactly one restore session. “End mode & restore” restarts the applications detected during activation. Only the application itself can restore open documents, tabs, or internal sessions.\r\n\r\n" +
        "4. Meaning of the ↻ column\r\n" +
        "“Yes” means ProcessSet can determine a readable EXE path for the running process and can therefore generally restart it later. “No” does not mean closing will fail – only automatic restore is not guaranteed.\r\n\r\n" +
        "5. Protection list\r\n" +
        "Under “Settings” you can protect process names. Protected applications are never closed by ProcessSet, even if they are stored in an older profile.\r\n\r\n" +
        "6. System tray\r\n" +
        "When “Minimize to notification area” is enabled, ProcessSet keeps running next to the clock. Right-click the tray icon to activate profiles, restore the current mode, or reopen the main window.\r\n\r\n" +
        "7. Export\r\n" +
        "Profiles can still be exported as TXT, PowerShell, or Batch files. These exports use the original process-name based approach.\r\n\r\n" +
        "8. Language\r\n" +
        "Under “Settings” you can switch between Deutsch and English. The new language becomes active after restarting the application.\r\n\r\n" +
        "SAFETY / LIMITS\r\n" +
        "ProcessSet first tries to close applications normally and only forces termination after a timeout. Applications with unsaved data may lose data when force-closed. Critical Windows processes are always protected. ProcessSet may itself require administrator rights to manage elevated applications.\r\n\r\n" +
        "Tip: If you are unsure, review the preview. Pay special attention to applications marked as not automatically restorable.";
}
