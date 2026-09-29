using ProcessSetManager.Models;

namespace ProcessSetManager;

public sealed class ModePreviewForm : Form
{
    private static readonly Color Bg = Color.FromArgb(24, 26, 31);
    private static readonly Color Panel = Color.FromArgb(31, 34, 40);
    private static readonly Color Panel2 = Color.FromArgb(38, 42, 49);
    private static readonly Color TextColor = Color.FromArgb(235, 238, 242);
    private static readonly Color Muted = Color.FromArgb(170, 176, 185);

    public ModePreviewForm(string modeName, ModePreview preview)
    {
        Text = Localization.F("Vorschau – {0}", modeName);
        StartPosition = FormStartPosition.CenterParent;
        MinimumSize = new Size(760, 480);
        Size = new Size(900, 590);
        AutoScaleMode = AutoScaleMode.Dpi;
        BackColor = Bg;
        ForeColor = TextColor;
        Font = new Font("Segoe UI", 9);

        try
        {
            var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (appIcon is not null)
                Icon = (Icon)appIcon.Clone();
        }
        catch
        {
        }

        var root = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 4,
            Padding = new Padding(16),
            BackColor = Bg
        };

        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        root.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var intro = new Label
        {
            Text = Localization.F("Folgendes würde beim Aktivieren von „{0}“ passieren:", modeName),
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 12),
            ForeColor = TextColor,
            Margin = new Padding(0, 0, 0, 10)
        };

        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            BackgroundColor = Panel2,
            BorderStyle = BorderStyle.FixedSingle,
            EnableHeadersVisualStyles = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        grid.ColumnHeadersDefaultCellStyle.BackColor = Panel;
        grid.ColumnHeadersDefaultCellStyle.ForeColor = TextColor;
        grid.DefaultCellStyle.BackColor = Panel2;
        grid.DefaultCellStyle.ForeColor = TextColor;
        grid.DefaultCellStyle.SelectionBackColor = Color.FromArgb(58, 84, 125);
        grid.DefaultCellStyle.SelectionForeColor = TextColor;
        grid.GridColor = Color.FromArgb(59, 64, 74);

        grid.Columns.Add("Name", Localization.T("Prozess"));
        grid.Columns.Add("State", Localization.T("Aktueller Zustand"));
        grid.Columns.Add("Restore", Localization.T("Wiederherstellung"));
        grid.Columns.Add("Instances", Localization.T("Instanzen"));

        foreach (var item in preview.Items)
        {
            string state;
            string restore;

            if (item.IsProtected)
            {
                state = Localization.T("Geschützt – wird übersprungen");
                restore = Localization.T("Nicht nötig");
            }
            else if (!item.IsRunning)
            {
                state = Localization.T("Läuft nicht");
                restore = Localization.T("Nicht nötig");
            }
            else
            {
                state = Localization.T("Wird beendet");
                restore = item.IsRestorable
                    ? Localization.T("Ja")
                    : Localization.T("Nein – kein lesbarer EXE-Pfad");
            }

            grid.Rows.Add(
                item.Name,
                state,
                restore,
                item.RunningInstances > 0 ? item.RunningInstances.ToString() : "–");
        }

        var summary = new Label
        {
            AutoSize = true,
            ForeColor = Muted,
            MaximumSize = new Size(820, 0),
            Margin = new Padding(0, 12, 0, 8),
            Text = Localization.F(
                "{0} laufende Programme würden beendet · {1} davon automatisch wiederherstellbar · {2} nicht automatisch wiederherstellbar · {3} geschützt · {4} laufen derzeit nicht",
                preview.RunningCount,
                preview.RestorableCount,
                preview.NonRestorableRunningCount,
                preview.ProtectedCount,
                preview.NotRunningCount)
        };

        var warning = new Label
        {
            AutoSize = true,
            ForeColor = preview.NonRestorableRunningCount > 0 ? Color.Khaki : Muted,
            MaximumSize = new Size(820, 0),
            Margin = new Padding(0, 0, 0, 10),
            Text = preview.NonRestorableRunningCount > 0
                ? Localization.T("Hinweis: Programme ohne lesbaren EXE-Pfad können beendet, aber von ProcessSet danach nicht automatisch neu gestartet werden.")
                : Localization.T("Die Wiederherstellung startet Programme neu. Offene Dokumente, Tabs oder interne Sitzungen kann nur die jeweilige Anwendung selbst wiederherstellen.")
        };

        var buttons = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = Padding.Empty
        };

        var cancel = new Button
        {
            Text = Localization.T("Abbrechen"),
            AutoSize = true,
            MinimumSize = new Size(110, 36),
            DialogResult = DialogResult.Cancel
        };

        var activate = new Button
        {
            Text = Localization.T("Modus aktivieren"),
            AutoSize = true,
            MinimumSize = new Size(150, 36),
            DialogResult = DialogResult.OK
        };

        buttons.Controls.Add(cancel);
        buttons.Controls.Add(activate);

        var infoStack = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            AutoSize = true,
            ColumnCount = 1,
            RowCount = 2
        };
        infoStack.Controls.Add(summary, 0, 0);
        infoStack.Controls.Add(warning, 0, 1);

        root.Controls.Add(intro, 0, 0);
        root.Controls.Add(grid, 0, 1);
        root.Controls.Add(infoStack, 0, 2);
        root.Controls.Add(buttons, 0, 3);

        Controls.Add(root);

        AcceptButton = activate;
        CancelButton = cancel;
    }
}
