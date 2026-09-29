using ProcessSetManager.Models;
using ProcessSetManager.Services;

namespace ProcessSetManager;

public sealed class MainForm : Form
{
    private readonly ProcessService _processService = new();
    private readonly ProfileStore _profileStore = new();

    private readonly List<ProcessInfoRow> _allProcesses = new();
    private readonly List<string> _selectedOrder = new();

    private readonly TabControl _tabs = new();
    private readonly DataGridView _processGrid = new();
    private readonly ListBox _selectedList = new();
    private readonly TextBox _searchBox = new();
    private readonly CheckBox _onlyApps = new();
    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _statusLabel = new();

    private readonly ListBox _dashboardProfiles = new();
    private readonly Label _dashboardProfileDetails = new();
    private readonly Label _modeStatusLabel = new();
    private readonly Label _modeDetailsLabel = new();
    private readonly Button _modeActionButton = new();

    private readonly ListBox _manageProfiles = new();
    private readonly TextBox _profileName = new();
    private readonly TextBox _logBox = new();

    private readonly ListBox _protectedList = new();
    private readonly TextBox _protectInput = new();
    private readonly CheckBox _minimizeToTray = new();
    private readonly ComboBox _languageBox = new();

    private readonly NotifyIcon _trayIcon = new();
    private readonly ContextMenuStrip _trayMenu = new();
    private readonly ToolTip _toolTip = new() { AutoPopDelay = 8000, InitialDelay = 450, ReshowDelay = 100 };

    private AppSettings _settings;
    private ModeState _modeState;

    private bool _updatingChecks;
    private bool _trayBalloonShown;

    private static readonly Color Bg = Color.FromArgb(24, 26, 31);
    private static readonly Color Panel = Color.FromArgb(31, 34, 40);
    private static readonly Color Panel2 = Color.FromArgb(38, 42, 49);
    private static readonly Color TextColor = Color.FromArgb(235, 238, 242);
    private static readonly Color Muted = Color.FromArgb(170, 176, 185);
    private static readonly Color Danger = Color.FromArgb(190, 66, 66);
    private static readonly Color Active = Color.FromArgb(91, 201, 128);

    public MainForm()
    {
        _settings = _profileStore.LoadSettings();
        Localization.SetLanguage(_settings.Language);
        _modeState = _profileStore.LoadModeState();

        Text = "ProcessSet Manager";
        StartPosition = FormStartPosition.CenterScreen;
        MinimumSize = new Size(980, 640);
        Size = new Size(1280, 780);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Bg;
        ForeColor = TextColor;
        Font = new Font("Segoe UI", 9);

        TryApplyApplicationIcon(this);

        BuildHeader();
        BuildTabs();
        BuildStatus();
        BuildTray();

        Load += (_, _) =>
        {
            RefreshProcesses();
            RefreshProfileLists();
            LoadSettingsIntoUi();
            RefreshModeUi();
            Log(Localization.T("ProcessSet Manager gestartet."));
        };

        Resize += (_, _) =>
        {
            if (WindowState == FormWindowState.Minimized && _settings.MinimizeToTray)
            {
                if (IsHandleCreated && !IsDisposed)
                {
                    BeginInvoke((Action)(() =>
                    {
                        if (IsDisposed || WindowState != FormWindowState.Minimized)
                            return;

                        Hide();

                        if (!_trayBalloonShown)
                    {
                        _trayBalloonShown = true;
                        _trayIcon.BalloonTipTitle = "ProcessSet Manager";
                        _trayIcon.BalloonTipText = Localization.T("ProcessSet Manager läuft im Infobereich weiter.");
                            _trayIcon.ShowBalloonTip(2500);
                        }
                    }));
                }
            }
        };

        FormClosed += (_, _) =>
        {
            _trayIcon.Visible = false;
            _trayIcon.Dispose();
            _trayMenu.Dispose();
        };
    }

    private void BuildHeader()
    {
        var header = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Panel,
            Padding = new Padding(18, 10, 18, 10)
        };

        header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

        var textStack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty
        };

        textStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        textStack.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var title = new Label
        {
            Text = "ProcessSet Manager",
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 18),
            ForeColor = TextColor,
            Margin = new Padding(0, 0, 0, 2)
        };

        var subtitle = new Label
        {
            Text = Localization.T("Prozessprofile aktivieren, den aktuellen PC-Zustand reduzieren und später wiederherstellen."),
            AutoSize = true,
            ForeColor = Muted,
            Margin = Padding.Empty
        };

        textStack.Controls.Add(title, 0, 0);
        textStack.Controls.Add(subtitle, 0, 1);

        var headerButtons = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Anchor = AnchorStyles.Right,
            Margin = new Padding(16, 2, 0, 2),
            BackColor = Panel
        };

        var help = MakeButton("Hilfe", 92);
        help.Click += (_, _) =>
        {
            var helpIndex = _tabs.TabPages.IndexOfKey("HelpTab");
            if (helpIndex >= 0)
                _tabs.SelectedIndex = helpIndex;
        };
        _toolTip.SetToolTip(help, Localization.T("Öffnet die integrierte Kurzanleitung."));

        var about = MakeButton("About", 92);
        about.Click += (_, _) => new AboutForm().ShowDialog(this);
        _toolTip.SetToolTip(about, Localization.T("Versions- und Projektinformationen."));

        headerButtons.Controls.Add(help);
        headerButtons.Controls.Add(about);

        header.Controls.Add(textStack, 0, 0);
        header.Controls.Add(headerButtons, 1, 0);

        Controls.Add(header);
    }

    private void BuildTabs()
    {
        _tabs.Dock = DockStyle.Fill;
        _tabs.Appearance = TabAppearance.Normal;
        _tabs.Padding = new Point(14, 6);

        _tabs.TabPages.Add(BuildDashboardTab());
        _tabs.TabPages.Add(BuildProcessesTab());
        _tabs.TabPages.Add(BuildProfilesTab());
        _tabs.TabPages.Add(BuildSettingsTab());
        _tabs.TabPages.Add(BuildHelpTab());
        _tabs.TabPages.Add(BuildLogTab());

        Controls.Add(_tabs);
        _tabs.BringToFront();
    }

    private TabPage BuildDashboardTab()
    {
        var page = MakePage("Dashboard");
        page.Padding = new Padding(18);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Bg,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 42));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 58));

        var profilesGroup = MakeGroup("Schnellprofile");
        profilesGroup.Dock = DockStyle.Fill;
        profilesGroup.Margin = new Padding(0, 0, 12, 0);

        var profileLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Bg,
            Margin = Padding.Empty
        };
        profileLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        profileLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        _dashboardProfiles.Dock = DockStyle.Fill;
        StyleListBox(_dashboardProfiles);
        _dashboardProfiles.SelectedIndexChanged += (_, _) => UpdateDashboardProfileDetails();

        _dashboardProfileDetails.AutoSize = true;
        _dashboardProfileDetails.ForeColor = Muted;
        _dashboardProfileDetails.Margin = new Padding(0, 8, 0, 0);

        profileLayout.Controls.Add(_dashboardProfiles, 0, 0);
        profileLayout.Controls.Add(_dashboardProfileDetails, 0, 1);
        profilesGroup.Controls.Add(profileLayout);

        var actionsGroup = MakeGroup("Modus & Aktionen");
        actionsGroup.Dock = DockStyle.Fill;
        actionsGroup.Margin = new Padding(12, 0, 0, 0);

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            Padding = new Padding(10),
            BackColor = Bg
        };

        _modeStatusLabel.AutoSize = true;
        _modeStatusLabel.Font = new Font("Segoe UI Semibold", 14);
        _modeStatusLabel.ForeColor = Muted;
        _modeStatusLabel.Margin = new Padding(0, 0, 0, 4);

        _modeDetailsLabel.AutoSize = true;
        _modeDetailsLabel.ForeColor = Muted;
        _modeDetailsLabel.MaximumSize = new Size(560, 0);
        _modeDetailsLabel.Margin = new Padding(0, 0, 0, 18);

        var execute = MakeButton("Ausgewähltes Profil aktivieren", 270);
        execute.Click += async (_, _) => await ExecuteSelectedProfileAsync();
        _toolTip.SetToolTip(execute, Localization.T("Prüft den aktuellen Zustand und zeigt vor dem Beenden eine Vorschau."));

        ConfigureButton(_modeActionButton, Localization.T("Modus beenden & wiederherstellen"), 285, danger: false);
        _modeActionButton.Click += (_, _) => RestoreLastSession(fromActiveMode: true);
        _toolTip.SetToolTip(_modeActionButton, Localization.T("Startet die beim Aktivieren gespeicherten Programme wieder und beendet den aktiven Modus."));

        var restore = MakeButton("Letzte Sitzung erneut wiederherstellen", 285);
        restore.Click += (_, _) => RestoreLastSession(fromActiveMode: false);

        var openProcesses = MakeButton("Neues Profil aus Prozessen erstellen", 285);
        openProcesses.Click += (_, _) => _tabs.SelectedIndex = 1;

        var note = MakeLabel(
            "Ein aktiver Modus besitzt genau eine Wiederherstellungssitzung. Solange er aktiv ist, kann kein zweites Profil gestartet werden. " +
            "So wird der vorherige Zustand nicht versehentlich überschrieben.",
            9,
            FontStyle.Regular);
        note.ForeColor = Muted;
        note.MaximumSize = new Size(560, 0);
        note.Margin = new Padding(0, 18, 0, 0);

        actions.Controls.Add(_modeStatusLabel);
        actions.Controls.Add(_modeDetailsLabel);
        actions.Controls.Add(execute);
        actions.Controls.Add(_modeActionButton);
        actions.Controls.Add(restore);
        actions.Controls.Add(openProcesses);
        actions.Controls.Add(note);

        actionsGroup.Controls.Add(actions);

        root.Controls.Add(profilesGroup, 0, 0);
        root.Controls.Add(actionsGroup, 1, 0);
        page.Controls.Add(root);

        return page;
    }

    private TabPage BuildProcessesTab()
    {
        var page = MakePage("Prozesse");
        page.Padding = new Padding(10);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Bg,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var top = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Bg,
            Padding = new Padding(0, 0, 0, 8),
            Margin = Padding.Empty
        };

        var searchLabel = MakeLabel("Suche:", 9, FontStyle.Regular);
        searchLabel.Margin = new Padding(0, 8, 6, 0);

        _searchBox.Width = 280;
        _searchBox.Margin = new Padding(0, 4, 14, 4);
        StyleTextBox(_searchBox);
        _searchBox.TextChanged += (_, _) => RefreshProcessGrid();

        _onlyApps.Text = Localization.T("Nur Programme mit Fenster");
        _onlyApps.AutoSize = true;
        _onlyApps.ForeColor = TextColor;
        _onlyApps.Margin = new Padding(0, 7, 14, 0);
        _onlyApps.CheckedChanged += (_, _) => RefreshProcessGrid();

        var refresh = MakeButton("Aktualisieren", 120);
        refresh.Margin = new Padding(0, 2, 0, 2);
        refresh.Click += (_, _) => RefreshProcesses();

        top.Controls.Add(searchLabel);
        top.Controls.Add(_searchBox);
        top.Controls.Add(_onlyApps);
        top.Controls.Add(refresh);

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterWidth = 6,
            BackColor = Color.FromArgb(59, 64, 74),
            Margin = Padding.Empty
        };

        ConfigureInitialSplitter(split, 0.68);

        BuildProcessGrid();
        split.Panel1.Padding = new Padding(0, 0, 5, 0);
        split.Panel1.Controls.Add(_processGrid);

        var right = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Bg,
            Padding = new Padding(8, 0, 0, 0),
            Margin = Padding.Empty
        };

        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        right.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        right.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var selectedTitle = MakeLabel("Auswahl / Reihenfolge", 12, FontStyle.Bold);
        selectedTitle.Margin = new Padding(0, 0, 0, 8);

        _selectedList.Dock = DockStyle.Fill;
        StyleListBox(_selectedList);
        _selectedList.Margin = Padding.Empty;

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 8, 0, 0),
            Margin = Padding.Empty,
            BackColor = Bg
        };

        var up = MakeButton("▲ Hoch", 90);
        up.Click += (_, _) => MoveSelected(-1);

        var down = MakeButton("▼ Runter", 90);
        down.Click += (_, _) => MoveSelected(1);

        var remove = MakeButton("Entfernen", 100);
        remove.Click += (_, _) => RemoveSelectedName();

        var save = MakeButton("Als Profil speichern", 190);
        save.Click += (_, _) => SaveCurrentSelectionAsProfile();
        _toolTip.SetToolTip(save, Localization.T("Speichert die aktuelle Auswahl und Reihenfolge als wiederverwendbares Profil."));

        var stopNow = MakeButton("Auswahl als Modus aktivieren", 225, danger: true);
        stopNow.Click += async (_, _) => await StopCurrentSelectionAsync();
        _toolTip.SetToolTip(stopNow, Localization.T("Zeigt zuerst eine Vorschau und beendet danach die ausgewählten Programme."));

        buttons.Controls.Add(up);
        buttons.Controls.Add(down);
        buttons.Controls.Add(remove);
        buttons.Controls.Add(save);
        buttons.Controls.Add(stopNow);

        right.Controls.Add(selectedTitle, 0, 0);
        right.Controls.Add(_selectedList, 0, 1);
        right.Controls.Add(buttons, 0, 2);

        split.Panel2.Controls.Add(right);

        root.Controls.Add(top, 0, 0);
        root.Controls.Add(split, 0, 1);

        page.Controls.Add(root);
        return page;
    }

    private TabPage BuildProfilesTab()
    {
        var page = MakePage("Profile");
        page.Padding = new Padding(10);

        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            SplitterWidth = 6,
            BackColor = Color.FromArgb(59, 64, 74)
        };

        ConfigureInitialSplitter(split, 0.40);

        var left = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            BackColor = Bg,
            Padding = new Padding(8)
        };

        left.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        left.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        left.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var title = MakeLabel("Gespeicherte Profile", 15, FontStyle.Bold);
        title.Margin = new Padding(0, 0, 0, 8);

        _manageProfiles.Dock = DockStyle.Fill;
        StyleListBox(_manageProfiles);
        _manageProfiles.SelectedIndexChanged += (_, _) =>
        {
            if (_manageProfiles.SelectedItem is string name)
                ShowProfile(name);
        };

        var actions = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Padding = new Padding(0, 8, 0, 0),
            Margin = Padding.Empty,
            BackColor = Bg
        };

        var load = MakeButton("In Prozessauswahl laden", 190);
        load.Click += (_, _) => LoadSelectedProfileIntoSelection();

        var execute = MakeButton("Aktivieren", 105);
        execute.Click += async (_, _) => await ExecuteManagedProfileAsync();

        var rename = MakeButton("Umbenennen", 115);
        rename.Click += (_, _) => RenameSelectedProfile();

        var duplicate = MakeButton("Duplizieren", 115);
        duplicate.Click += (_, _) => DuplicateSelectedProfile();

        var delete = MakeButton("Löschen", 105, danger: true);
        delete.Click += (_, _) => DeleteSelectedProfile();

        actions.Controls.Add(load);
        actions.Controls.Add(execute);
        actions.Controls.Add(rename);
        actions.Controls.Add(duplicate);
        actions.Controls.Add(delete);

        left.Controls.Add(title, 0, 0);
        left.Controls.Add(_manageProfiles, 0, 1);
        left.Controls.Add(actions, 0, 2);

        var right = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Bg,
            AutoScroll = true,
            Padding = new Padding(16)
        };

        var rightFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            BackColor = Bg,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        var detailTitle = MakeLabel("Profil bearbeiten / exportieren", 15, FontStyle.Bold);
        detailTitle.Margin = new Padding(0, 0, 0, 18);

        var nameLabel = MakeLabel("Profilname", 9, FontStyle.Regular);
        nameLabel.Margin = new Padding(0, 0, 0, 4);

        _profileName.Width = 360;
        _profileName.Margin = new Padding(0, 0, 0, 12);
        StyleTextBox(_profileName);

        var save = MakeButton("Aktuelle Auswahl speichern", 230);
        save.Margin = new Padding(0, 0, 0, 18);
        save.Click += (_, _) => SaveCurrentSelectionAsProfile(_profileName.Text);

        var exportLabel = MakeLabel("Export", 10, FontStyle.Bold);
        exportLabel.Margin = new Padding(0, 0, 0, 6);

        var exportRow = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            Margin = new Padding(0, 0, 0, 18),
            BackColor = Bg
        };

        var txt = MakeButton("TXT", 95);
        txt.Click += (_, _) => ExportCurrentProfile("txt");

        var ps1 = MakeButton("PowerShell", 125);
        ps1.Click += (_, _) => ExportCurrentProfile("ps1");

        var bat = MakeButton("Batch", 95);
        bat.Click += (_, _) => ExportCurrentProfile("bat");

        exportRow.Controls.Add(txt);
        exportRow.Controls.Add(ps1);
        exportRow.Controls.Add(bat);

        var detail = MakeLabel(
            "Die Exporte verwenden weiterhin das ursprüngliche Prinzip: Prozessnamen werden gespeichert bzw. als direkt ausführbares PowerShell-/Batch-Skript ausgegeben.",
            9,
            FontStyle.Regular);
        detail.MaximumSize = new Size(560, 0);
        detail.ForeColor = Muted;
        detail.Margin = Padding.Empty;

        rightFlow.Controls.Add(detailTitle);
        rightFlow.Controls.Add(nameLabel);
        rightFlow.Controls.Add(_profileName);
        rightFlow.Controls.Add(save);
        rightFlow.Controls.Add(exportLabel);
        rightFlow.Controls.Add(exportRow);
        rightFlow.Controls.Add(detail);

        right.Controls.Add(rightFlow);

        split.Panel1.Controls.Add(left);
        split.Panel2.Controls.Add(right);
        page.Controls.Add(split);

        return page;
    }

    private TabPage BuildSettingsTab()
    {
        var page = MakePage("Einstellungen");
        page.Padding = new Padding(18);

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            BackColor = Bg,
            Margin = Padding.Empty
        };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));

        var protection = MakeGroup("Schutzliste");
        protection.Dock = DockStyle.Fill;
        protection.Margin = new Padding(0, 0, 10, 0);

        var protectLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            BackColor = Bg
        };
        protectLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        protectLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        protectLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        protectLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var protectInfo = MakeLabel(
            "Diese Prozessnamen werden von ProcessSet niemals beendet. \".exe\" darf beim Eingeben mit angegeben werden.",
            9,
            FontStyle.Regular);
        protectInfo.ForeColor = Muted;
        protectInfo.MaximumSize = new Size(650, 0);
        protectInfo.Margin = new Padding(0, 0, 0, 8);

        _protectedList.Dock = DockStyle.Fill;
        StyleListBox(_protectedList);

        var inputRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Bg,
            Padding = new Padding(0, 8, 0, 0)
        };

        _protectInput.Width = 240;
        _protectInput.Margin = new Padding(0, 3, 8, 3);
        StyleTextBox(_protectInput);

        var add = MakeButton("Hinzufügen", 110);
        add.Click += (_, _) => AddProtectedProcess(_protectInput.Text);

        var remove = MakeButton("Entfernen", 105);
        remove.Click += (_, _) => RemoveProtectedProcess();

        inputRow.Controls.Add(_protectInput);
        inputRow.Controls.Add(add);
        inputRow.Controls.Add(remove);

        var selectRow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true,
            BackColor = Bg
        };

        var fromSelection = MakeButton("Aktuelle Prozessauswahl schützen", 235);
        fromSelection.Click += (_, _) => ProtectCurrentSelection();
        _toolTip.SetToolTip(fromSelection, Localization.T("Nimmt die momentan ausgewählten Prozessnamen in die Schutzliste auf."));

        selectRow.Controls.Add(fromSelection);

        protectLayout.Controls.Add(protectInfo, 0, 0);
        protectLayout.Controls.Add(_protectedList, 0, 1);
        protectLayout.Controls.Add(inputRow, 0, 2);
        protectLayout.Controls.Add(selectRow, 0, 3);
        protection.Controls.Add(protectLayout);

        var general = MakeGroup("Allgemein");
        general.Dock = DockStyle.Fill;
        general.Margin = new Padding(10, 0, 0, 0);

        var generalFlow = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Bg,
            Padding = new Padding(10)
        };

        _minimizeToTray.Text = Localization.T("Beim Minimieren in den Infobereich verschieben");
        _minimizeToTray.AutoSize = true;
        _minimizeToTray.ForeColor = TextColor;
        _minimizeToTray.Margin = new Padding(0, 0, 0, 14);
        _minimizeToTray.CheckedChanged += (_, _) =>
        {
            _settings.MinimizeToTray = _minimizeToTray.Checked;
            SaveSettings();
        };

        var languageTitle = MakeLabel("Sprache / Language", 10, FontStyle.Bold);
        languageTitle.Margin = new Padding(0, 0, 0, 6);

        _languageBox.DropDownStyle = ComboBoxStyle.DropDownList;
        _languageBox.Width = 190;
        _languageBox.BackColor = Panel2;
        _languageBox.ForeColor = TextColor;
        _languageBox.Items.AddRange(new object[] { "Deutsch", "English" });
        _languageBox.Margin = new Padding(0, 0, 0, 4);
        _languageBox.SelectedIndexChanged += (_, _) =>
        {
            if (_languageBox.SelectedIndex < 0)
                return;

            var language = _languageBox.SelectedIndex == 1 ? "en" : "de";
            if (string.Equals(language, _settings.Language, StringComparison.OrdinalIgnoreCase))
                return;

            _settings.Language = language;
            SaveSettings();

            var answer = MessageBox.Show(
                Localization.T("Die neue Sprache wird nach einem Neustart verwendet. ProcessSet Manager jetzt neu starten?"),
                Localization.T("Sprache geändert"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer == DialogResult.Yes)
            {
                _trayIcon.Visible = false;
                Application.Restart();
                Environment.Exit(0);
            }
        };

        var languageInfo = MakeLabel(
            "Sprachänderungen werden nach einem Neustart der Anwendung wirksam.",
            9,
            FontStyle.Regular);
        languageInfo.ForeColor = Muted;
        languageInfo.MaximumSize = new Size(520, 0);
        languageInfo.Margin = new Padding(0, 0, 0, 18);

        var builtInTitle = MakeLabel("Immer geschützte Windows-Prozesse", 10, FontStyle.Bold);
        builtInTitle.Margin = new Padding(0, 8, 0, 6);

        var builtIn = MakeLabel(
            string.Join(", ", ProcessService.GetBuiltInProtectedNames()),
            9,
            FontStyle.Regular);
        builtIn.ForeColor = Muted;
        builtIn.MaximumSize = new Size(520, 0);

        var pathInfo = MakeLabel(
            Localization.F("Konfiguration:\n{0}", _profileStore.BaseDirectory),
            9,
            FontStyle.Regular);
        pathInfo.ForeColor = Muted;
        pathInfo.MaximumSize = new Size(520, 0);
        pathInfo.Margin = new Padding(0, 22, 0, 0);

        generalFlow.Controls.Add(_minimizeToTray);
        generalFlow.Controls.Add(languageTitle);
        generalFlow.Controls.Add(_languageBox);
        generalFlow.Controls.Add(languageInfo);
        generalFlow.Controls.Add(builtInTitle);
        generalFlow.Controls.Add(builtIn);
        generalFlow.Controls.Add(pathInfo);

        general.Controls.Add(generalFlow);

        root.Controls.Add(protection, 0, 0);
        root.Controls.Add(general, 1, 0);
        page.Controls.Add(root);

        return page;
    }

    private TabPage BuildHelpTab()
    {
        var page = MakePage("Hilfe");
        page.Name = "HelpTab";
        page.Padding = new Padding(16);

        var help = new RichTextBox
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            BorderStyle = BorderStyle.FixedSingle,
            BackColor = Panel2,
            ForeColor = TextColor,
            Font = new Font("Segoe UI", 10),
            DetectUrls = false,
            WordWrap = true,
            ScrollBars = RichTextBoxScrollBars.Vertical
        };

        help.Text = Localization.HelpText;

        page.Controls.Add(help);
        return page;
    }

    private TabPage BuildLogTab()
    {
        var page = MakePage("Verlauf");
        page.Padding = new Padding(12);

        _logBox.Dock = DockStyle.Fill;
        _logBox.Multiline = true;
        _logBox.ReadOnly = true;
        _logBox.ScrollBars = ScrollBars.Both;
        _logBox.WordWrap = false;
        _logBox.Font = new Font("Consolas", 9);
        _logBox.BackColor = Panel2;
        _logBox.ForeColor = TextColor;
        _logBox.BorderStyle = BorderStyle.FixedSingle;

        page.Controls.Add(_logBox);
        return page;
    }

    private void BuildStatus()
    {
        _statusStrip.BackColor = Panel;
        _statusStrip.ForeColor = TextColor;
        _statusLabel.Spring = true;
        _statusLabel.TextAlign = ContentAlignment.MiddleLeft;
        _statusLabel.Text = Localization.T("Bereit");
        _statusStrip.Items.Add(_statusLabel);
        Controls.Add(_statusStrip);
        _statusStrip.BringToFront();
    }

    private void BuildTray()
    {
        try
        {
            _trayIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath)
                             ?? SystemIcons.Application;
        }
        catch
        {
            _trayIcon.Icon = SystemIcons.Application;
        }

        _trayIcon.Text = "ProcessSet Manager";
        _trayIcon.ContextMenuStrip = _trayMenu;
        _trayIcon.Visible = true;
        _trayIcon.DoubleClick += (_, _) => ShowMainWindow();

        RefreshTrayMenu();
    }

    private void RefreshTrayMenu()
    {
        _trayMenu.Items.Clear();

        var status = new ToolStripMenuItem(
            _modeState.IsActive
                ? Localization.F("● Aktiv: {0}", DisplayModeName(_modeState.ProfileName))
                : Localization.T("○ Kein aktiver Modus"))
        {
            Enabled = false
        };
        _trayMenu.Items.Add(status);

        if (_modeState.IsActive)
        {
            var endMode = new ToolStripMenuItem(Localization.T("Modus beenden & wiederherstellen"));
            endMode.Click += (_, _) => RestoreLastSession(fromActiveMode: true);
            _trayMenu.Items.Add(endMode);
        }

        _trayMenu.Items.Add(new ToolStripSeparator());

        var profiles = new ToolStripMenuItem(Localization.T("Profil aktivieren"));
        var names = _profileStore.GetProfileNames();

        if (names.Count == 0)
        {
            profiles.DropDownItems.Add(new ToolStripMenuItem(Localization.T("(keine Profile)")) { Enabled = false });
        }
        else
        {
            foreach (var name in names)
            {
                var item = new ToolStripMenuItem(name)
                {
                    Enabled = !_modeState.IsActive
                };

                item.Click += async (_, _) => await ExecuteProfileAsync(name);
                profiles.DropDownItems.Add(item);
            }
        }

        _trayMenu.Items.Add(profiles);
        _trayMenu.Items.Add(new ToolStripSeparator());

        var show = new ToolStripMenuItem(Localization.T("Fenster öffnen"));
        show.Click += (_, _) => ShowMainWindow();

        var exit = new ToolStripMenuItem(Localization.T("Beenden"));
        exit.Click += (_, _) => Close();

        _trayMenu.Items.Add(show);
        _trayMenu.Items.Add(exit);
    }

    private void ShowMainWindow()
    {
        Show();
        WindowState = FormWindowState.Normal;
        BringToFront();
        Activate();
    }

    private void BuildProcessGrid()
    {
        _processGrid.Dock = DockStyle.Fill;
        _processGrid.BackgroundColor = Panel2;
        _processGrid.BorderStyle = BorderStyle.FixedSingle;
        _processGrid.AllowUserToAddRows = false;
        _processGrid.AllowUserToDeleteRows = false;
        _processGrid.AllowUserToResizeRows = false;
        _processGrid.RowHeadersVisible = false;
        _processGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _processGrid.MultiSelect = true;
        _processGrid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        _processGrid.EnableHeadersVisualStyles = false;
        _processGrid.ColumnHeadersDefaultCellStyle.BackColor = Panel;
        _processGrid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
        _processGrid.DefaultCellStyle.BackColor = Panel2;
        _processGrid.DefaultCellStyle.ForeColor = TextColor;
        _processGrid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(58, 84, 125);
        _processGrid.DefaultCellStyle.SelectionForeColor = TextColor;
        _processGrid.GridColor = Color.FromArgb(59, 64, 74);
        _processGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None;

        _processGrid.Columns.Add(new DataGridViewCheckBoxColumn
        {
            Name = "Selected",
            HeaderText = "",
            Width = 34
        });

        _processGrid.Columns.Add("Name", Localization.T("Prozess"));
        _processGrid.Columns.Add("Pid", "PID");
        _processGrid.Columns.Add("Kind", Localization.T("Typ"));
        _processGrid.Columns.Add("Ram", "RAM");
        _processGrid.Columns.Add("Protected", Localization.T("Schutz"));
        _processGrid.Columns.Add("Restorable", "↻");
        _processGrid.Columns.Add("Title", Localization.T("Fenstertitel"));
        _processGrid.Columns.Add("Path", Localization.T("Pfad"));

        _processGrid.Columns["Name"]!.Width = 160;
        _processGrid.Columns["Pid"]!.Width = 65;
        _processGrid.Columns["Kind"]!.Width = 90;
        _processGrid.Columns["Ram"]!.Width = 80;
        _processGrid.Columns["Protected"]!.Width = 65;
        _processGrid.Columns["Restorable"]!.Width = 50;
        _processGrid.Columns["Restorable"]!.HeaderCell.ToolTipText =
            Localization.T("Ja = ein lesbarer EXE-Pfad ist vorhanden und der Prozess kann grundsätzlich automatisch neu gestartet werden.");
        _processGrid.Columns["Protected"]!.HeaderCell.ToolTipText =
            Localization.T("Geschützte Prozesse werden von ProcessSet niemals beendet.");
        _processGrid.Columns["Title"]!.Width = 230;
        _processGrid.Columns["Path"]!.Width = 380;

        _processGrid.CurrentCellDirtyStateChanged += (_, _) =>
        {
            if (_processGrid.IsCurrentCellDirty)
                _processGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);
        };

        _processGrid.CellValueChanged += (_, e) =>
        {
            if (_updatingChecks || e.RowIndex < 0 || e.ColumnIndex != 0)
                return;

            var row = _processGrid.Rows[e.RowIndex];
            var name = row.Cells["Name"].Value?.ToString() ?? string.Empty;
            var selected = row.Cells["Selected"].Value is bool b && b;

            SetProcessNameSelected(name, selected);
        };
    }

    private void RefreshProcesses()
    {
        SetStatus(Localization.T("Prozesse werden geladen …"));
        Application.DoEvents();

        _allProcesses.Clear();
        _allProcesses.AddRange(_processService.GetProcesses());
        RefreshProcessGrid();

        SetStatus(Localization.F("{0} Prozesse geladen | {1} ausgewählt", _allProcesses.Count, _selectedOrder.Count));
        Log(Localization.F("{0} Prozesse eingelesen.", _allProcesses.Count));
    }

    private void RefreshProcessGrid()
    {
        var search = _searchBox.Text.Trim();

        IEnumerable<ProcessInfoRow> filtered = _allProcesses;

        if (_onlyApps.Checked)
            filtered = filtered.Where(p => p.Kind == "App");

        if (!string.IsNullOrWhiteSpace(search))
        {
            filtered = filtered.Where(p =>
                p.Name.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Title.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Path.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Kind.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                Localization.T(p.Kind).Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        _updatingChecks = true;
        try
        {
            _processGrid.Rows.Clear();

            foreach (var p in filtered)
            {
                var protectedProcess = IsUserProtected(p.Name);
                var selected = ContainsSelected(p.Name) && !protectedProcess;

                _processGrid.Rows.Add(
                    selected,
                    p.Name,
                    p.Id,
                    Localization.T(p.Kind),
                    p.MemoryText,
                    protectedProcess ? Localization.T("Ja") : "",
                    IsRestorablePath(p.Path) ? Localization.T("Ja") : Localization.T("Nein"),
                    p.Title,
                    p.Path);
            }
        }
        finally
        {
            _updatingChecks = false;
        }

        RefreshSelectedList();
    }

    private void SetProcessNameSelected(string name, bool selected)
    {
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (selected && IsUserProtected(name))
        {
            _updatingChecks = true;
            try
            {
                foreach (DataGridViewRow row in _processGrid.Rows)
                {
                    var rowName = row.Cells["Name"].Value?.ToString();
                    if (string.Equals(rowName, name, StringComparison.OrdinalIgnoreCase))
                        row.Cells["Selected"].Value = false;
                }
            }
            finally
            {
                _updatingChecks = false;
            }

            SetStatus(Localization.F("{0} steht auf der Schutzliste.", name));
            return;
        }

        if (selected && !ContainsSelected(name))
            _selectedOrder.Add(name);

        if (!selected)
            _selectedOrder.RemoveAll(n => n.Equals(name, StringComparison.OrdinalIgnoreCase));

        _updatingChecks = true;
        try
        {
            foreach (DataGridViewRow row in _processGrid.Rows)
            {
                var rowName = row.Cells["Name"].Value?.ToString();
                if (string.Equals(rowName, name, StringComparison.OrdinalIgnoreCase))
                    row.Cells["Selected"].Value = selected;
            }
        }
        finally
        {
            _updatingChecks = false;
        }

        RefreshSelectedList();
        SetStatus(Localization.F("{0} Programme ausgewählt", _selectedOrder.Count));
    }

    private void RefreshSelectedList()
    {
        _selectedList.BeginUpdate();
        try
        {
            _selectedList.Items.Clear();
            for (var i = 0; i < _selectedOrder.Count; i++)
                _selectedList.Items.Add($"{i + 1}. {_selectedOrder[i]}");
        }
        finally
        {
            _selectedList.EndUpdate();
        }
    }

    private void MoveSelected(int direction)
    {
        var index = _selectedList.SelectedIndex;
        if (index < 0)
            return;

        var target = index + direction;
        if (target < 0 || target >= _selectedOrder.Count)
            return;

        var name = _selectedOrder[index];
        _selectedOrder.RemoveAt(index);
        _selectedOrder.Insert(target, name);
        RefreshSelectedList();
        _selectedList.SelectedIndex = target;
    }

    private void RemoveSelectedName()
    {
        var index = _selectedList.SelectedIndex;
        if (index < 0 || index >= _selectedOrder.Count)
            return;

        var name = _selectedOrder[index];
        SetProcessNameSelected(name, false);
    }

    private bool ContainsSelected(string name) =>
        _selectedOrder.Any(n => n.Equals(name, StringComparison.OrdinalIgnoreCase));

    private bool IsUserProtected(string name) =>
        _settings.ProtectedProcesses.Any(
            p => p.Equals(name, StringComparison.OrdinalIgnoreCase));

    private ProcessProfile BuildProfile(string name)
    {
        var profile = new ProcessProfile
        {
            Name = name.Trim(),
            Created = DateTime.Now
        };

        foreach (var selectedName in _selectedOrder.Where(n => !IsUserProtected(n)))
        {
            var best = _allProcesses.FirstOrDefault(p =>
                p.Name.Equals(selectedName, StringComparison.OrdinalIgnoreCase) &&
                !string.IsNullOrWhiteSpace(p.Path));

            profile.Processes.Add(new ProcessTarget
            {
                Name = selectedName,
                Path = best?.Path
            });
        }

        return profile;
    }

    private void SaveCurrentSelectionAsProfile(string? suggestedName = null)
    {
        if (_selectedOrder.Count == 0)
        {
            MessageBox.Show(Localization.T("Bitte zuerst Programme auswählen."), Localization.T("Keine Auswahl"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var name = suggestedName?.Trim();

        if (string.IsNullOrWhiteSpace(name))
        {
            using var prompt = new ProfileNameForm(Localization.T("Profil speichern"));
            if (prompt.ShowDialog(this) != DialogResult.OK)
                return;

            name = prompt.ProfileName;
        }

        try
        {
            _profileStore.SaveProfile(BuildProfile(name!));
            _profileName.Text = name;
            RefreshProfileLists();
            SetStatus(Localization.F("Profil '{0}' gespeichert", name));
            Log(Localization.F("Profil '{0}' gespeichert.", name));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Profil speichern"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RefreshProfileLists()
    {
        var names = _profileStore.GetProfileNames();

        FillList(_dashboardProfiles, names);
        FillList(_manageProfiles, names);
        UpdateDashboardProfileDetails();
        RefreshTrayMenu();
    }

    private static void FillList(ListBox list, IReadOnlyList<string> names)
    {
        var previous = list.SelectedItem?.ToString();

        list.BeginUpdate();
        try
        {
            list.Items.Clear();
            foreach (var name in names)
                list.Items.Add(name);
        }
        finally
        {
            list.EndUpdate();
        }

        if (previous is not null && list.Items.Contains(previous))
            list.SelectedItem = previous;
        else if (list.Items.Count > 0 && list.SelectedIndex < 0)
            list.SelectedIndex = 0;
    }

    private void UpdateDashboardProfileDetails()
    {
        if (_dashboardProfiles.SelectedItem is not string name)
        {
            _dashboardProfileDetails.Text = Localization.T("Keine Profile gespeichert.");
            return;
        }

        try
        {
            var profile = _profileStore.LoadProfile(name);
            var last = profile.LastExecutedAt.HasValue
                ? profile.LastExecutedAt.Value.ToString("g")
                : Localization.T("noch nie");

            _dashboardProfileDetails.Text =
                Localization.F("{0} Programme · zuletzt aktiviert: {1}", profile.Processes.Count, last);
        }
        catch
        {
            _dashboardProfileDetails.Text = string.Empty;
        }
    }

    private async Task ExecuteSelectedProfileAsync()
    {
        if (_dashboardProfiles.SelectedItem is not string name)
        {
            MessageBox.Show(Localization.T("Bitte zuerst ein Profil auswählen."), Localization.T("Profil"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        await ExecuteProfileAsync(name);
    }

    private async Task ExecuteManagedProfileAsync()
    {
        if (_manageProfiles.SelectedItem is string name)
            await ExecuteProfileAsync(name);
    }

    private async Task ExecuteProfileAsync(string name)
    {
        if (_modeState.IsActive)
        {
            MessageBox.Show(
                Localization.F("Der Modus '{0}' ist bereits aktiv.\n\n", DisplayModeName(_modeState.ProfileName)) +
                Localization.T("Beende und stelle diesen Modus zuerst wieder her, bevor ein anderes Profil aktiviert wird."),
                Localization.T("Modus bereits aktiv"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        try
        {
            var profile = _profileStore.LoadProfile(name);
            var names = profile.Processes.Select(p => p.Name).ToList();

            if (names.Count == 0)
                return;

            var preview = _processService.CreateModePreview(
                names,
                _settings.ProtectedProcesses);

            if (!ShowModePreview(name, preview))
                return;

            if (preview.RunningCount == 0)
            {
                MessageBox.Show(
                    Localization.T("Von diesem Profil läuft aktuell kein beendbarer Prozess. Der Modus wurde daher nicht aktiviert."),
                    Localization.T("Nichts zu beenden"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            var snapshot = new SessionSnapshot
            {
                Created = DateTime.Now,
                SourceProfile = name,
                Processes = preview.Items
                    .Where(i => i.IsRunning && i.IsRestorable && !i.IsProtected && !string.IsNullOrWhiteSpace(i.RestartPath))
                    .Select(i => new RestartTarget
                    {
                        Name = i.Name,
                        Path = i.RestartPath!
                    })
                    .ToList()
            };

            _profileStore.SaveLastSession(snapshot);

            var progress = new Progress<string>(messageText =>
            {
                SetStatus(messageText);
                Log(messageText);
            });

            var result = await _processService.StopProcessesAsync(
                names,
                _settings.ProtectedProcesses,
                progress);

            profile.LastExecutedAt = DateTime.Now;
            _profileStore.SaveProfile(profile);

            _modeState = new ModeState
            {
                IsActive = true,
                ProfileName = name,
                ActivatedAt = DateTime.Now,
                RestorableCount = snapshot.Processes.Count
            };
            _profileStore.SaveModeState(_modeState);

            RefreshModeUi();
            RefreshProfileLists();
            RefreshProcesses();

            SetStatus(Localization.F("Modus '{0}' ist aktiv", name));
            Log(Localization.F(
                "Modus '{0}' aktiviert. Beendet: {1}, geschützt: {2}, Fehler: {3}, wiederherstellbar: {4}.",
                name,
                result.StoppedCount,
                result.ProtectedCount,
                result.ErrorCount,
                snapshot.Processes.Count));

            ShowActivationResult(name, result, snapshot.Processes.Count);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Modus aktivieren"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private async Task StopCurrentSelectionAsync()
    {
        if (_selectedOrder.Count == 0)
            return;

        if (_modeState.IsActive)
        {
            MessageBox.Show(
                Localization.T("Es ist bereits ein Modus aktiv. Beende ihn zuerst, damit die Wiederherstellungssitzung nicht überschrieben wird."),
                Localization.T("Modus bereits aktiv"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var preview = _processService.CreateModePreview(
            _selectedOrder,
            _settings.ProtectedProcesses);

        if (!ShowModePreview(Localization.T("Manuelle Auswahl"), preview))
            return;

        if (preview.RunningCount == 0)
        {
            MessageBox.Show(
                Localization.T("Von der aktuellen Auswahl läuft kein beendbarer Prozess. Der Modus wurde daher nicht aktiviert."),
                Localization.T("Nichts zu beenden"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var snapshot = new SessionSnapshot
        {
            Created = DateTime.Now,
            SourceProfile = null,
            Processes = preview.Items
                .Where(i => i.IsRunning && i.IsRestorable && !i.IsProtected && !string.IsNullOrWhiteSpace(i.RestartPath))
                .Select(i => new RestartTarget
                {
                    Name = i.Name,
                    Path = i.RestartPath!
                })
                .ToList()
        };

        _profileStore.SaveLastSession(snapshot);

        var progress = new Progress<string>(message =>
        {
            SetStatus(message);
            Log(message);
        });

        var result = await _processService.StopProcessesAsync(
            _selectedOrder,
            _settings.ProtectedProcesses,
            progress);

        _modeState = new ModeState
        {
            IsActive = true,
            ProfileName = null,
            ActivatedAt = DateTime.Now,
            RestorableCount = snapshot.Processes.Count
        };
        _profileStore.SaveModeState(_modeState);

        RefreshModeUi();
        RefreshProcesses();

        Log(Localization.F(
            "Temporärer Modus aktiviert. Beendet: {0}, Fehler: {1}, wiederherstellbar: {2}.",
            result.StoppedCount,
            result.ErrorCount,
            snapshot.Processes.Count));

        ShowActivationResult(Localization.T("Manuelle Auswahl"), result, snapshot.Processes.Count);
    }

    private void RestoreLastSession(bool fromActiveMode)
    {
        try
        {
            var snapshot = _profileStore.LoadLastSession();

            if (snapshot is null)
            {
                MessageBox.Show(
                    Localization.T("Es ist keine Wiederherstellungssitzung gespeichert."),
                    Localization.T("Wiederherstellen"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (snapshot.Processes.Count == 0)
            {
                if (fromActiveMode && _modeState.IsActive)
                {
                    var clear = MessageBox.Show(
                        Localization.T("Für diesen Modus konnten keine Programme mit einem wiederstartbaren EXE-Pfad gespeichert werden.\n\nDen Modus trotzdem beenden?"),
                        Localization.T("Modus beenden"),
                        MessageBoxButtons.YesNo,
                        MessageBoxIcon.Question);

                    if (clear == DialogResult.Yes)
                        ClearActiveMode();
                }
                else
                {
                    MessageBox.Show(
                        Localization.T("Die letzte Sitzung enthält keine wiederherstellbaren Programme."),
                        Localization.T("Wiederherstellen"),
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                }

                return;
            }

            var title = fromActiveMode ? Localization.T("Modus beenden & wiederherstellen") : Localization.T("Wiederherstellen");
            var answer = MessageBox.Show(
                Localization.F("Sitzung vom {0:g} wiederherstellen?\n\n", snapshot.Created) +
                string.Join(Environment.NewLine, snapshot.Processes.Select(p => p.Name)),
                title,
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (answer != DialogResult.Yes)
                return;

            var progress = new Progress<string>(message =>
            {
                SetStatus(message);
                Log(message);
            });

            var result = _processService.RestoreSnapshot(snapshot, progress);

            if (_modeState.IsActive)
                ClearActiveMode();

            Log(Localization.F(
                "Wiederherstellung abgeschlossen. Gestartet: {0}, bereits aktiv: {1}, Fehler: {2}.",
                result.StartedCount,
                result.AlreadyRunningCount,
                result.ErrorCount));

            ShowRestoreResult(result);
            RefreshProcesses();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Wiederherstellen"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ClearActiveMode()
    {
        var previous = DisplayModeName(_modeState.ProfileName);
        _modeState = new ModeState();
        _profileStore.ClearModeState();
        RefreshModeUi();
        SetStatus(Localization.F("{0} beendet", previous));
        Log(Localization.F("{0} beendet.", previous));
    }

    private void RefreshModeUi()
    {
        if (_modeState.IsActive)
        {
            _modeStatusLabel.Text = Localization.F("● {0} aktiv", DisplayModeName(_modeState.ProfileName));
            _modeStatusLabel.ForeColor = Active;

            var since = _modeState.ActivatedAt.HasValue
                ? _modeState.ActivatedAt.Value.ToString("g")
                : Localization.T("unbekannt");

            _modeDetailsLabel.Text =
                Localization.F("Aktiv seit {0} · {1} Programme wiederherstellbar", since, _modeState.RestorableCount);
            _modeActionButton.Visible = true;
        }
        else
        {
            _modeStatusLabel.Text = Localization.T("○ Kein aktiver Modus");
            _modeStatusLabel.ForeColor = Muted;
            _modeDetailsLabel.Text = Localization.T("Wähle links ein Profil und aktiviere es, oder erstelle im Tab „Prozesse“ eine manuelle Auswahl.");
            _modeActionButton.Visible = false;
        }

        RefreshTrayMenu();
    }

    private void ShowProfile(string name)
    {
        try
        {
            var profile = _profileStore.LoadProfile(name);
            _profileName.Text = profile.Name;

            var last = profile.LastExecutedAt.HasValue
                ? Localization.F(" · zuletzt {0:g}", profile.LastExecutedAt.Value)
                : string.Empty;

            SetStatus(Localization.F("{0}: {1} Programme{2}", profile.Name, profile.Processes.Count, last));
        }
        catch
        {
            // Profil könnte zwischenzeitlich gelöscht worden sein.
        }
    }

    private void LoadSelectedProfileIntoSelection()
    {
        if (_manageProfiles.SelectedItem is not string name)
            return;

        try
        {
            var profile = _profileStore.LoadProfile(name);

            _selectedOrder.Clear();

            foreach (var target in profile.Processes)
            {
                if (IsUserProtected(target.Name))
                {
                    Log(Localization.F("{0}: wegen Schutzliste nicht in die Auswahl übernommen.", target.Name));
                    continue;
                }

                if (!ContainsSelected(target.Name))
                    _selectedOrder.Add(target.Name);
            }

            _profileName.Text = profile.Name;
            RefreshProcessGrid();
            _tabs.SelectedIndex = 1;
            SetStatus(Localization.F("Profil '{0}' in Auswahl geladen", name));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Profil laden"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void RenameSelectedProfile()
    {
        if (_manageProfiles.SelectedItem is not string oldName)
            return;

        if (_modeState.IsActive &&
            string.Equals(_modeState.ProfileName, oldName, StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                Localization.T("Das aktuell aktive Profil kann erst nach dem Beenden des Modus umbenannt werden."),
                Localization.T("Profil aktiv"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        using var prompt = new ProfileNameForm(Localization.T("Profil umbenennen"), oldName);
        if (prompt.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            _profileStore.RenameProfile(oldName, prompt.ProfileName);
            RefreshProfileLists();
            SelectProfile(prompt.ProfileName);
            Log(Localization.F("Profil '{0}' in '{1}' umbenannt.", oldName, prompt.ProfileName));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Profil umbenennen"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DuplicateSelectedProfile()
    {
        if (_manageProfiles.SelectedItem is not string sourceName)
            return;

        using var prompt = new ProfileNameForm(Localization.T("Profil duplizieren"), sourceName + " " + Localization.T("Kopie"));
        if (prompt.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            _profileStore.DuplicateProfile(sourceName, prompt.ProfileName);
            RefreshProfileLists();
            SelectProfile(prompt.ProfileName);
            Log(Localization.F("Profil '{0}' als '{1}' dupliziert.", sourceName, prompt.ProfileName));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Profil duplizieren"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void SelectProfile(string name)
    {
        if (_manageProfiles.Items.Contains(name))
            _manageProfiles.SelectedItem = name;

        if (_dashboardProfiles.Items.Contains(name))
            _dashboardProfiles.SelectedItem = name;
    }

    private void DeleteSelectedProfile()
    {
        if (_manageProfiles.SelectedItem is not string name)
            return;

        if (_modeState.IsActive &&
            string.Equals(_modeState.ProfileName, name, StringComparison.OrdinalIgnoreCase))
        {
            MessageBox.Show(
                Localization.T("Das aktuell aktive Profil kann erst nach dem Beenden des Modus gelöscht werden."),
                Localization.T("Profil aktiv"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if (MessageBox.Show(
                Localization.F("Profil '{0}' wirklich löschen?", name),
                Localization.T("Profil löschen"),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        _profileStore.DeleteProfile(name);
        RefreshProfileLists();
        _profileName.Clear();
        Log(Localization.F("Profil '{0}' gelöscht.", name));
    }

    private void ExportCurrentProfile(string type)
    {
        ProcessProfile? profile = null;

        if (_manageProfiles.SelectedItem is string selectedProfile)
        {
            try { profile = _profileStore.LoadProfile(selectedProfile); }
            catch { }
        }

        if (profile is null && _selectedOrder.Count > 0)
            profile = BuildProfile(string.IsNullOrWhiteSpace(_profileName.Text) ? "process-set" : _profileName.Text);

        if (profile is null || profile.Processes.Count == 0)
        {
            MessageBox.Show(Localization.T("Kein Profil bzw. keine Auswahl zum Exportieren vorhanden."), Localization.T("Export"),
                MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        using var dialog = new SaveFileDialog();
        var names = profile.Processes.Select(p => p.Name).ToList();

        switch (type)
        {
            case "txt":
                dialog.Filter = Localization.T("Textdatei (*.txt)|*.txt");
                dialog.FileName = profile.Name + ".txt";
                break;
            case "ps1":
                dialog.Filter = Localization.T("PowerShell-Skript (*.ps1)|*.ps1");
                dialog.FileName = profile.Name + ".ps1";
                break;
            case "bat":
                dialog.Filter = Localization.T("Batch-Datei (*.bat)|*.bat");
                dialog.FileName = profile.Name + ".bat";
                break;
            default:
                return;
        }

        if (dialog.ShowDialog(this) != DialogResult.OK)
            return;

        try
        {
            if (type == "txt") ExportService.ExportTxt(dialog.FileName, names);
            if (type == "ps1") ExportService.ExportPowerShell(dialog.FileName, names);
            if (type == "bat") ExportService.ExportBatch(dialog.FileName, names);

            SetStatus(Localization.F("Exportiert: {0}", dialog.FileName));
            Log(Localization.F("Profil '{0}' als {1} exportiert.", profile.Name, type.ToUpperInvariant()));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, Localization.T("Export"),
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadSettingsIntoUi()
    {
        _minimizeToTray.Checked = _settings.MinimizeToTray;
        _languageBox.SelectedIndex =
            string.Equals(_settings.Language, "en", StringComparison.OrdinalIgnoreCase) ? 1 : 0;
        RefreshProtectedList();
    }

    private void AddProtectedProcess(string rawName)
    {
        var name = ProfileStore.NormalizeProcessName(rawName);
        if (string.IsNullOrWhiteSpace(name))
            return;

        if (ProcessService.GetBuiltInProtectedNames().Any(
                n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show(
                Localization.F("{0} ist bereits fest durch ProcessSet geschützt.", name),
                Localization.T("Bereits geschützt"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        if (!_settings.ProtectedProcesses.Any(
                n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
        {
            _settings.ProtectedProcesses.Add(name);
        }

        _selectedOrder.RemoveAll(n => n.Equals(name, StringComparison.OrdinalIgnoreCase));
        _protectInput.Clear();
        SaveSettings();
        RefreshProtectedList();
        RefreshProcessGrid();

        Log(Localization.F("{0} zur Schutzliste hinzugefügt.", name));
    }

    private void RemoveProtectedProcess()
    {
        if (_protectedList.SelectedItem is not string name)
            return;

        _settings.ProtectedProcesses.RemoveAll(
            n => n.Equals(name, StringComparison.OrdinalIgnoreCase));

        SaveSettings();
        RefreshProtectedList();
        RefreshProcessGrid();
        Log(Localization.F("{0} aus Schutzliste entfernt.", name));
    }

    private void ProtectCurrentSelection()
    {
        if (_selectedOrder.Count == 0)
        {
            MessageBox.Show(
                Localization.T("Es sind aktuell keine Programme ausgewählt."),
                Localization.T("Schutzliste"),
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);
            return;
        }

        var names = _selectedOrder.ToList();
        foreach (var name in names)
        {
            if (!_settings.ProtectedProcesses.Any(
                    n => n.Equals(name, StringComparison.OrdinalIgnoreCase)))
            {
                _settings.ProtectedProcesses.Add(name);
            }
        }

        _selectedOrder.Clear();
        SaveSettings();
        RefreshProtectedList();
        RefreshProcessGrid();
        Log(Localization.F("{0} Prozessnamen aus der aktuellen Auswahl geschützt.", names.Count));
    }

    private void RefreshProtectedList()
    {
        _settings.ProtectedProcesses = _settings.ProtectedProcesses
            .Where(n => !string.IsNullOrWhiteSpace(n))
            .Select(ProfileStore.NormalizeProcessName)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(n => n, StringComparer.CurrentCultureIgnoreCase)
            .ToList();

        _protectedList.BeginUpdate();
        try
        {
            _protectedList.Items.Clear();
            foreach (var name in _settings.ProtectedProcesses)
                _protectedList.Items.Add(name);
        }
        finally
        {
            _protectedList.EndUpdate();
        }
    }

    private void SaveSettings()
    {
        try
        {
            _profileStore.SaveSettings(_settings);
        }
        catch (Exception ex)
        {
            Log(Localization.F("Einstellungen konnten nicht gespeichert werden: {0}", ex.Message));
        }
    }

    private static bool IsRestorablePath(string? path) =>
        !string.IsNullOrWhiteSpace(path) && File.Exists(path);

    private string DisplayModeName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name) ||
            name.Equals("Manuelle Auswahl", StringComparison.OrdinalIgnoreCase) ||
            name.Equals("Manual selection", StringComparison.OrdinalIgnoreCase))
        {
            return Localization.T("Manuelle Auswahl");
        }

        return name;
    }

    private bool ShowModePreview(string modeName, ModePreview preview)
    {
        using var dialog = new ModePreviewForm(modeName, preview);
        return dialog.ShowDialog(this) == DialogResult.OK;
    }

    private void ShowActivationResult(string modeName, StopOperationResult result, int restorableCount)
    {
        var lines = new List<string>
        {
            Localization.F("Modus „{0}“ ist aktiv.", modeName),
            "",
            Localization.F("Beendet: {0}", result.StoppedCount),
            Localization.F("Davon erzwungen beendet: {0}", result.ForcedCount),
            Localization.F("Liefen nicht: {0}", result.NotRunningCount),
            Localization.F("Geschützt / übersprungen: {0}", result.ProtectedCount),
            Localization.F("Fehler: {0}", result.ErrorCount),
            Localization.F("Für Wiederherstellung gespeichert: {0}", restorableCount)
        };

        if (result.Errors.Count > 0)
        {
            lines.Add("");
            lines.Add(Localization.T("Fehlerdetails:"));
            lines.AddRange(result.Errors.Take(8));

            if (result.Errors.Count > 8)
                lines.Add(Localization.F("… und {0} weitere.", result.Errors.Count - 8));
        }

        MessageBox.Show(
            string.Join(Environment.NewLine, lines),
            Localization.T("Modus aktiviert – Ergebnis"),
            MessageBoxButtons.OK,
            result.ErrorCount > 0 ? MessageBoxIcon.Warning : MessageBoxIcon.Information);
    }

    private void ShowRestoreResult(RestoreOperationResult result)
    {
        var lines = new List<string>
        {
            Localization.T("Wiederherstellung abgeschlossen."),
            "",
            Localization.F("Gestartet: {0}", result.StartedCount),
            Localization.F("Liefen bereits: {0}", result.AlreadyRunningCount),
            Localization.F("EXE nicht gefunden: {0}", result.MissingFileCount),
            Localization.F("Fehler: {0}", result.ErrorCount)
        };

        if (result.Details.Count > 0)
        {
            lines.Add("");
            lines.Add(Localization.T("Details:"));
            lines.AddRange(result.Details.Take(10));

            if (result.Details.Count > 10)
                lines.Add(Localization.F("… und {0} weitere.", result.Details.Count - 10));
        }

        MessageBox.Show(
            string.Join(Environment.NewLine, lines),
            Localization.T("Wiederherstellung – Ergebnis"),
            MessageBoxButtons.OK,
            result.ErrorCount > 0 || result.MissingFileCount > 0
                ? MessageBoxIcon.Warning
                : MessageBoxIcon.Information);
    }

    private static void ConfigureInitialSplitter(SplitContainer split, double ratio)
    {
        var initialized = false;

        void ApplyWhenReady()
        {
            if (initialized || split.IsDisposed)
                return;

            var available = split.ClientSize.Width - split.SplitterWidth;
            if (available < 240)
                return;

            const int safePanelMinimum = 120;
            var desired = (int)Math.Round(available * ratio);
            var maximum = Math.Max(safePanelMinimum, available - safePanelMinimum);
            desired = Math.Clamp(desired, safePanelMinimum, maximum);

            try
            {
                split.SplitterDistance = desired;
                initialized = true;
            }
            catch (ArgumentOutOfRangeException)
            {
            }
            catch (InvalidOperationException)
            {
            }
        }

        split.HandleCreated += (_, _) =>
        {
            if (!split.IsDisposed && split.IsHandleCreated)
                split.BeginInvoke((Action)ApplyWhenReady);
        };

        split.SizeChanged += (_, _) => ApplyWhenReady();
    }

    private static void TryApplyApplicationIcon(Form form)
    {
        try
        {
            var icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (icon is not null)
                form.Icon = (Icon)icon.Clone();
        }
        catch
        {
        }
    }

    private void SetStatus(string text) => _statusLabel.Text = text;

    private void Log(string text)
    {
        var line = $"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}";
        _logBox.AppendText(line);
    }

    private TabPage MakePage(string text) => new()
    {
        Text = Localization.T(text),
        BackColor = Bg,
        ForeColor = TextColor,
        AutoScroll = false
    };

    private GroupBox MakeGroup(string text) => new()
    {
        Text = Localization.T(text),
        ForeColor = TextColor,
        BackColor = Bg,
        Padding = new Padding(10)
    };

    private Label MakeLabel(string text, float size, FontStyle style) => new()
    {
        Text = Localization.T(text),
        AutoSize = true,
        Font = new Font("Segoe UI", size, style),
        ForeColor = TextColor
    };

    private Button MakeButton(string text, int minWidth = 0, bool danger = false)
    {
        var button = new Button();
        ConfigureButton(button, Localization.T(text), minWidth, danger);
        return button;
    }

    private static void ConfigureButton(Button button, string text, int minWidth, bool danger)
    {
        button.Text = text;
        button.AutoSize = true;
        button.AutoSizeMode = AutoSizeMode.GrowAndShrink;
        button.MinimumSize = new Size(minWidth, 34);
        button.Padding = new Padding(10, 4, 10, 4);
        button.FlatStyle = FlatStyle.Flat;
        button.BackColor = danger ? Danger : Panel2;
        button.ForeColor = TextColor;
        button.UseVisualStyleBackColor = false;
        button.Margin = new Padding(0, 0, 8, 8);

        button.FlatAppearance.BorderColor = danger
            ? Danger
            : Color.FromArgb(65, 70, 80);

        button.FlatAppearance.MouseOverBackColor = danger
            ? Color.FromArgb(210, 75, 75)
            : Color.FromArgb(49, 54, 64);
    }

    private static void StyleTextBox(TextBox box)
    {
        box.BackColor = Panel2;
        box.ForeColor = TextColor;
        box.BorderStyle = BorderStyle.FixedSingle;
    }

    private static void StyleListBox(ListBox box)
    {
        box.BackColor = Panel2;
        box.ForeColor = TextColor;
        box.BorderStyle = BorderStyle.FixedSingle;
        box.IntegralHeight = false;
    }
}

internal sealed class ProfileNameForm : Form
{
    private readonly TextBox _box = new();

    public string ProfileName => _box.Text.Trim();

    public ProfileNameForm(string title, string initialValue = "")
    {
        Text = Localization.T(title);
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Padding = new Padding(16);
        BackColor = Color.FromArgb(31, 34, 40);
        ForeColor = Color.WhiteSmoke;
        Font = new Font("Segoe UI", 9);

        var layout = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 3,
            Dock = DockStyle.Fill
        };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            Text = Localization.T("Profilname:"),
            AutoSize = true,
            Margin = new Padding(0, 0, 0, 5)
        };

        _box.Width = 360;
        _box.Text = initialValue;
        _box.BackColor = Color.FromArgb(38, 42, 49);
        _box.ForeColor = Color.WhiteSmoke;
        _box.BorderStyle = BorderStyle.FixedSingle;
        _box.Margin = new Padding(0, 0, 0, 14);

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty
        };

        var cancel = new Button
        {
            Text = Localization.T("Abbrechen"),
            AutoSize = true,
            MinimumSize = new Size(100, 34),
            DialogResult = DialogResult.Cancel
        };

        var ok = new Button
        {
            Text = Localization.T("Speichern"),
            AutoSize = true,
            MinimumSize = new Size(100, 34),
            DialogResult = DialogResult.OK
        };

        ok.Click += (_, _) =>
        {
            if (string.IsNullOrWhiteSpace(_box.Text))
            {
                MessageBox.Show(
                    Localization.T("Bitte einen Profilnamen eingeben."),
                    Localization.T("Profilname"),
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);

                DialogResult = DialogResult.None;
            }
        };

        buttons.Controls.Add(cancel);
        buttons.Controls.Add(ok);

        layout.Controls.Add(label, 0, 0);
        layout.Controls.Add(_box, 0, 1);
        layout.Controls.Add(buttons, 0, 2);

        Controls.Add(layout);

        AcceptButton = ok;
        CancelButton = cancel;
        Shown += (_, _) =>
        {
            _box.Focus();
            _box.SelectAll();
        };
    }
}
