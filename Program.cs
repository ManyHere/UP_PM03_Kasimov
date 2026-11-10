namespace CarpoolingApp;

internal static class Program
{
    [STAThread]
    private static void Main()
    {
        ApplicationConfiguration.Initialize();
        DatabaseHelper.InitializeDatabase();

        while (true)
        {
            using var loginForm = new LoginForm();
            if (loginForm.ShowDialog() != DialogResult.OK || loginForm.AuthenticatedUser is null)
                break;

            using var mainForm = new MainForm(loginForm.AuthenticatedUser);
            if (mainForm.ShowDialog() != DialogResult.Retry)
                break;
        }
    }
}
