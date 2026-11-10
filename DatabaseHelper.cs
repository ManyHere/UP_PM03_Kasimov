using System.Security.Cryptography;
using System.Text;
using Microsoft.Data.Sqlite;

namespace CarpoolingApp;

/// <summary>
/// Модуль работы с базой данных SQLite (Версия 2.0).
/// Реализует авторизацию пользователей, внешние ключи, индексы, транзакционное
/// бронирование и отмену броней, журнал аудита и экспорт в формат CSV.
/// </summary>
public static class DatabaseHelper
{
    public const string DbFileName = "carpooling_v2.db";
    private static string ConnectionString => $"Data Source={DbFileName}";

    private static SqliteConnection OpenConnection()
    {
        var conn = new SqliteConnection(ConnectionString);
        conn.Open();
        using var pragma = conn.CreateCommand();
        pragma.CommandText = "PRAGMA foreign_keys = ON;";
        pragma.ExecuteNonQuery();
        return conn;
    }

    public static void InitializeDatabase()
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            CREATE TABLE IF NOT EXISTS Users (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                FullName TEXT NOT NULL,
                Phone TEXT NOT NULL,
                Login TEXT NOT NULL UNIQUE,
                PasswordHash TEXT NOT NULL,
                Role TEXT NOT NULL
            );

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
            CREATE INDEX IF NOT EXISTS idx_trips_cities ON Trips(FromCity, ToCity);
            CREATE INDEX IF NOT EXISTS idx_trips_date ON Trips(TripDate);

            CREATE TABLE IF NOT EXISTS Bookings (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                TripId INTEGER NOT NULL,
                PassengerName TEXT NOT NULL,
                PassengerPhone TEXT NOT NULL,
                SeatsBooked INTEGER NOT NULL CHECK (SeatsBooked > 0),
                TotalCost REAL NOT NULL CHECK (TotalCost >= 0),
                BookingDate TEXT NOT NULL,
                Status TEXT NOT NULL DEFAULT 'Подтверждено',
                FOREIGN KEY (TripId) REFERENCES Trips(Id) ON DELETE CASCADE
            );
            CREATE INDEX IF NOT EXISTS idx_bookings_trip ON Bookings(TripId);

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
                AnnualSavings REAL NOT NULL,
                CreatedAt TEXT NOT NULL
            );

            CREATE TABLE IF NOT EXISTS AuditLogs (
                Id INTEGER PRIMARY KEY AUTOINCREMENT,
                EventType TEXT NOT NULL,
                Description TEXT NOT NULL,
                CreatedAt TEXT NOT NULL
            );
            """;
        cmd.ExecuteNonQuery();

        cmd.CommandText = "SELECT COUNT(*) FROM Users;";
        if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
        {
            SeedUsers(conn);
        }

        cmd.CommandText = "SELECT COUNT(*) FROM Trips;";
        if (Convert.ToInt64(cmd.ExecuteScalar()) == 0)
        {
            SeedTripsAndBookings(conn);
        }
    }

    private static string HashPassword(string password)
    {
        byte[] bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
        return Convert.ToHexString(bytes);
    }

    private static void SeedUsers(SqliteConnection conn)
    {
        var users = new[]
        {
            ("Касимов Дмитрий Андреевич", "+7 (912) 850-12-34", "driver", "1234", "Водитель"),
            ("Петрова Елена Игоревна", "+7 (909) 555-33-22", "passenger", "1234", "Пассажир")
        };

        foreach (var u in users)
        {
            using var ins = conn.CreateCommand();
            ins.CommandText = """
                INSERT INTO Users (FullName, Phone, Login, PasswordHash, Role)
                VALUES ($name, $phone, $login, $hash, $role);
                """;
            ins.Parameters.AddWithValue("$name", u.Item1);
            ins.Parameters.AddWithValue("$phone", u.Item2);
            ins.Parameters.AddWithValue("$login", u.Item3);
            ins.Parameters.AddWithValue("$hash", HashPassword(u.Item4));
            ins.Parameters.AddWithValue("$role", u.Item5);
            ins.ExecuteNonQuery();
        }
    }

    private static void SeedTripsAndBookings(SqliteConnection conn)
    {
        var initial = new[]
        {
            ("Касимов Дмитрий", "+7 (912) 850-12-34", "Lada Vesta", "Ижевск", "Казань", "2026-11-12", "08:00", 3, 390.0, 950.0),
            ("Иванов Алексей", "+7 (909) 111-22-33", "Skoda Octavia", "Ижевск", "Пермь", "2026-11-13", "09:30", 4, 280.0, 750.0),
            ("Петров Сергей", "+7 (950) 444-55-66", "Kia Rio", "Ижевск", "Воткинск", "2026-11-12", "14:00", 3, 58.0, 200.0),
            ("Смирнова Анна", "+7 (919) 777-88-99", "Hyundai Solaris", "Ижевск", "Набережные Челны", "2026-11-14", "11:00", 3, 200.0, 550.0),
            ("Морозов Илья", "+7 (904) 222-33-44", "Volkswagen Polo", "Ижевск", "Глазов", "2026-11-13", "16:00", 3, 175.0, 480.0),
            ("Васильев Денис", "+7 (912) 333-77-88", "Renault Duster", "Ижевск", "Сарапул", "2026-11-12", "17:30", 4, 65.0, 220.0)
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

        LogEventInternal(conn, "INIT_DATABASE", "База данных carpooling_v2.db инициализирована (6 маршрутов).");
    }

    private static void LogEventInternal(SqliteConnection conn, string eventType, string description)
    {
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO AuditLogs (EventType, Description, CreatedAt)
            VALUES ($type, $desc, $dt);
            """;
        cmd.Parameters.AddWithValue("$type", eventType);
        cmd.Parameters.AddWithValue("$desc", description);
        cmd.Parameters.AddWithValue("$dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        cmd.ExecuteNonQuery();
    }

    public static UserAccount? AuthenticateUser(string login, string password)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT Id, FullName, Phone, Login, Role
            FROM Users
            WHERE LOWER(Login) = LOWER($login) AND PasswordHash = $hash;
            """;
        cmd.Parameters.AddWithValue("$login", login.Trim());
        cmd.Parameters.AddWithValue("$hash", HashPassword(password));

        using var reader = cmd.ExecuteReader();
        if (!reader.Read())
            return null;

        var user = new UserAccount
        {
            Id = reader.GetInt32(0),
            FullName = reader.GetString(1),
            Phone = reader.GetString(2),
            Login = reader.GetString(3),
            Role = reader.GetString(4)
        };
        reader.Close();
        LogEventInternal(conn, "USER_LOGIN", $"Вход в систему: {user.FullName} ({user.Role})");
        return user;
    }

    public static UserAccount RegisterUser(string fullName, string phone, string login, string password, string role)
    {
        if (string.IsNullOrWhiteSpace(fullName) || string.IsNullOrWhiteSpace(login) || string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("ФИО, логин и пароль обязательны для заполнения.");

        using var conn = OpenConnection();
        using var check = conn.CreateCommand();
        check.CommandText = "SELECT COUNT(*) FROM Users WHERE LOWER(Login) = LOWER($login);";
        check.Parameters.AddWithValue("$login", login.Trim());
        if (Convert.ToInt64(check.ExecuteScalar()) > 0)
            throw new InvalidOperationException($"Логин «{login.Trim()}» уже занят.");

        using var ins = conn.CreateCommand();
        ins.CommandText = """
            INSERT INTO Users (FullName, Phone, Login, PasswordHash, Role)
            VALUES ($name, $phone, $login, $hash, $role);
            SELECT last_insert_rowid();
            """;
        ins.Parameters.AddWithValue("$name", fullName.Trim());
        ins.Parameters.AddWithValue("$phone", phone.Trim());
        ins.Parameters.AddWithValue("$login", login.Trim());
        ins.Parameters.AddWithValue("$hash", HashPassword(password));
        ins.Parameters.AddWithValue("$role", role);
        int id = Convert.ToInt32(ins.ExecuteScalar());

        LogEventInternal(conn, "USER_REGISTER", $"Зарегистрирован пользователь {fullName.Trim()} ({role})");

        return new UserAccount
        {
            Id = id,
            FullName = fullName.Trim(),
            Phone = phone.Trim(),
            Login = login.Trim(),
            Role = role
        };
    }

    public static List<TripItem> GetTrips(string? fromCity = null, string? toCity = null)
    {
        var result = new List<TripItem>();
        using var conn = OpenConnection();
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

    public static int AddTrip(TripItem trip)
    {
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            INSERT INTO Trips (DriverName, DriverPhone, CarModel, FromCity, ToCity, TripDate, TripTime, SeatsTotal, SeatsAvailable, DistanceKm, PricePerSeat)
            VALUES ($driver, $phone, $car, $from, $to, $date, $time, $seats, $seats, $dist, $price);
            SELECT last_insert_rowid();
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
        int newId = Convert.ToInt32(cmd.ExecuteScalar());

        LogEventInternal(conn, "ADD_TRIP", $"Создан рейс #{newId}: {trip.FromCity} → {trip.ToCity} (водитель {trip.DriverName})");
        return newId;
    }

    public static (int BookingId, double TotalCost) BookSeat(int tripId, string passengerName, string passengerPhone, int seatsCount)
    {
        using var conn = OpenConnection();
        using var tx = conn.BeginTransaction();

        using var sel = conn.CreateCommand();
        sel.Transaction = tx;
        sel.CommandText = "SELECT SeatsAvailable, PricePerSeat, FromCity, ToCity FROM Trips WHERE Id = $id;";
        sel.Parameters.AddWithValue("$id", tripId);

        using var reader = sel.ExecuteReader();
        if (!reader.Read())
            throw new InvalidOperationException($"Поездка #{tripId} не найдена.");

        int seatsAvailable = reader.GetInt32(0);
        double pricePerSeat = reader.GetDouble(1);
        string fromCity = reader.GetString(2);
        string toCity = reader.GetString(3);
        reader.Close();

        if (seatsAvailable < seatsCount)
            throw new InvalidOperationException($"Недостаточно мест! Доступно: {seatsAvailable}, запрошено: {seatsCount}.");

        double totalCost = seatsCount * pricePerSeat;

        using var upd = conn.CreateCommand();
        upd.Transaction = tx;
        upd.CommandText = "UPDATE Trips SET SeatsAvailable = SeatsAvailable - $cnt WHERE Id = $id;";
        upd.Parameters.AddWithValue("$cnt", seatsCount);
        upd.Parameters.AddWithValue("$id", tripId);
        upd.ExecuteNonQuery();

        using var ins = conn.CreateCommand();
        ins.Transaction = tx;
        ins.CommandText = """
            INSERT INTO Bookings (TripId, PassengerName, PassengerPhone, SeatsBooked, TotalCost, BookingDate, Status)
            VALUES ($tid, $pname, $pphone, $seats, $cost, $dt, 'Подтверждено');
            SELECT last_insert_rowid();
            """;
        ins.Parameters.AddWithValue("$tid", tripId);
        ins.Parameters.AddWithValue("$pname", passengerName.Trim());
        ins.Parameters.AddWithValue("$pphone", passengerPhone.Trim());
        ins.Parameters.AddWithValue("$seats", seatsCount);
        ins.Parameters.AddWithValue("$cost", totalCost);
        ins.Parameters.AddWithValue("$dt", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
        int bookingId = Convert.ToInt32(ins.ExecuteScalar());

        tx.Commit();
        LogEventInternal(conn, "BOOK_SEAT", $"Бронь #{bookingId}: {passengerName.Trim()} ({seatsCount} мест) на рейс #{tripId} ({fromCity} → {toCity})");
        return (bookingId, totalCost);
    }

    public static void CancelBooking(int bookingId)
    {
        using var conn = OpenConnection();
        using var tx = conn.BeginTransaction();

        using var sel = conn.CreateCommand();
        sel.Transaction = tx;
        sel.CommandText = "SELECT TripId, SeatsBooked, Status FROM Bookings WHERE Id = $id;";
        sel.Parameters.AddWithValue("$id", bookingId);

        using var reader = sel.ExecuteReader();
        if (!reader.Read())
            throw new InvalidOperationException($"Бронирование #{bookingId} не найдено.");

        int tripId = reader.GetInt32(0);
        int seatsBooked = reader.GetInt32(1);
        string status = reader.GetString(2);
        reader.Close();

        if (status == "Отменено")
            throw new InvalidOperationException("Данное бронирование уже отменено ранее.");

        using var updBook = conn.CreateCommand();
        updBook.Transaction = tx;
        updBook.CommandText = "UPDATE Bookings SET Status = 'Отменено' WHERE Id = $id;";
        updBook.Parameters.AddWithValue("$id", bookingId);
        updBook.ExecuteNonQuery();

        using var updTrip = conn.CreateCommand();
        updTrip.Transaction = tx;
        updTrip.CommandText = "UPDATE Trips SET SeatsAvailable = SeatsAvailable + $cnt WHERE Id = $tid;";
        updTrip.Parameters.AddWithValue("$cnt", seatsBooked);
        updTrip.Parameters.AddWithValue("$tid", tripId);
        updTrip.ExecuteNonQuery();

        tx.Commit();
        LogEventInternal(conn, "CANCEL_BOOKING", $"Бронирование #{bookingId} отменено, {seatsBooked} мест возвращено в рейс #{tripId}");
    }

    public static List<BookingItem> GetBookings()
    {
        var list = new List<BookingItem>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = """
            SELECT b.Id, b.TripId, (t.FromCity || ' → ' || t.ToCity), b.PassengerName, b.PassengerPhone, b.SeatsBooked, b.TotalCost, b.BookingDate, b.Status
            FROM Bookings b
            JOIN Trips t ON b.TripId = t.Id
            ORDER BY b.Id DESC;
            """;

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new BookingItem
            {
                Id = reader.GetInt32(0),
                TripId = reader.GetInt32(1),
                Route = reader.GetString(2),
                PassengerName = reader.GetString(3),
                PassengerPhone = reader.GetString(4),
                SeatsBooked = reader.GetInt32(5),
                TotalCost = reader.GetDouble(6),
                BookingDate = reader.GetString(7),
                Status = reader.GetString(8)
            });
        }
        return list;
    }

    public static List<AuditLogItem> GetAuditLogs()
    {
        var list = new List<AuditLogItem>();
        using var conn = OpenConnection();
        using var cmd = conn.CreateCommand();
        cmd.CommandText = "SELECT Id, EventType, Description, CreatedAt FROM AuditLogs ORDER BY Id DESC LIMIT 50;";

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            list.Add(new AuditLogItem
            {
                Id = reader.GetInt32(0),
                EventType = reader.GetString(1),
                Description = reader.GetString(2),
                CreatedAt = reader.GetString(3)
            });
        }
        return list;
    }

    public static void SaveExperimentGrid(string routeName, double distanceKm, double fuelCons, double fuelPrice, double deprec, IEnumerable<ExperimentResult> grid)
    {
        using var conn = OpenConnection();
        using var tx = conn.BeginTransaction();
        string now = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        foreach (var item in grid)
        {
            using var cmd = conn.CreateCommand();
            cmd.Transaction = tx;
            cmd.CommandText = """
                INSERT INTO CostExperiments (RouteName, DistanceKm, FuelConsumption, FuelPrice, DepreciationPerKm, PassengersCount, TotalCost, CostPerPerson, DriverSavings, AnnualSavings, CreatedAt)
                VALUES ($route, $dist, $cons, $price, $deprec, $pass, $total, $per, $sav, $ann, $dt);
                """;
            cmd.Parameters.AddWithValue("$route", routeName);
            cmd.Parameters.AddWithValue("$dist", distanceKm);
            cmd.Parameters.AddWithValue("$cons", fuelCons);
            cmd.Parameters.AddWithValue("$price", fuelPrice);
            cmd.Parameters.AddWithValue("$deprec", deprec);
            cmd.Parameters.AddWithValue("$pass", item.PassengersCount);
            cmd.Parameters.AddWithValue("$total", item.TotalCost);
            cmd.Parameters.AddWithValue("$per", item.CostPerPerson);
            cmd.Parameters.AddWithValue("$sav", item.DriverSavings);
            cmd.Parameters.AddWithValue("$ann", item.AnnualSavings);
            cmd.Parameters.AddWithValue("$dt", now);
            cmd.ExecuteNonQuery();
        }

        tx.Commit();
        LogEventInternal(conn, "SAVE_EXPERIMENT", $"Сохранена матрица эксперимента для маршрута «{routeName}» ({distanceKm:F0} км)");
    }

    public static int ExportTripsToCsv(string filePath)
    {
        var trips = GetTrips();
        var sb = new StringBuilder();
        sb.AppendLine("ID;Водитель;Телефон;Автомобиль;Откуда;Куда;Дата;Время;Всего мест;Свободно мест;Дистанция (км);Цена за место (руб)");
        foreach (var t in trips)
        {
            sb.AppendLine($"{t.Id};{EscapeCsv(t.DriverName)};{EscapeCsv(t.DriverPhone)};{EscapeCsv(t.CarModel)};{EscapeCsv(t.FromCity)};{EscapeCsv(t.ToCity)};{t.TripDate};{t.TripTime};{t.SeatsTotal};{t.SeatsAvailable};{t.DistanceKm};{t.PricePerSeat}");
        }
        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
        return trips.Count;
    }

    public static int ExportBookingsToCsv(string filePath)
    {
        var bookings = GetBookings();
        var sb = new StringBuilder();
        sb.AppendLine("ID брони;№ рейса;Маршрут;Пассажир;Телефон;Мест;Сумма (руб);Дата бронирования;Статус");
        foreach (var b in bookings)
        {
            sb.AppendLine($"{b.Id};{b.TripId};{EscapeCsv(b.Route)};{EscapeCsv(b.PassengerName)};{EscapeCsv(b.PassengerPhone)};{b.SeatsBooked};{b.TotalCost};{b.BookingDate};{EscapeCsv(b.Status)}");
        }
        File.WriteAllText(filePath, sb.ToString(), new UTF8Encoding(true));
        return bookings.Count;
    }

    private static string EscapeCsv(string val) =>
        val.Contains(';') || val.Contains('"') ? $"\"{val.Replace("\"", "\"\"")}\"" : val;
}
