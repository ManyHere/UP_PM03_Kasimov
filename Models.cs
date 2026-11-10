namespace CarpoolingApp;

/// <summary>
/// Модель поездки (маршрута) в базе данных.
/// </summary>
public class TripItem
{
    public int Id { get; set; }
    public string DriverName { get; set; } = string.Empty;
    public string DriverPhone { get; set; } = string.Empty;
    public string CarModel { get; set; } = string.Empty;
    public string FromCity { get; set; } = string.Empty;
    public string ToCity { get; set; } = string.Empty;
    public string TripDate { get; set; } = string.Empty;
    public string TripTime { get; set; } = string.Empty;
    public int SeatsTotal { get; set; }
    public int SeatsAvailable { get; set; }
    public double DistanceKm { get; set; }
    public double PricePerSeat { get; set; }

    public string Route => $"{FromCity} -> {ToCity}";
    public string DateTimeText => $"{TripDate} {TripTime}";
}

/// <summary>
/// Результат вычислительного эксперимента по распределению транспортных затрат.
/// </summary>
public class ExperimentResult
{
    public int PassengersCount { get; set; }
    public int TotalPersons { get; set; }
    public double FuelCost { get; set; }
    public double DepreciationCost { get; set; }
    public double TotalCost { get; set; }
    public double CostPerPerson { get; set; }
    public double DriverSavings { get; set; }
    public double CoveragePercent { get; set; }
}
