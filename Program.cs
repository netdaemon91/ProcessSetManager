using System.Text;

namespace ProcessSetManager;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        Application.SetHighDpiMode(HighDpiMode.PerMonitorV2);
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);

        Application.ThreadException += (_, args) =>
            ReportCrash("WinForms UI exception", args.Exception);

        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                ReportCrash("Unhandled application exception", ex);
            else
                ReportCrash("Unhandled application exception", new Exception(args.ExceptionObject?.ToString()));
        };

        try
        {
            Application.Run(new MainForm());
        }
        catch (Exception ex)
        {
            ReportCrash("Startup exception", ex);
        }
    }

    private static void ReportCrash(string context, Exception exception)
    {
        string? logPath = null;

        try
        {
            var directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ProcessSetManager");

            Directory.CreateDirectory(directory);
            logPath = Path.Combine(directory, "crash.log");

            var text = new StringBuilder()
                .AppendLine("ProcessSet Manager crash report")
                .AppendLine($"Time: {DateTime.Now:O}")
                .AppendLine($"Context: {context}")
                .AppendLine($"Version: {Application.ProductVersion}")
                .AppendLine($"OS: {Environment.OSVersion}")
                .AppendLine($"64-bit process: {Environment.Is64BitProcess}")
                .AppendLine()
                .AppendLine(exception.ToString())
                .AppendLine(new string('-', 80))
                .ToString();

            File.AppendAllText(logPath, text, Encoding.UTF8);
        }
        catch
        {
            // Logging itself must never hide the original crash.
        }

        try
        {
            var message =
                "ProcessSet Manager konnte nicht gestartet bzw. weiter ausgeführt werden.\n\n" +
                $"{exception.GetType().Name}: {exception.Message}";

            if (!string.IsNullOrWhiteSpace(logPath))
                message += $"\n\nFehlerdetails wurden gespeichert unter:\n{logPath}";

            MessageBox.Show(
                message,
                "ProcessSet Manager – Fehler",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
        }
        catch
        {
            // Last resort: nothing else can safely be displayed.
        }
    }
}
