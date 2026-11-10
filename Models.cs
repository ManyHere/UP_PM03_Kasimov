namespace CarpoolingApp;

/// <summary>
/// Учетная запись пользователя системы (водитель или пассажир).
/// </summary>
public class UserAccount
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Login { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}

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

    public string Route => $"{FromCity} → {ToCity}";
    public string DateTimeText => $"{TripDate} {TripTime}";
}

/// <summary>
/// Модель бронирования места пассажиром.
/// </summary>
public class BookingItem
{
    public int Id { get; set; }
    public int TripId { get; set; }
    public string Route { get; set; } = string.Empty;
    public string PassengerName { get; set; } = string.Empty;
    public string PassengerPhone { get; set; } = string.Empty;
    public int SeatsBooked { get; set; }
    public double TotalCost { get; set; }
    public string BookingDate { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}

/// <summary>
/// Запись системного журнала аудита.
/// </summary>
public class AuditLogItem
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string CreatedAt { get; set; } = string.Empty;
}

/// <summary>
/// Результат вычислительного эксперимента по распределению транспортных затрат (Версия 2.0).
/// </summary>
public class ExperimentResult
{
    public int PassengersCount { get; set; }
    public int TotalPersons { get; set; }
    public double FuelCost { get; set; }
    public double DepreciationCost { get; set; }
    public double TotalCost { get; set; }
    public double CostPerKm { get; set; }
    public double CostPerPerson { get; set; }
    public double DriverSavings { get; set; }
    public double CoveragePercent { get; set; }
    public double AnnualSavings { get; set; }
}
