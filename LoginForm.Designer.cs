#nullable enable
namespace CarpoolingApp;

partial class LoginForm
{
    private System.ComponentModel.IContainer? components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private Panel pnlTop = null!;
    private Label lblAppName = null!;
    private Label lblAppSub = null!;
    private TabControl tabAuth = null!;
    private TabPage tabLogin = null!;
    private TabPage tabRegister = null!;
    private TextBox txtLogin = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Label lblDemoHint = null!;
    private Button btnFillDriver = null!;
    private Button btnFillPassenger = null!;
    private TextBox txtRegName = null!;
    private TextBox txtRegPhone = null!;
    private TextBox txtRegLogin = null!;
    private TextBox txtRegPass = null!;
    private ComboBox cmbRegRole = null!;
    private Button btnRegister = null!;

    private void InitializeComponent()
    {
        pnlTop = new Panel();
        lblAppName = new Label();
        lblAppSub = new Label();
        tabAuth = new TabControl();
        tabLogin = new TabPage();
        tabRegister = new TabPage();
        txtLogin = new TextBox();
        txtPassword = new TextBox();
        btnLogin = new Button();
        lblDemoHint = new Label();
        btnFillDriver = new Button();
        btnFillPassenger = new Button();
        txtRegName = new TextBox();
        txtRegPhone = new TextBox();
        txtRegLogin = new TextBox();
        txtRegPass = new TextBox();
        cmbRegRole = new ComboBox();
        btnRegister = new Button();

        pnlTop.SuspendLayout();
        tabAuth.SuspendLayout();
        tabLogin.SuspendLayout();
        tabRegister.SuspendLayout();
        SuspendLayout();

        // pnlTop
        pnlTop.BackColor = Color.FromArgb(30, 58, 95);
        pnlTop.Dock = DockStyle.Top;
        pnlTop.Size = new Size(440, 74);
        lblAppName.AutoSize = true;
        lblAppName.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblAppName.ForeColor = Color.White;
        lblAppName.Location = new Point(20, 14);
        lblAppName.Text = "СЕРВИС ПОИСКА ПОПУТЧИКОВ";

        lblAppSub.AutoSize = true;
        lblAppSub.Font = new Font("Segoe UI", 9F);
        lblAppSub.ForeColor = Color.LightSteelBlue;
        lblAppSub.Location = new Point(20, 42);
        lblAppSub.Text = "Авторизация в системе (Версия 2.0) | ПМ.03";
        pnlTop.Controls.AddRange([lblAppName, lblAppSub]);

        // tabAuth
        tabAuth.Location = new Point(16, 86);
        tabAuth.Size = new Size(408, 266);
        tabAuth.Controls.AddRange([tabLogin, tabRegister]);

        // tabLogin
        tabLogin.BackColor = Color.White;
        tabLogin.Padding = new Padding(12);
        tabLogin.Text = "Вход в систему";

        var l1 = new Label { AutoSize = true, Text = "Логин:", Location = new Point(20, 22) };
        txtLogin.Location = new Point(110, 19);
        txtLogin.Size = new Size(260, 24);
        txtLogin.Text = "driver";

        var l2 = new Label { AutoSize = true, Text = "Пароль:", Location = new Point(20, 58) };
        txtPassword.Location = new Point(110, 55);
        txtPassword.Size = new Size(260, 24);
        txtPassword.UseSystemPasswordChar = true;
        txtPassword.Text = "1234";

        btnLogin.BackColor = Color.FromArgb(43, 108, 176);
        btnLogin.ForeColor = Color.White;
        btnLogin.FlatStyle = FlatStyle.Flat;
        btnLogin.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnLogin.Location = new Point(20, 96);
        btnLogin.Size = new Size(350, 36);
        btnLogin.Text = "Войти в программу";
        btnLogin.Click += BtnLogin_Click;

        lblDemoHint.AutoSize = true;
        lblDemoHint.ForeColor = Color.DimGray;
        lblDemoHint.Font = new Font("Segoe UI", 8.5F);
        lblDemoHint.Location = new Point(20, 145);
        lblDemoHint.Text = "Быстрый выбор тестовой учетной записи (пароль 1234):";

        btnFillDriver.Location = new Point(20, 170);
        btnFillDriver.Size = new Size(168, 30);
        btnFillDriver.Text = "Водитель (Касимов)";
        btnFillDriver.Click += (_, _) => { txtLogin.Text = "driver"; txtPassword.Text = "1234"; };

        btnFillPassenger.Location = new Point(202, 170);
        btnFillPassenger.Size = new Size(168, 30);
        btnFillPassenger.Text = "Пассажир (Петрова)";
        btnFillPassenger.Click += (_, _) => { txtLogin.Text = "passenger"; txtPassword.Text = "1234"; };

        tabLogin.Controls.AddRange([l1, txtLogin, l2, txtPassword, btnLogin, lblDemoHint, btnFillDriver, btnFillPassenger]);

        // tabRegister
        tabRegister.BackColor = Color.White;
        tabRegister.Padding = new Padding(10);
        tabRegister.Text = "Регистрация";

        var r1 = new Label { AutoSize = true, Text = "ФИО:", Location = new Point(18, 18) };
        txtRegName.Location = new Point(110, 15);
        txtRegName.Size = new Size(260, 24);

        var r2 = new Label { AutoSize = true, Text = "Телефон:", Location = new Point(18, 50) };
        txtRegPhone.Location = new Point(110, 47);
        txtRegPhone.Size = new Size(260, 24);
        txtRegPhone.Text = "+7 (900) 000-00-00";

        var r3 = new Label { AutoSize = true, Text = "Логин:", Location = new Point(18, 82) };
        txtRegLogin.Location = new Point(110, 79);
        txtRegLogin.Size = new Size(260, 24);

        var r4 = new Label { AutoSize = true, Text = "Пароль:", Location = new Point(18, 114) };
        txtRegPass.Location = new Point(110, 111);
        txtRegPass.Size = new Size(260, 24);
        txtRegPass.UseSystemPasswordChar = true;

        var r5 = new Label { AutoSize = true, Text = "Роль:", Location = new Point(18, 146) };
        cmbRegRole.DropDownStyle = ComboBoxStyle.DropDownList;
        cmbRegRole.Items.AddRange(["Пассажир", "Водитель"]);
        cmbRegRole.SelectedIndex = 0;
        cmbRegRole.Location = new Point(110, 143);
        cmbRegRole.Size = new Size(260, 24);

        btnRegister.BackColor = Color.FromArgb(47, 133, 90);
        btnRegister.ForeColor = Color.White;
        btnRegister.FlatStyle = FlatStyle.Flat;
        btnRegister.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnRegister.Location = new Point(18, 182);
        btnRegister.Size = new Size(352, 34);
        btnRegister.Text = "Зарегистрироваться и войти";
        btnRegister.Click += BtnRegister_Click;

        tabRegister.Controls.AddRange([r1, txtRegName, r2, txtRegPhone, r3, txtRegLogin, r4, txtRegPass, r5, cmbRegRole, btnRegister]);

        AcceptButton = btnLogin;
        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        BackColor = Color.FromArgb(244, 246, 249);
        ClientSize = new Size(440, 366);
        Controls.AddRange([pnlTop, tabAuth]);
        Font = new Font("Segoe UI", 9.5F);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Вход — Сервис поиска попутчиков v2.0";

        pnlTop.ResumeLayout(false);
        pnlTop.PerformLayout();
        tabAuth.ResumeLayout(false);
        tabLogin.ResumeLayout(false);
        tabLogin.PerformLayout();
        tabRegister.ResumeLayout(false);
        tabRegister.PerformLayout();
        ResumeLayout(false);
    }
}
