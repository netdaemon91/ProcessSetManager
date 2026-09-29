using System.Diagnostics;

namespace ProcessSetManager;

public sealed class AboutForm : Form
{
    private static readonly Color Bg = Color.FromArgb(31, 34, 40);
    private static readonly Color TextColor = Color.FromArgb(235, 238, 242);
    private static readonly Color Accent = Color.FromArgb(64, 132, 255);

    public AboutForm()
    {
        Text = Localization.T("About - ProcessSet Manager");
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        AutoScaleMode = AutoScaleMode.Dpi;
        AutoSize = true;
        AutoSizeMode = AutoSizeMode.GrowAndShrink;
        Padding = new Padding(18);
        BackColor = Bg;
        ForeColor = TextColor;
        Font = new Font("Segoe UI", 10);

        try
        {
            var appIcon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath);
            if (appIcon is not null)
                Icon = (Icon)appIcon.Clone();
        }
        catch
        {
        }

        var layout = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            RowCount = 4,
            Dock = DockStyle.Fill,
            BackColor = Bg,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };

        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var title = new Label
        {
            Text = "ProcessSet Manager",
            AutoSize = true,
            Font = new Font("Segoe UI Semibold", 17),
            ForeColor = TextColor,
            Margin = new Padding(0, 0, 0, 20)
        };

        var credit = new Label
        {
            Text = Localization.F("Vibe Coded by NetDaemon  ·  Version {0}", Application.ProductVersion),
            AutoSize = true,
            ForeColor = TextColor,
            Margin = new Padding(0, 0, 0, 6)
        };

        var link = new LinkLabel
        {
            Text = "https://ntdmn.xyz/",
            AutoSize = true,
            LinkColor = Accent,
            ActiveLinkColor = Color.LightSkyBlue,
            VisitedLinkColor = Accent,
            BackColor = Bg,
            Margin = new Padding(0, 0, 0, 20)
        };

        link.LinkClicked += (_, _) =>
        {
            try
            {
                Process.Start(new ProcessStartInfo("https://ntdmn.xyz/")
                {
                    UseShellExecute = true
                });
            }
            catch
            {
                MessageBox.Show(
                    "https://ntdmn.xyz/",
                    "Link",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
            }
        };

        var buttons = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty
        };

        var ok = new Button
        {
            Text = "OK",
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(90, 34),
            Padding = new Padding(10, 3, 10, 3),
            DialogResult = DialogResult.OK
        };

        buttons.Controls.Add(ok);

        layout.Controls.Add(title, 0, 0);
        layout.Controls.Add(credit, 0, 1);
        layout.Controls.Add(link, 0, 2);
        layout.Controls.Add(buttons, 0, 3);

        Controls.Add(layout);

        AcceptButton = ok;
        CancelButton = ok;
    }
}
