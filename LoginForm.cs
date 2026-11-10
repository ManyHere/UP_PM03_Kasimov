namespace CarpoolingApp;

public partial class LoginForm : Form
{
    public UserAccount? AuthenticatedUser { get; private set; }

    public LoginForm()
    {
        InitializeComponent();
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        string login = txtLogin.Text.Trim();
        string pass = txtPassword.Text;

        if (string.IsNullOrWhiteSpace(login) || string.IsNullOrEmpty(pass))
        {
            MessageBox.Show("Введите логин и пароль.", "Авторизация", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        UserAccount? user = DatabaseHelper.AuthenticateUser(login, pass);
        if (user is null)
        {
            MessageBox.Show("Неверный логин или пароль.", "Ошибка входа", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        AuthenticatedUser = user;
        DialogResult = DialogResult.OK;
        Close();
    }

    private void BtnRegister_Click(object? sender, EventArgs e)
    {
        try
        {
            string role = cmbRegRole.SelectedItem?.ToString() ?? "Пассажир";
            UserAccount created = DatabaseHelper.RegisterUser(
                txtRegName.Text,
                txtRegPhone.Text,
                txtRegLogin.Text,
                txtRegPass.Text,
                role
            );

            AuthenticatedUser = created;
            MessageBox.Show($"Учетная запись «{created.FullName}» успешно создана!", "Регистрация", MessageBoxButtons.OK, MessageBoxIcon.Information);
            DialogResult = DialogResult.OK;
            Close();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка регистрации", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }
}
