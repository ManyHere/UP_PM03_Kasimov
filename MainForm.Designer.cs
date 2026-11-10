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
    private Label lblSubtitle = null!;
    private GroupBox grpSearch = null!;
    private Label lblFrom = null!;
    private TextBox txtFilterFrom = null!;
    private Label lblTo = null!;
    private TextBox txtFilterTo = null!;
    private Button btnSearch = null!;
    private Button btnReset = null!;
    private DataGridView dgvTrips = null!;
    private GroupBox grpAddTrip = null!;
    private TextBox txtDriver = null!;
    private TextBox txtPhone = null!;
    private TextBox txtCar = null!;
    private TextBox txtAddFrom = null!;
    private TextBox txtAddTo = null!;
    private TextBox txtDate = null!;
    private TextBox txtTime = null!;
    private NumericUpDown numSeats = null!;
    private TextBox txtAddDist = null!;
    private TextBox txtAddPrice = null!;
    private Button btnAddTrip = null!;
    private GroupBox grpExperiment = null!;
    private TextBox txtExpRoute = null!;
    private TextBox txtExpDist = null!;
    private TextBox txtExpCons = null!;
    private TextBox txtExpFuelPrice = null!;
    private TextBox txtExpDeprec = null!;
    private NumericUpDown numExpPassengers = null!;
    private Button btnCalcExperiment = null!;
    private Label lblExpOutput = null!;
    private StatusStrip statusStrip1 = null!;
    private ToolStripStatusLabel lblStatus = null!;

    private void InitializeComponent()
    {
        pnlHeader = new Panel();
        lblSubtitle = new Label();
        lblTitle = new Label();
        grpSearch = new GroupBox();
        btnReset = new Button();
        btnSearch = new Button();
        txtFilterTo = new TextBox();
        lblTo = new Label();
        txtFilterFrom = new TextBox();
        lblFrom = new Label();
        dgvTrips = new DataGridView();
        grpAddTrip = new GroupBox();
        btnAddTrip = new Button();
        txtAddPrice = new TextBox();
        txtAddDist = new TextBox();
        numSeats = new NumericUpDown();
        txtTime = new TextBox();
        txtDate = new TextBox();
        txtAddTo = new TextBox();
        txtAddFrom = new TextBox();
        txtCar = new TextBox();
        txtPhone = new TextBox();
        txtDriver = new TextBox();
        grpExperiment = new GroupBox();
        lblExpOutput = new Label();
        btnCalcExperiment = new Button();
        numExpPassengers = new NumericUpDown();
        txtExpDeprec = new TextBox();
        txtExpFuelPrice = new TextBox();
        txtExpCons = new TextBox();
        txtExpDist = new TextBox();
        txtExpRoute = new TextBox();
        statusStrip1 = new StatusStrip();
        lblStatus = new ToolStripStatusLabel();

        pnlHeader.SuspendLayout();
        grpSearch.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTrips).BeginInit();
        grpAddTrip.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numSeats).BeginInit();
        grpExperiment.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)numExpPassengers).BeginInit();
        statusStrip1.SuspendLayout();
        SuspendLayout();

        // pnlHeader
        pnlHeader.BackColor = Color.FromArgb(30, 58, 95);
        pnlHeader.Controls.Add(lblSubtitle);
        pnlHeader.Controls.Add(lblTitle);
        pnlHeader.Dock = DockStyle.Top;
        pnlHeader.Location = new Point(0, 0);
        pnlHeader.Name = "pnlHeader";
        pnlHeader.Size = new Size(1084, 54);
        pnlHeader.TabIndex = 0;

        lblTitle.AutoSize = true;
        lblTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
        lblTitle.ForeColor = Color.White;
        lblTitle.Location = new Point(16, 14);
        lblTitle.Name = "lblTitle";
        lblTitle.Size = new Size(540, 25);
        lblTitle.Text = "СЕРВИС ПОИСКА ПОПУТЧИКОВ — Версия 1.0 (Базовая)";

        lblSubtitle.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        lblSubtitle.AutoSize = true;
        lblSubtitle.Font = new Font("Segoe UI", 9.5F);
        lblSubtitle.ForeColor = Color.LightSteelBlue;
        lblSubtitle.Location = new Point(820, 18);
        lblSubtitle.Name = "lblSubtitle";
        lblSubtitle.Size = new Size(245, 17);
        lblSubtitle.Text = "Студент: Касимов Д.А. | Группа ИСП 31-9";

        // grpSearch
        grpSearch.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
        grpSearch.Location = new Point(12, 64);
        grpSearch.Name = "grpSearch";
        grpSearch.Size = new Size(680, 62);
        grpSearch.Text = "Фильтр поездок";

        lblFrom.AutoSize = true;
        lblFrom.Location = new Point(14, 28);
        lblFrom.Text = "Откуда:";
        txtFilterFrom.Location = new Point(72, 25);
        txtFilterFrom.Size = new Size(140, 23);
        txtFilterFrom.Text = "Ижевск";

        lblTo.AutoSize = true;
        lblTo.Location = new Point(226, 28);
        lblTo.Text = "Куда:";
        txtFilterTo.Location = new Point(268, 25);
        txtFilterTo.Size = new Size(140, 23);

        btnSearch.Location = new Point(424, 23);
        btnSearch.Size = new Size(115, 28);
        btnSearch.Text = "Найти рейсы";
        btnSearch.UseVisualStyleBackColor = true;
        btnSearch.Click += BtnSearch_Click;

        btnReset.Location = new Point(548, 23);
        btnReset.Size = new Size(115, 28);
        btnReset.Text = "Сбросить";
        btnReset.UseVisualStyleBackColor = true;
        btnReset.Click += BtnReset_Click;

        grpSearch.Controls.AddRange([lblFrom, txtFilterFrom, lblTo, txtFilterTo, btnSearch, btnReset]);

        // dgvTrips
        dgvTrips.AllowUserToAddRows = false;
        dgvTrips.AllowUserToDeleteRows = false;
        dgvTrips.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        dgvTrips.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgvTrips.BackgroundColor = Color.White;
        dgvTrips.Location = new Point(12, 134);
        dgvTrips.MultiSelect = false;
        dgvTrips.Name = "dgvTrips";
        dgvTrips.ReadOnly = true;
        dgvTrips.RowHeadersVisible = false;
        dgvTrips.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgvTrips.Size = new Size(680, 480);

        // grpAddTrip
        grpAddTrip.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        grpAddTrip.Location = new Point(704, 64);
        grpAddTrip.Name = "grpAddTrip";
        grpAddTrip.Size = new Size(368, 282);
        grpAddTrip.Text = "Добавить новую поездку (БД SQLite)";

        AddFormRow(grpAddTrip, "Водитель:", txtDriver, 24, "Касимов Дмитрий");
        AddFormRow(grpAddTrip, "Телефон:", txtPhone, 52, "+7 (912) 850-12-34");
        AddFormRow(grpAddTrip, "Автомобиль:", txtCar, 80, "Lada Vesta");
        AddFormRow(grpAddTrip, "Откуда:", txtAddFrom, 108, "Ижевск");
        AddFormRow(grpAddTrip, "Куда:", txtAddTo, 136, "Сарапул");
        AddFormRow(grpAddTrip, "Дата / Время:", txtDate, 164, "2026-11-15", 105);
        txtTime.Location = new Point(236, 161);
        txtTime.Size = new Size(116, 23);
        txtTime.Text = "10:00";
        grpAddTrip.Controls.Add(txtTime);

        var lblSeats = new Label { AutoSize = true, Text = "Мест / Км / Цена:", Location = new Point(12, 194) };
        numSeats.Location = new Point(125, 191);
        numSeats.Minimum = 1;
        numSeats.Maximum = 8;
        numSeats.Value = 3;
        numSeats.Size = new Size(50, 23);

        txtAddDist.Location = new Point(182, 191);
        txtAddDist.Size = new Size(80, 23);
        txtAddDist.Text = "65";

        txtAddPrice.Location = new Point(270, 191);
        txtAddPrice.Size = new Size(82, 23);
        txtAddPrice.Text = "220";

        btnAddTrip.BackColor = Color.FromArgb(47, 133, 90);
        btnAddTrip.ForeColor = Color.White;
        btnAddTrip.FlatStyle = FlatStyle.Flat;
        btnAddTrip.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnAddTrip.Location = new Point(14, 232);
        btnAddTrip.Size = new Size(338, 34);
        btnAddTrip.Text = "Сохранить поездку в базу данных";
        btnAddTrip.Click += BtnAddTrip_Click;

        grpAddTrip.Controls.AddRange([lblSeats, numSeats, txtAddDist, txtAddPrice, btnAddTrip]);

        // grpExperiment
        grpExperiment.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Right;
        grpExperiment.Location = new Point(704, 354);
        grpExperiment.Name = "grpExperiment";
        grpExperiment.Size = new Size(368, 260);
        grpExperiment.Text = "Вычислительный эксперимент (себестоимость)";

        AddFormRow(grpExperiment, "Маршрут:", txtExpRoute, 24, "Ижевск — Казань");
        AddFormRow(grpExperiment, "Дистанция (км):", txtExpDist, 52, "390");
        AddFormRow(grpExperiment, "Расход (л/100км):", txtExpCons, 80, "8.5");
        AddFormRow(grpExperiment, "Цена (руб/л):", txtExpFuelPrice, 108, "58.5");
        AddFormRow(grpExperiment, "Аморт. (руб/км):", txtExpDeprec, 136, "1.8");

        var lblPass = new Label { AutoSize = true, Text = "Попутчиков (N):", Location = new Point(12, 166) };
        numExpPassengers.Location = new Point(125, 163);
        numExpPassengers.Minimum = 1;
        numExpPassengers.Maximum = 6;
        numExpPassengers.Value = 3;
        numExpPassengers.Size = new Size(65, 23);

        btnCalcExperiment.BackColor = Color.FromArgb(43, 108, 176);
        btnCalcExperiment.ForeColor = Color.White;
        btnCalcExperiment.FlatStyle = FlatStyle.Flat;
        btnCalcExperiment.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
        btnCalcExperiment.Location = new Point(200, 159);
        btnCalcExperiment.Size = new Size(152, 30);
        btnCalcExperiment.Text = "Рассчитать";
        btnCalcExperiment.Click += BtnCalcExperiment_Click;

        lblExpOutput.BorderStyle = BorderStyle.FixedSingle;
        lblExpOutput.BackColor = Color.WhiteSmoke;
        lblExpOutput.Location = new Point(12, 196);
        lblExpOutput.Size = new Size(340, 54);
        lblExpOutput.Text = "Нажмите «Рассчитать» для проведения эксперимента.";

        grpExperiment.Controls.AddRange([lblPass, numExpPassengers, btnCalcExperiment, lblExpOutput]);

        // statusStrip1
        statusStrip1.Items.Add(lblStatus);
        statusStrip1.Location = new Point(0, 622);
        statusStrip1.Name = "statusStrip1";
        statusStrip1.Size = new Size(1084, 22);
        lblStatus.Text = "База данных carpooling_v1.db подключена.";

        AutoScaleDimensions = new SizeF(7F, 15F);
        AutoScaleMode = AutoScaleMode.Font;
        ClientSize = new Size(1084, 644);
        MinimumSize = new Size(980, 620);
        Controls.AddRange([pnlHeader, grpSearch, dgvTrips, grpAddTrip, grpExperiment, statusStrip1]);
        Font = new Font("Segoe UI", 9.5F);
        Name = "MainForm";
        StartPosition = FormStartPosition.CenterScreen;
        Text = "Сервис поиска попутчиков — Версия 1.0 (Касимов Д.А., ИСП 31-9)";

        pnlHeader.ResumeLayout(false);
        pnlHeader.PerformLayout();
        grpSearch.ResumeLayout(false);
        grpSearch.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)dgvTrips).EndInit();
        grpAddTrip.ResumeLayout(false);
        grpAddTrip.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numSeats).EndInit();
        grpExperiment.ResumeLayout(false);
        grpExperiment.PerformLayout();
        ((System.ComponentModel.ISupportInitialize)numExpPassengers).EndInit();
        statusStrip1.ResumeLayout(false);
        statusStrip1.PerformLayout();
        ResumeLayout(false);
        PerformLayout();
    }

    private static void AddFormRow(GroupBox grp, string labelText, TextBox tb, int y, string defaultVal, int width = 227)
    {
        var lbl = new Label { AutoSize = true, Text = labelText, Location = new Point(12, y) };
        tb.Location = new Point(125, y - 3);
        tb.Size = new Size(width, 23);
        tb.Text = defaultVal;
        grp.Controls.Add(lbl);
        grp.Controls.Add(tb);
    }
}
