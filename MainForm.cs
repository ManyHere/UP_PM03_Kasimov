using System.Globalization;

namespace CarpoolingApp;

public partial class MainForm : Form
{
    public MainForm()
    {
        InitializeComponent();
        LoadTrips();
    }

    private void LoadTrips()
    {
        var trips = DatabaseHelper.GetTrips(txtFilterFrom.Text, txtFilterTo.Text);
        dgvTrips.DataSource = null;
        dgvTrips.DataSource = trips.Select(t => new
        {
            ID = t.Id,
            Маршрут = t.Route,
            Время = t.DateTimeText,
            Авто = t.CarModel,
            Водитель = t.DriverName,
            Мест = t.SeatsAvailable,
            Дистанция = $"{t.DistanceKm:F0} км",
            Цена = $"{t.PricePerSeat:F0} руб."
        }).ToList();

        if (dgvTrips.Columns["ID"] is not null)
        {
            dgvTrips.Columns["ID"]!.FillWeight = 28;
        }

        lblStatus.Text = $"Загружено поездок из SQLite: {trips.Count}";
    }

    private void BtnSearch_Click(object? sender, EventArgs e) => LoadTrips();

    private void BtnReset_Click(object? sender, EventArgs e)
    {
        txtFilterFrom.Clear();
        txtFilterTo.Clear();
        LoadTrips();
    }

    private void BtnAddTrip_Click(object? sender, EventArgs e)
    {
        try
        {
            double dist = ParsePositiveDouble(txtAddDist.Text, "Дистанция");
            double price = ParsePositiveDouble(txtAddPrice.Text, "Цена");

            var trip = new TripItem
            {
                DriverName = txtDriver.Text.Trim(),
                DriverPhone = txtPhone.Text.Trim(),
                CarModel = txtCar.Text.Trim(),
                FromCity = txtAddFrom.Text.Trim(),
                ToCity = txtAddTo.Text.Trim(),
                TripDate = txtDate.Text.Trim(),
                TripTime = txtTime.Text.Trim(),
                SeatsTotal = (int)numSeats.Value,
                DistanceKm = dist,
                PricePerSeat = price
            };

            if (string.IsNullOrWhiteSpace(trip.DriverName) || string.IsNullOrWhiteSpace(trip.FromCity) || string.IsNullOrWhiteSpace(trip.ToCity))
                throw new ArgumentException("Заполните ФИО водителя и города маршрута.");

            DatabaseHelper.AddTrip(trip);
            LoadTrips();
            MessageBox.Show("Поездка добавлена в базу данных SQLite.", "Успех", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка ввода", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
    }

    private void BtnCalcExperiment_Click(object? sender, EventArgs e)
    {
        try
        {
            double dist = ParsePositiveDouble(txtExpDist.Text, "Дистанция");
            double cons = ParsePositiveDouble(txtExpCons.Text, "Расход");
            double fuelPrice = ParsePositiveDouble(txtExpFuelPrice.Text, "Цена топлива");
            double deprec = ParsePositiveDouble(txtExpDeprec.Text, "Амортизация", allowZero: true);
            int passCount = (int)numExpPassengers.Value;

            ExperimentResult res = ExperimentCalculator.CalculateCostSharing(dist, cons, fuelPrice, deprec, passCount);
            DatabaseHelper.SaveExperiment(txtExpRoute.Text.Trim(), dist, cons, fuelPrice, deprec, res);

            lblExpOutput.Text = $"Полная себестоимость: {res.TotalCost:F2} руб. (Топливо: {res.FuelCost:F2})\n" +
                                $"Доля на 1 чел. ({res.TotalPersons} чел.): {res.CostPerPerson:F2} руб. | Экономия водителя: {res.DriverSavings:F2} руб.";
            lblStatus.Text = "Эксперимент рассчитан и сохранен в CostExperiments.";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "Ошибка расчета", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
