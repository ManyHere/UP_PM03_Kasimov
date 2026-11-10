namespace CarpoolingApp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        DatabaseHelper.InitializeDatabase();
        Application.Run(new MainForm());
    }
}
