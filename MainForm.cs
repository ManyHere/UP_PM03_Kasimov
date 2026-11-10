using System.Globalization;

namespace CarpoolingApp;

public partial class MainForm : Form
{
    private readonly UserAccount _currentUser;
    private int? _selectedTripId;
    private List<ExperimentResult>? _lastExperimentGrid;
    private double _lastDist, _lastCons, _lastFuelPrice, _lastDeprec;
    private string _lastRoute = string.Empty;

    public MainForm(UserAccount currentUser)
    {
        _currentUser = currentUser;
        InitializeComponent();

        lblUserBadge.Text = $"{_currentUser.FullName} ({_currentUser.Role})";
        txtBookName.Text = _currentUser.FullName;
        txtBookPhone.Text = _currentUser.Phone;
        txtPubDriver.Text = _currentUser.FullName;
        txtPubPhone.Text = _currentUser.Phone;

        LoadTrips();
        LoadManagementData();
        RunExperimentAndBind();
    }

    private void BtnSwitchUser_Click(object? sender, EventArgs e)
    {
        DialogResult = DialogResult.Retry;
        Close();
    }

    private void LoadTrips()
    {
        var trips = DatabaseHelper.GetTrips(txtFilterFrom.Text, txtFilterTo.Text);
        dgvTrips.DataSource = null;
        dgvTrips.DataSource = trips.Select(t => new
        {
            ID = t.Id,
            Маршрут = t.Route,
            ДатаВремя = t.DateTimeText,
            Автомобиль = t.CarModel,
            Водитель = t.DriverName,
            Телефон = t.DriverPhone,
            Мест = t.SeatsAvailable,
            Дистанция = $"{t.DistanceKm:F0} км",
            Цена = $"{t.PricePerSeat:F0} руб."
        }).ToList();

        if (dgvTrips.Columns["ID"] is not null) dgvTrips.Columns["ID"]!.FillWeight = 32;
        if (dgvTrips.Columns["Маршрут"] is not null) dgvTrips.Columns["Маршрут"]!.FillWeight = 135;
        if (dgvTrips.Columns["ДатаВремя"] is not null)
        {
            dgvTrips.Columns["ДатаВремя"]!.HeaderText = "Дата и время";
            dgvTrips.Columns["ДатаВремя"]!.FillWeight = 95;
        }
        if (dgvTrips.Columns["Мест"] is not null) dgvTrips.Columns["Мест"]!.FillWeight = 45;
        if (dgvTrips.Columns["Дистанция"] is not null) dgvTrips.Columns["Дистанция"]!.FillWeight = 65;
        if (dgvTrips.Columns["Цена"] is not null) dgvTrips.Columns["Цена"]!.FillWeight = 70;

        _selectedTripId = null;
        btnBookSeat.Enabled = false;
        lblSelectedTrip.Text = "Выберите поездку из таблицы выше для бронирования места.";
        lblStatus.Text = $"Загружено доступных маршрутов: {trips.Count}";
    }

    private void DgvTrips_SelectionChanged(object? sender, EventArgs e)
    {
        if (dgvTrips.CurrentRow is null)
            return;

        object? idVal = dgvTrips.CurrentRow.Cells["ID"].Value;
        if (idVal is null || !int.TryParse(idVal.ToString(), out int tripId))
            return;

        _selectedTripId = tripId;
        string route = dgvTrips.CurrentRow.Cells["Маршрут"].Value?.ToString() ?? "";
        string dt = dgvTrips.CurrentRow.Cells["ДатаВремя"].Value?.ToString() ?? "";
        string price = dgvTrips.CurrentRow.Cells["Цена"].Value?.ToString() ?? "";
        int seats = Convert.ToInt32(dgvTrips.CurrentRow.Cells["Мест"].Value ?? 1);

        lblSelectedTrip.Text = $"Выбран рейс #{tripId}: {route} ({dt}) | Тариф: {price} | Свободно мест: {seats}";
        numBookSeats.Maximum = Math.Max(1, seats);
        if (numBookSeats.Value > numBookSeats.Maximum)
            numBookSeats.Value = numBookSeats.Maximum;
        btnBookSeat.Enabled = true;
    }

    private void BtnBookSeat_Click(object? sender, EventArgs e)
    {
        if (_selectedTripId is null)
            return;

        string name = txtBookName.Text.Trim();
        string phone = txtBookPhone.Text.Trim();
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(phone))
        {
            MessageBox.Show("Укажите ФИО и контактный телефон пассажира.", "Проверка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        try
        {
            var (bookingId, totalCost) = DatabaseHelper.BookSeat(_selectedTripId.Value, name, phone, (int)numBookSeats.Value);
            MessageBox.Show(
                $"Бронь #{bookingId} успешно подтверждена!\nПассажир: {name}\nМест: {numBookSeats.Value}\nСумма к оплате водителю: {totalCost:F2} руб.",
                "Успешное бронирование",
                MessageBoxButtons.OK,
                MessageBoxIcon.Information);

            LoadTrips();
            LoadManagementData();
            lblStatus.Text = $"Оформлено бронирование #{bookingId}.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка бронирования", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnFillSample_Click(object? sender, EventArgs e)
    {
        txtPubFrom.Text = "Ижевск";
        txtPubTo.Text = "Сарапул";
        txtPubDist.Text = "65";
        txtPubPrice.Text = "220";
        numPubSeats.Value = 4;
    }

    private void BtnPublishTrip_Click(object? sender, EventArgs e)
    {
        try
        {
            var trip = new TripItem
            {
                DriverName = txtPubDriver.Text.Trim(),
                DriverPhone = txtPubPhone.Text.Trim(),
                CarModel = txtPubCar.Text.Trim(),
                FromCity = txtPubFrom.Text.Trim(),
                ToCity = txtPubTo.Text.Trim(),
                TripDate = txtPubDate.Text.Trim(),
                TripTime = txtPubTime.Text.Trim(),
                SeatsTotal = (int)numPubSeats.Value,
                DistanceKm = ParsePositiveDouble(txtPubDist.Text, "Дистанция"),
                PricePerSeat = ParsePositiveDouble(txtPubPrice.Text, "Цена за место", allowZero: true)
            };

            if (string.IsNullOrWhiteSpace(trip.DriverName) || string.IsNullOrWhiteSpace(trip.FromCity) || string.IsNullOrWhiteSpace(trip.ToCity))
                throw new ArgumentException("Заполните ФИО водителя, город отправления и город назначения.");

            int newId = DatabaseHelper.AddTrip(trip);
            MessageBox.Show($"Поездка #{newId} ({trip.Route}) успешно опубликована!", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);

            LoadTrips();
            LoadManagementData();
            tabMain.SelectedTab = tabSearch;
            lblStatus.Text = $"Опубликован новый маршрут #{newId}: {trip.Route}";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnRunExp_Click(object? sender, EventArgs e) => RunExperimentAndBind();

    private void RunExperimentAndBind()
    {
        try
        {
            string route = txtExpRoute.Text.Trim();
            double dist = ParsePositiveDouble(txtExpDist.Text, "Дистанция");
            double cons = ParsePositiveDouble(txtExpCons.Text, "Расход топлива");
            double fuelPrice = ParsePositiveDouble(txtExpFuelPrice.Text, "Цена топлива");
            double deprec = ParsePositiveDouble(txtExpDeprec.Text, "Амортизация", allowZero: true);
            int tripsPerYear = (int)ParsePositiveDouble(txtExpTripsYear.Text, "Поездок в год");

            double cFuel = ExperimentCalculator.CalculateFuelCost(dist, cons, fuelPrice);
            double cDeprec = ExperimentCalculator.CalculateDepreciationCost(dist, deprec);
            double cTotal = ExperimentCalculator.CalculateTotalTripCost(dist, cons, fuelPrice, deprec);
            var grid = ExperimentCalculator.RunExperimentGrid(dist, cons, fuelPrice, deprec, tripsPerYear);

            _lastRoute = route;
            _lastDist = dist;
            _lastCons = cons;
            _lastFuelPrice = fuelPrice;
            _lastDeprec = deprec;
            _lastExperimentGrid = grid;

            lblExpSummary.Text =
                $"Маршрут: {route} ({dist:F0} км) | Себестоимость рейса: {cTotal:F2} руб. " +
                $"[Топливо: {cFuel:F2} руб. + Амортизация: {cDeprec:F2} руб.] | 1 км: {(cTotal / dist):F2} руб./км";

            dgvExpGrid.DataSource = null;
            dgvExpGrid.DataSource = grid.Select(r => new
            {
                Попутчиков = $"{r.PassengersCount} чел.",
                ВсегоЛюдей = $"{r.TotalPersons} чел. (с водителем)",
                ДоляНаЧеловека = $"{r.CostPerPerson:F2} руб.",
                ЭкономияВодителя = $"{r.DriverSavings:F2} руб.",
                ПокрытиеЗатрат = $"{r.CoveragePercent:F1}%",
                ГодоваяЭкономия = $"{r.AnnualSavings:F2} руб."
            }).ToList();

            dgvExpGrid.Columns["ВсегоЛюдей"]!.HeaderText = "Всего людей в авто";
            dgvExpGrid.Columns["ДоляНаЧеловека"]!.HeaderText = "Доля на 1 человека";
            dgvExpGrid.Columns["ЭкономияВодителя"]!.HeaderText = "Экономия водителя";
            dgvExpGrid.Columns["ПокрытиеЗатрат"]!.HeaderText = "Покрытие затрат";
            dgvExpGrid.Columns["ГодоваяЭкономия"]!.HeaderText = $"Прогноз за год ({tripsPerYear} рейсов)";

            btnSaveExp.Enabled = true;
            lblStatus.Text = $"Вычислительный эксперимент для маршрута «{route}» выполнен.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка моделирования", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void BtnSaveExp_Click(object? sender, EventArgs e)
    {
        if (_lastExperimentGrid is null)
            return;

        DatabaseHelper.SaveExperimentGrid(_lastRoute, _lastDist, _lastCons, _lastFuelPrice, _lastDeprec, _lastExperimentGrid);
        MessageBox.Show("Матрица расчетов успешно сохранена в таблицу CostExperiments БД SQLite.", "Сохранено", MessageBoxButtons.OK, MessageBoxIcon.Information);
        btnSaveExp.Enabled = false;
        LoadManagementData();
        lblStatus.Text = "Результаты эксперимента сохранены в БД.";
    }

    private void LoadManagementData()
    {
        var bookings = DatabaseHelper.GetBookings();
        dgvBookings.DataSource = null;
        dgvBookings.DataSource = bookings.Select(b => new
        {
            ID = b.Id,
            Рейс = b.TripId,
            Маршрут = b.Route,
            Пассажир = b.PassengerName,
            Телефон = b.PassengerPhone,
            Мест = b.SeatsBooked,
            Сумма = $"{b.TotalCost:F0} руб.",
            Дата = b.BookingDate,
            Статус = b.Status
        }).ToList();

        if (dgvBookings.Columns["ID"] is not null) dgvBookings.Columns["ID"]!.FillWeight = 35;
        if (dgvBookings.Columns["Рейс"] is not null) dgvBookings.Columns["Рейс"]!.FillWeight = 45;
        if (dgvBookings.Columns["Мест"] is not null) dgvBookings.Columns["Мест"]!.FillWeight = 45;

        var logs = DatabaseHelper.GetAuditLogs();
        dgvAudit.DataSource = null;
        dgvAudit.DataSource = logs.Select(l => new
        {
            ID = l.Id,
            Событие = l.EventType,
            Описание = l.Description,
            Время = l.CreatedAt
        }).ToList();

        if (dgvAudit.Columns["ID"] is not null) dgvAudit.Columns["ID"]!.FillWeight = 30;
        if (dgvAudit.Columns["Событие"] is not null) dgvAudit.Columns["Событие"]!.FillWeight = 85;
        if (dgvAudit.Columns["Описание"] is not null) dgvAudit.Columns["Описание"]!.FillWeight = 230;
        if (dgvAudit.Columns["Время"] is not null) dgvAudit.Columns["Время"]!.FillWeight = 90;
    }

    private void BtnCancelBooking_Click(object? sender, EventArgs e)
    {
        if (dgvBookings.CurrentRow is null)
        {
            MessageBox.Show("Выберите строку бронирования для отмены.", "Внимание", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int bookingId = Convert.ToInt32(dgvBookings.CurrentRow.Cells["ID"].Value);
        if (MessageBox.Show($"Отменить бронирование #{bookingId} и вернуть места в рейс?", "Подтверждение", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
            return;

        try
        {
            DatabaseHelper.CancelBooking(bookingId);
            MessageBox.Show($"Бронирование #{bookingId} отменено. Места возвращены в поездку.", "Готово", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LoadTrips();
            LoadManagementData();
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка отмены", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnExportTrips_Click(object? sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "CSV файл (*.csv)|*.csv",
            FileName = "trips_export.csv",
            Title = "Экспорт поездок в CSV"
        };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            int cnt = DatabaseHelper.ExportTripsToCsv(sfd.FileName);
            MessageBox.Show($"Экспортировано маршрутов: {cnt}\nФайл: {sfd.FileName}", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private void BtnExportBookings_Click(object? sender, EventArgs e)
    {
        using var sfd = new SaveFileDialog
        {
            Filter = "CSV файл (*.csv)|*.csv",
            FileName = "bookings_export.csv",
            Title = "Экспорт бронирований в CSV"
        };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            int cnt = DatabaseHelper.ExportBookingsToCsv(sfd.FileName);
            MessageBox.Show($"Экспортировано бронирований: {cnt}\nФайл: {sfd.FileName}", "Экспорт завершен", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }

    private static double ParsePositiveDouble(string raw, string fieldName, bool allowZero = false)
    {
        string normalized = raw.Trim().Replace(',', '.');
        if (!double.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out double val))
            throw new ArgumentException($"Поле «{fieldName}» должно содержать корректное число.");
        if (allowZero ? val < 0 : val <= 0)
            throw new ArgumentException($"Поле «{fieldName}» должно быть положительным.");
        return val;
    }
}
