using Microsoft.Data.Sqlite;

namespace CarpoolingApp;

/// <summary>
/// Модуль работы с базой данных SQLite для сервиса поиска попутчиков (Версия 1.0).
/// </summary>
public static class DatabaseHelper
{
    public const string DbFileName = "carpooling_v1.db";
    private static string ConnectionString => $"Data Source={DbFileName}";

    public static void InitializeDatabase()
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Trips (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                DriverName TEXT NOT NULL,
                DriverPhone TEXT NOT NULL,
                CarModel TEXT NOT NULL,
                FromCity TEXT NOT NULL,
                ToCity TEXT NOT NULL,
                TripDate TEXT NOT NULL,
                TripTime TEXT NOT NULL,
                SeatsTotal INTEGER NOT NULL CHECK (SeatsTotal > 0),
                SeatsAvailable INTEGER NOT NULL CHECK (SeatsAvailable >= 0),
                DistanceKm REAL NOT NULL CHECK (DistanceKm > 0),
                PricePerSeat REAL NOT NULL CHECK (PricePerSeat >= 0)
            );

            CREATE TABLE IF NOT EXISTS CostExperiments (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                RouteName TEXT NOT NULL,
                DistanceKm REAL NOT NULL,
                FuelConsumption REAL NOT NULL,
                FuelPrice REAL NOT NULL,
                DepreciationPerKm REAL NOT NULL,
                PassengersCount INTEGER NOT NULL,
                TotalCost REAL NOT NULL,
                CostPerPerson REAL NOT NULL,
                DriverSavings REAL NOT NULL,
                CreatedAt TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT COUNT(*) FROM Trips;";
        long count = Convert.ToInt64(cmd.ExecuteScalar());
        if (count == 0)
        {
            SeedInitialTrips(conn);
        }
    }

    private static void SeedInitialTrips(SqliteConnection conn)
    {
        var initial = new[]
        {
            ("Касимов Дмитрий", "+7 (912) 850-12-34", "Lada Vesta", "Ижевск", "Казань", "2026-11-12", "08:00", 3, 390.0, 950.0),
            ("Иванов Алексей", "+7 (909) 111-22-33", "Skoda Octavia", "Ижевск", "Пермь", "2026-11-13", "09:30", 4, 280.0, 750.0),
            ("Петров Сергей", "+7 (950) 444-55-66", "Kia Rio", "Ижевск", "Воткинск", "2026-11-12", "14:00", 3, 58.0, 200.0),
            ("Смирнова Анна", "+7 (919) 777-88-99", "Hyundai Solaris", "Ижевск", "Набережные Челны", "2026-11-14", "11:00", 3, 200.0, 550.0)
        };

        foreach (var t in initial)
        {
            using var insert = conn.CreateCommand();
            insert.CommandText = """
                INSERT INTO Trips (DriverName, DriverPhone, CarModel, FromCity, ToCity, TripDate, TripTime, SeatsTotal, SeatsAvailable, DistanceKm, PricePerSeat)
                VALUES ($driver, $phone, $car, $from, $to, $date, $time, $seats, $seats, $dist, $price);
                """;
            insert.Parameters.AddWithValue("$driver", t.Item1);
            insert.Parameters.AddWithValue("$phone", t.Item2);
            insert.Parameters.AddWithValue("$car", t.Item3);
            insert.Parameters.AddWithValue("$from", t.Item4);
            insert.Parameters.AddWithValue("$to", t.Item5);
            insert.Parameters.AddWithValue("$date", t.Item6);
            insert.Parameters.AddWithValue("$time", t.Item7);
            insert.Parameters.AddWithValue("$seats", t.Item8);
            insert.Parameters.AddWithValue("$dist", t.Item9);
            insert.Parameters.AddWithValue("$price", t.Item10);
            insert.ExecuteNonQuery();
        }
    }

    public static List<TripItem> GetTrips(string? fromCity = null, string? toCity = null)
    {
        var result = new List<TripItem>();
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        var sql = """
            SELECT Id, DriverName, DriverPhone, CarModel, FromCity, ToCity, TripDate, TripTime, SeatsTotal, SeatsAvailable, DistanceKm, PricePerSeat
            FROM Trips
            WHERE SeatsAvailable > 0
            """;

        if (!string.IsNullOrWhiteSpace(fromCity))
        {
            sql += " AND LOWER(FromCity) LIKE LOWER($from)";
            cmd.Parameters.AddWithValue("$from", $"%{fromCity.Trim()}%");
        }
        if (!string.IsNullOrWhiteSpace(toCity))
        {
            sql += " AND LOWER(ToCity) LIKE LOWER($to)";
            cmd.Parameters.AddWithValue("$to", $"%{toCity.Trim()}%");
        }

        sql += " ORDER BY TripDate ASC, TripTime ASC;";
        cmd.CommandText = sql;

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            result.Add(new TripItem
            {
                Id = reader.GetInt32(0),
                DriverName = reader.GetString(1),
                DriverPhone = reader.GetString(2),
                CarModel = reader.GetString(3),
                FromCity = reader.GetString(4),
                ToCity = reader.GetString(5),
                TripDate = reader.GetString(6),
                TripTime = reader.GetString(7),
                SeatsTotal = reader.GetInt32(8),
                SeatsAvailable = reader.GetInt32(9),
                DistanceKm = reader.GetDouble(10),
                PricePerSeat = reader.GetDouble(11)
            });
        }

        return result;
    }

    public static void AddTrip(TripItem trip)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Trips (DriverName, DriverPhone, CarModel, FromCity, ToCity, TripDate, TripTime, SeatsTotal, SeatsAvailable, DistanceKm, PricePerSeat)
            VALUES ($driver, $phone, $car, $from, $to, $date, $time, $seats, $seats, $dist, $price);
            """;
        cmd.Parameters.AddWithValue("$driver", trip.DriverName);
        cmd.Parameters.AddWithValue("$phone", trip.DriverPhone);
        cmd.Parameters.AddWithValue("$car", trip.CarModel);
        cmd.Parameters.AddWithValue("$from", trip.FromCity);
        cmd.Parameters.AddWithValue("$to", trip.ToCity);
        cmd.Parameters.AddWithValue("$date", trip.TripDate);
        cmd.Parameters.AddWithValue("$time", trip.TripTime);
        cmd.Parameters.AddWithValue("$seats", trip.SeatsTotal);
        cmd.Parameters.AddWithValue("$dist", trip.DistanceKm);
        cmd.Parameters.AddWithValue("$price", trip.PricePerSeat);
        cmd.ExecuteNonQuery();
    }

    public static void SaveExperiment(string routeName, double distanceKm, double fuelCons, double fuelPrice, double deprec, ExperimentResult exp)
    {
        using var conn = new SqliteConnection(ConnectionString);
        conn.Open();

        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO CostExperiments (RouteName, DistanceKm, FuelConsumption, FuelPrice, DepreciationPerKm, PassengersCount, TotalCost, CostPerPerson, DriverSavings, CreatedAt)
            VALUES ($route, $dist, $cons, $price, $deprec, $pass, $total, $per, $sav, $dt);
            """;
        cmd.Parameters.AddWithValue("$route", routeName);
        cmd.Parameters.AddWithValue("$dist", distanceKm);
        cmd.Parameters.AddWithValue("$cons", fuelCons);
        cmd.Parameters.AddWithValue("$price", fuelPrice);
        cmd.Parameters.AddWithValue("$deprec", deprec);
        cmd.Parameters.AddWithValue("$pass", exp.PassengersCount);
        cmd.Parameters.AddWithValue("$total", exp.TotalCost);
        cmd.Parameters.AddWithValue("$per", exp.CostPerPerson);
        cmd.Parameters.AddWithValue("$sav", exp.DriverSavings);
        cmd.Parameters.AddWithValue("$dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }
}
