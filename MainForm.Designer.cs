#nullable enable
namespace CarpoolingApp;

partial class MainForm
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

    private Panel pnlHeader = null!;
    private Label lblTitle = null!;
    private Label lblUserBadge = null!;
    private Button btnSwitchUser = null!;
    private TabControl tabMain = null!;
    private TabPage tabSearch = null!;
    private TabPage tabPublish = null!;
    private TabPage tabExperiment = null!;
    private TabPage tabManage = null!;

    // Tab 1: Search & Book
    private GroupBox grpFilter = null!;
    private TextBox txtFilterFrom = null!;
    private TextBox txtFilterTo = null!;
    private Button btnSearch = null!;
    private Button btnReset = null!;
    private DataGridView dgvTrips = null!;
    private GroupBox grpBook = null!;
    private Label lblSelectedTrip = null!;
    private TextBox txtBookName = null!;
    private TextBox txtBookPhone = null!;
    private NumericUpDown numBookSeats = null!;
    private Button btnBookSeat = null!;

    // Tab 2: Publish Trip
    private GroupBox grpPublishCard = null!;
    private TextBox txtPubDriver = null!;
    private TextBox txtPubPhone = null!;
    private TextBox txtPubCar = null!;
    private TextBox txtPubFrom = null!;
    private TextBox txtPubTo = null!;
    private TextBox txtPubDate = null!;
    private TextBox txtPubTime = null!;
    private NumericUpDown numPubSeats = null!;
    private TextBox txtPubDist = null!;
    private TextBox txtPubPrice = null!;
    private Button btnPublishTrip = null!;
    private Button btnFillSample = null!;

    // Tab 3: Experiment
    private GroupBox grpExpParams = null!;
    private TextBox txtExpRoute = null!;
    private TextBox txtExpDist = null!;
    private TextBox txtExpCons = null!;
    private TextBox txtExpFuelPrice = null!;
    private TextBox txtExpDeprec = null!;
    private TextBox txtExpTripsYear = null!;
    private Button btnRunExp = null!;
    private Label lblExpSummary = null!;
    private DataGridView dgvExpGrid = null!;
    private Button btnSaveExp = null!;

    // Tab 4: Bookings, Audit & Export
    private Panel pnlManageTop = null!;
    private Button btnRefreshManage = null!;
    private Button btnCancelBooking = null!;
    private Button btnExportTrips = null!;
    private Button btnExportBookings = null!;
    private SplitContainer splitManage = null!;
    private DataGridView dgvBookings = null!;
    private DataGridView dgvAudit = null!;

    private StatusStrip statusStrip1 = null!;
    private ToolStripStatusLabel lblStatus = null!;

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblTitle = new Label();
        lblUserBadge = new Label();
        btnSwitchUser = new Button();
        tabMain = new TabControl();
        tabSearch = new TabPage();
        tabPublish = new TabPage();
        tabExperiment = new TabPage();
        tabManage = new TabPage();

        grpFilter = new GroupBox();
        txtFilterFrom = new TextBox();
        txtFilterTo = new TextBox();
        btnSearch = new Button();
        btnReset = new Button();
        dgvTrips = new DataGridView();
        grpBook = new GroupBox();
        lblSelectedTrip = new Label();
        txtBookName = new TextBox();
        txtBookPhone = new TextBox();
        numBookSeats = new NumericUpDown();
        btnBookSeat = new Button();

        grpPublishCard = new GroupBox();
        txtPubDriver = new TextBox();
        txtPubPhone = new TextBox();
        txtPubCar = new TextBox();
        txtPubFrom = new TextBox();
        txtPubTo = new TextBox();
        txtPubDate = new TextBox();
        txtPubTime = new TextBox();
        numPubSeats = new NumericUpDown();
        txtPubDist = new TextBox();
        txtPubPrice = new TextBox();
        btnPublishTrip = new Button();
        btnFillSample = new Button();

        grpExpParams = new GroupBox();
        txtExpRoute = new TextBox();
        txtExpDist = new TextBox();
        txtExpCons = new TextBox();
        txtExpFuelPrice = new TextBox();
        txtExpDeprec = new TextBox();
        txtExpTripsYear = new TextBox();
        btnRunExp = new Button();
        lblExpSummary = new Label();
        dgvExpGrid = new DataGridView();
        btnSaveExp = new Button();

        pnlManageTop = new Panel();
        btnRefreshManage = new Button();
        btnCancelBooking = new Button();
        btnExportTrips = new Button();
        btnExportBookings = new Button();
        splitManage = new SplitContainer();
        dgvBookings = new DataGridView();
        dgvAudit = new DataGridView();

        statusStrip1 = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();

        pnlHeader.SuspendLayout();
        tabMain.SuspendLayout();
        tabSearch.SuspendLayout();
        grpFilter.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTrips).BeginInit();
        grpBook.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numBookSeats).BeginInit();
        tabPublish.SuspendLayout();
        grpPublishCard.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numPubSeats).BeginInit();
        tabExperiment.SuspendLayout();
        grpExpParams.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvExpGrid).BeginInit();
        tabManage.SuspendLayout();
        pnlManageTop.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)splitManage).BeginInit();
        splitManage.Panel1.SuspendLayout();
        splitManage.Panel2.SuspendLayout();
        splitManage.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvBookings).BeginInit();
        ((System.ComponentModel.ISupportInitialize)dgvAudit).BeginInit();
        statusStrip1.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 58, 95);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Size = new Size(1140, 58);

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 13.5F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(16, 16);
        lblTitle.Text = "СЕРВИС ПОИСКА ПОПУТЧИКОВ — Версия 2.0 (Касимов Д.А., ИСП 31-9)";

        lblUserBadge.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblUserBadge.AutoSize = false;
        lblUserBadge.TextAlign = ContentAlignment.MiddleRight;
        lblUserBadge.Font = new Font("Segoe UI", 9.5F);
        lblUserBadge.ForeColor = Color.LightSteelBlue;
        lblUserBadge.Location = new Point(660, 17);
        lblUserBadge.Size = new Size(315, 24);
        lblUserBadge.Text = "Пользователь: Гость";

        btnSwitchUser.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSwitchUser.BackColor = Color.FromArgb(43, 108, 176);
        btnSwitchUser.FlatStyle = FlatStyle.Flat;
        btnSwitchUser.FlatAppearance.BorderSize = 0;
        btnSwitchUser.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
        btnSwitchUser.ForeColor = Color.White;
        btnSwitchUser.Location = new Point(985, 14);
        btnSwitchUser.Size = new Size(140, 30);
        btnSwitchUser.Text = "Сменить аккаунт";
        btnSwitchUser.Click += BtnSwitchUser_Click;

        pnlHeader.Controls.AddRange([lblTitle, lblUserBadge, btnSwitchUser]);

        // tabMain
        tabMain.Dock = DockStyle.Fill;
        tabMain.Padding = new Point(12, 6);
        tabMain.Controls.AddRange([tabSearch, tabPublish, tabExperiment, tabManage]);

        // ==================== TAB 1: SEARCH & BOOK ====================
        tabSearch.BackColor = Color.FromArgb(247, 249, 252);
        tabSearch.Padding = new Padding(12);
        tabSearch.Text = "🔍 Поиск и бронирование";

        grpFilter.Dock = DockStyle.Top;
        grpFilter.Size = new Size(1100, 66);
        grpFilter.Text = "Параметры поиска маршрута";

        var lf1 = new Label { AutoSize = true, Text = "Откуда:", Location = new Point(16, 28) };
        txtFilterFrom.Location = new Point(76, 25);
        txtFilterFrom.Size = new Size(170, 24);
        txtFilterFrom.Text = "Ижевск";

        var lf2 = new Label { AutoSize = true, Text = "Куда:", Location = new Point(268, 28) };
        txtFilterTo.Location = new Point(314, 25);
        txtFilterTo.Size = new Size(170, 24);

        btnSearch.BackColor = Color.FromArgb(43, 108, 176);
        btnSearch.FlatStyle = FlatStyle.Flat;
        btnSearch.ForeColor = Color.White;
        btnSearch.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSearch.Location = new Point(506, 22);
        btnSearch.Size = new Size(140, 30);
        btnSearch.Text = "🔍 Найти поездки";
        btnSearch.Click += (_, _) => LoadTrips();

        btnReset.Location = new Point(658, 22);
        btnReset.Size = new Size(145, 30);
        btnReset.Text = "🔄 Сбросить фильтр";
        btnReset.Click += (_, _) => { txtFilterFrom.Clear(); txtFilterTo.Clear(); LoadTrips(); };

        grpFilter.Controls.AddRange([lf1, txtFilterFrom, lf2, txtFilterTo, btnSearch, btnReset]);

        grpBook.Dock = DockStyle.Bottom;
        grpBook.Size = new Size(1100, 96);
        grpBook.Text = "Оформление бронирования для пассажира";

        lblSelectedTrip.AutoSize = true;
        lblSelectedTrip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        lblSelectedTrip.ForeColor = Color.FromArgb(43, 108, 176);
        lblSelectedTrip.Location = new Point(16, 24);
        lblSelectedTrip.Text = "Выберите поездку из таблицы выше для бронирования места.";

        var lb1 = new Label { AutoSize = true, Text = "ФИО пассажира:", Location = new Point(16, 56) };
        txtBookName.Location = new Point(132, 53);
        txtBookName.Size = new Size(215, 24);

        var lb2 = new Label { AutoSize = true, Text = "Телефон:", Location = new Point(364, 56) };
        txtBookPhone.Location = new Point(432, 53);
        txtBookPhone.Size = new Size(165, 24);

        var lb3 = new Label { AutoSize = true, Text = "Кол-во мест:", Location = new Point(615, 56) };
        numBookSeats.Location = new Point(710, 53);
        numBookSeats.Size = new Size(58, 24);
        numBookSeats.Minimum = 1;
        numBookSeats.Maximum = 4;
        numBookSeats.Value = 1;

        btnBookSeat.BackColor = Color.FromArgb(47, 133, 90);
        btnBookSeat.FlatStyle = FlatStyle.Flat;
        btnBookSeat.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnBookSeat.ForeColor = Color.White;
        btnBookSeat.Location = new Point(792, 48);
        btnBookSeat.Size = new Size(210, 34);
        btnBookSeat.Text = "✅ Забронировать место";
        btnBookSeat.Enabled = false;
        btnBookSeat.Click += BtnBookSeat_Click;

        grpBook.Controls.AddRange([lblSelectedTrip, lb1, txtBookName, lb2, txtBookPhone, lb3, numBookSeats, btnBookSeat]);

        ConfigureGrid(dgvTrips);
        dgvTrips.Dock = DockStyle.Fill;
        dgvTrips.SelectionChanged += DgvTrips_SelectionChanged;

        tabSearch.Controls.AddRange([dgvTrips, grpFilter, grpBook]);

        // ==================== TAB 2: PUBLISH TRIP ====================
        tabPublish.BackColor = Color.FromArgb(247, 249, 252);
        tabPublish.Padding = new Padding(16);
        tabPublish.Text = "➕ Опубликовать поездку";

        grpPublishCard.Location = new Point(20, 16);
        grpPublishCard.Size = new Size(860, 340);
        grpPublishCard.Text = "Данные новой поездки водителя";

        AddPair(grpPublishCard, "ФИО водителя:", txtPubDriver, 20, 34);
        AddPair(grpPublishCard, "Телефон:", txtPubPhone, 440, 34);
        AddPair(grpPublishCard, "Марка и модель авто:", txtPubCar, 20, 76, "Lada Vesta SW Cross");
        AddPair(grpPublishCard, "Город отправления:", txtPubFrom, 440, 76, "Ижевск");
        AddPair(grpPublishCard, "Город назначения:", txtPubTo, 20, 118, "Уфа");
        AddPair(grpPublishCard, "Дата (ГГГГ-ММ-ДД):", txtPubDate, 440, 118, "2026-11-18");
        AddPair(grpPublishCard, "Время выезда (ЧЧ:ММ):", txtPubTime, 20, 160, "07:30");

        var lpSeats = new Label { AutoSize = true, Text = "Свободных мест:", Location = new Point(440, 163) };
        numPubSeats.Location = new Point(600, 160);
        numPubSeats.Size = new Size(220, 24);
        numPubSeats.Minimum = 1;
        numPubSeats.Maximum = 8;
        numPubSeats.Value = 3;
        grpPublishCard.Controls.AddRange([lpSeats, numPubSeats]);

        AddPair(grpPublishCard, "Дистанция (км):", txtPubDist, 20, 202, "340");
        AddPair(grpPublishCard, "Цена 1 места (руб.):", txtPubPrice, 440, 202, "850");

        btnPublishTrip.BackColor = Color.FromArgb(47, 133, 90);
        btnPublishTrip.FlatStyle = FlatStyle.Flat;
        btnPublishTrip.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnPublishTrip.ForeColor = Color.White;
        btnPublishTrip.Location = new Point(180, 260);
        btnPublishTrip.Size = new Size(280, 38);
        btnPublishTrip.Text = "🚀 Опубликовать маршрут в БД";
        btnPublishTrip.Click += BtnPublishTrip_Click;

        btnFillSample.Location = new Point(480, 260);
        btnFillSample.Size = new Size(270, 38);
        btnFillSample.Text = "Заполнить пример (Ижевск → Сарапул)";
        btnFillSample.Click += BtnFillSample_Click;

        grpPublishCard.Controls.AddRange([btnPublishTrip, btnFillSample]);
        tabPublish.Controls.Add(grpPublishCard);

        // ==================== TAB 3: COMPUTATIONAL EXPERIMENT ====================
        tabExperiment.BackColor = Color.FromArgb(247, 249, 252);
        tabExperiment.Padding = new Padding(12);
        tabExperiment.Text = "📊 Вычислительный эксперимент";

        grpExpParams.Dock = DockStyle.Top;
        grpExpParams.Size = new Size(1100, 112);
        grpExpParams.Text = "Параметры моделирования себестоимости и распределения затрат";

        AddSmallField(grpExpParams, "Маршрут:", txtExpRoute, 16, 30, 155, "Ижевск — Казань");
        AddSmallField(grpExpParams, "Дистанция (км):", txtExpDist, 265, 30, 90, "390");
        AddSmallField(grpExpParams, "Расход (л/100км):", txtExpCons, 470, 30, 90, "8.5");

        AddSmallField(grpExpParams, "Цена (руб/л):", txtExpFuelPrice, 16, 68, 155, "58.5");
        AddSmallField(grpExpParams, "Аморт. (руб/км):", txtExpDeprec, 265, 68, 90, "1.8");
        AddSmallField(grpExpParams, "Поездок в год:", txtExpTripsYear, 470, 68, 90, "40");

        btnRunExp.BackColor = Color.FromArgb(43, 108, 176);
        btnRunExp.FlatStyle = FlatStyle.Flat;
        btnRunExp.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
        btnRunExp.ForeColor = Color.White;
        btnRunExp.Location = new Point(685, 28);
        btnRunExp.Size = new Size(280, 62);
        btnRunExp.Text = "▶ Провести вычислительный\nэксперимент";
        btnRunExp.Click += BtnRunExp_Click;

        grpExpParams.Controls.Add(btnRunExp);

        lblExpSummary.Dock = DockStyle.Top;
        lblExpSummary.Font = new Font("Segoe UI", 10.5F, FontStyle.Bold);
        lblExpSummary.ForeColor = Color.FromArgb(30, 58, 95);
        lblExpSummary.Padding = new Padding(4, 8, 4, 8);
        lblExpSummary.Size = new Size(1100, 40);
        lblExpSummary.Text = "Нажмите «Провести вычислительный эксперимент» для моделирования затрат (1..4 попутчика).";

        var pnlExpBottom = new Panel { Dock = DockStyle.Bottom, Size = new Size(1100, 48) };
        btnSaveExp.BackColor = Color.FromArgb(47, 133, 90);
        btnSaveExp.FlatStyle = FlatStyle.Flat;
        btnSaveExp.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnSaveExp.ForeColor = Color.White;
        btnSaveExp.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnSaveExp.Location = new Point(800, 8);
        btnSaveExp.Size = new Size(290, 34);
        btnSaveExp.Text = "💾 Сохранить матрицу в БД SQLite";
        btnSaveExp.Enabled = false;
        btnSaveExp.Click += BtnSaveExp_Click;
        pnlExpBottom.Controls.Add(btnSaveExp);

        ConfigureGrid(dgvExpGrid);
        dgvExpGrid.Dock = DockStyle.Fill;

        tabExperiment.Controls.AddRange([dgvExpGrid, lblExpSummary, grpExpParams, pnlExpBottom]);

        // ==================== TAB 4: BOOKINGS, AUDIT & EXPORT ====================
        tabManage.BackColor = Color.FromArgb(247, 249, 252);
        tabManage.Padding = new Padding(12);
        tabManage.Text = "📋 Бронирования, аудит и экспорт";

        pnlManageTop.Dock = DockStyle.Top;
        pnlManageTop.Size = new Size(1100, 44);

        btnRefreshManage.Location = new Point(0, 6);
        btnRefreshManage.Size = new Size(150, 32);
        btnRefreshManage.Text = "🔄 Обновить данные";
        btnRefreshManage.Click += (_, _) => LoadManagementData();

        btnCancelBooking.Location = new Point(160, 6);
        btnCancelBooking.Size = new Size(210, 32);
        btnCancelBooking.Text = "❌ Отменить выбранную бронь";
        btnCancelBooking.Click += BtnCancelBooking_Click;

        btnExportTrips.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExportTrips.Location = new Point(710, 6);
        btnExportTrips.Size = new Size(185, 32);
        btnExportTrips.Text = "📥 Экспорт поездок (CSV)";
        btnExportTrips.Click += BtnExportTrips_Click;

        btnExportBookings.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        btnExportBookings.Location = new Point(905, 6);
        btnExportBookings.Size = new Size(195, 32);
        btnExportBookings.Text = "📥 Экспорт броней (CSV)";
        btnExportBookings.Click += BtnExportBookings_Click;

        pnlManageTop.Controls.AddRange([btnRefreshManage, btnCancelBooking, btnExportTrips, btnExportBookings]);

        splitManage.Dock = DockStyle.Fill;
        splitManage.Orientation = Orientation.Horizontal;
        splitManage.SplitterDistance = 285;

        var grpB = new GroupBox { Dock = DockStyle.Fill, Text = "Реестр бронирований пассажиров" };
        ConfigureGrid(dgvBookings);
        dgvBookings.Dock = DockStyle.Fill;
        grpB.Controls.Add(dgvBookings);
        splitManage.Panel1.Controls.Add(grpB);

        var grpA = new GroupBox { Dock = DockStyle.Fill, Text = "Журнал событий аудита системы (AuditLogs)" };
        ConfigureGrid(dgvAudit);
        dgvAudit.Dock = DockStyle.Fill;
        grpA.Controls.Add(dgvAudit);
        splitManage.Panel2.Controls.Add(grpA);

        tabManage.Controls.AddRange([splitManage, pnlManageTop]);

        // statusStrip1
        statusStrip1.Items.Add(lblStatus);
        lblStatus.Text = "Готово. База данных carpooling_v2.db подключена.";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1140, 720);
        MinimumSize = new Size(1060, 650);
        Controls.AddRange([tabMain, pnlHeader, statusStrip1]);
        Font = new Font("Segoe UI", 9.5F);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Сервис поиска попутчиков — Версия 2.0 (Касимов Д.А., ИСП 31-9)";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        tabMain.ResumeLayout(false);
        tabSearch.ResumeLayout(false);
        grpFilter.ResumeLayout(false);
        grpFilter.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTrips).EndInit();
        grpBook.ResumeLayout(false);
        grpBook.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numBookSeats).EndInit();
        tabPublish.ResumeLayout(false);
        grpPublishCard.ResumeLayout(false);
        grpPublishCard.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numPubSeats).EndInit();
        tabExperiment.ResumeLayout(false);
        grpExpParams.ResumeLayout(false);
        grpExpParams.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvExpGrid).EndInit();
        tabManage.ResumeLayout(false);
        pnlManageTop.ResumeLayout(false);
        splitManage.Panel1.ResumeLayout(false);
        splitManage.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)splitManage).EndInit();
        splitManage.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)dgvBookings).EndInit();
        ((System.ComponentModel.ISupportInitialize)dgvAudit).EndInit();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static void ConfigureGrid(DataGridView dgv)
    {
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.BackgroundColor = Color.White;
        dgv.BorderStyle = BorderStyle.Fixed3D;
        dgv.ColumnHeadersHeight = 32;
        dgv.EnableHeadersVisualStyles = false;
        dgv.ColumnHeadersDefaultCellStyle.BackColor = Color.FromArgb(232, 238, 245);
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        dgv.MultiSelect = false;
        dgv.ReadOnly = true;
        dgv.RowHeadersVisible = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
    }

    private static void AddPair(GroupBox grp, string labelText, TextBox tb, int x, int y, string def = "")
    {
        var lbl = new Label { AutoSize = true, Text = labelText, Location = new Point(x, y + 3) };
        tb.Location = new Point(x + 160, y);
        tb.Size = new Size(220, 24);
        tb.Text = def;
        grp.Controls.AddRange([lbl, tb]);
    }

    private static void AddSmallField(GroupBox grp, string labelText, TextBox tb, int x, int y, int width, string def)
    {
        var lbl = new Label { AutoSize = true, Text = labelText, Location = new Point(x, y + 3) };
        tb.Location = new Point(x + 115, y);
        tb.Size = new Size(width, 24);
        tb.Text = def;
        grp.Controls.AddRange([lbl, tb]);
    }
}
