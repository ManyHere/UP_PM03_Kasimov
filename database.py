# -*- coding: utf-8 -*-
"""
Модуль работы с базой данных SQLite для сервиса поиска попутчиков (Версия 2.0).
Реализует реляционную структуру данных, индексы, транзакционность,
аудит-логирование, отмену бронирований и экспорт данных в CSV.
"""

import sqlite3
import csv
import os
from datetime import datetime

DB_NAME = "carpooling_v2.db"

def get_connection(db_name=DB_NAME):
    """Возвращает соединение с базой данных с поддержкой внешних ключей."""
    conn = sqlite3.connect(db_name)
    conn.execute("PRAGMA foreign_keys = ON;")
    return conn

def init_database(db_name=DB_NAME):
    """Создает таблицы базы данных и индексы производительности."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        
        # Таблица поездок (маршрутов)
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS trips (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                driver_name TEXT NOT NULL,
                driver_phone TEXT NOT NULL,
                car_model TEXT NOT NULL,
                from_city TEXT NOT NULL,
                to_city TEXT NOT NULL,
                trip_date TEXT NOT NULL,
                trip_time TEXT NOT NULL,
                seats_total INTEGER NOT NULL CHECK (seats_total > 0),
                seats_available INTEGER NOT NULL CHECK (seats_available >= 0),
                distance_km REAL NOT NULL CHECK (distance_km > 0),
                price_per_seat REAL NOT NULL CHECK (price_per_seat >= 0)
            );
        """)
        
        # Индексы для оптимизации поиска
        cursor.execute("CREATE INDEX IF NOT EXISTS idx_trips_cities ON trips(from_city, to_city);")
        cursor.execute("CREATE INDEX IF NOT EXISTS idx_trips_date ON trips(trip_date);")
        
        # Таблица бронирований
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS bookings (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                trip_id INTEGER NOT NULL,
                passenger_name TEXT NOT NULL,
                passenger_phone TEXT NOT NULL,
                seats_booked INTEGER NOT NULL CHECK (seats_booked > 0),
                total_cost REAL NOT NULL CHECK (total_cost >= 0),
                booking_date TEXT NOT NULL,
                status TEXT NOT NULL DEFAULT 'Подтверждено',
                FOREIGN KEY (trip_id) REFERENCES trips (id) ON DELETE CASCADE
            );
        """)
        cursor.execute("CREATE INDEX IF NOT EXISTS idx_bookings_trip ON bookings(trip_id);")
        
        # Таблица результатов вычислительных экспериментов
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS cost_experiments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                route_name TEXT NOT NULL,
                distance_km REAL NOT NULL,
                fuel_consumption REAL NOT NULL,
                fuel_price REAL NOT NULL,
                depreciation_per_km REAL NOT NULL,
                passengers_count INTEGER NOT NULL,
                total_cost REAL NOT NULL,
                cost_per_person REAL NOT NULL,
                driver_savings REAL NOT NULL,
                annual_savings REAL NOT NULL,
                created_at TEXT NOT NULL
            );
        """)
        
        # Таблица аудита событий системы
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS audit_logs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                event_type TEXT NOT NULL,
                description TEXT NOT NULL,
                created_at TEXT NOT NULL
            );
        """)
        
        # Начальное заполнение реалистичными маршрутами Удмуртской Республики и соседних регионов
        cursor.execute("SELECT COUNT(*) FROM trips;")
        if cursor.fetchone()[0] == 0:
            initial_trips = [
                ("Касимов Дмитрий", "+7 (912) 850-12-34", "Lada Vesta", "Ижевск", "Казань", "2026-11-10", "08:00", 3, 3, 390.0, 950.0),
                ("Иванов Алексей", "+7 (909) 111-22-33", "Skoda Octavia", "Ижевск", "Пермь", "2026-11-11", "09:30", 4, 4, 280.0, 750.0),
                ("Петров Сергей", "+7 (950) 444-55-66", "Kia Rio", "Ижевск", "Воткинск", "2026-11-10", "14:00", 3, 3, 58.0, 200.0),
                ("Смирнова Анна", "+7 (919) 777-88-99", "Hyundai Solaris", "Ижевск", "Набережные Челны", "2026-11-12", "11:00", 3, 3, 200.0, 550.0),
                ("Морозов Илья", "+7 (904) 222-33-44", "Volkswagen Polo", "Ижевск", "Глазов", "2026-11-11", "16:00", 3, 3, 175.0, 480.0),
                ("Васильев Денис", "+7 (912) 333-77-88", "Renault Duster", "Ижевск", "Сарапул", "2026-11-10", "17:30", 4, 4, 65.0, 220.0)
            ]
            cursor.executemany("""
                INSERT INTO trips (driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_available, distance_km, price_per_seat)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            """, initial_trips)
            
            # Запись лога инициализации
            now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            cursor.execute("""
                INSERT INTO audit_logs (event_type, description, created_at)
                VALUES ('INIT_DATABASE', 'База данных carpooling_v2.db успешно инициализирована', ?);
            """, (now_str,))
            
            conn.commit()

def log_event(event_type: str, description: str, db_name=DB_NAME):
    """Записывает событие аудита в журнал."""
    try:
        with get_connection(db_name) as conn:
            cursor = conn.cursor()
            now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
            cursor.execute("""
                INSERT INTO audit_logs (event_type, description, created_at)
                VALUES (?, ?, ?);
            """, (event_type, description, now_str))
            conn.commit()
    except Exception:
        pass

def add_trip(driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, distance_km, price_per_seat, db_name=DB_NAME):
    """Публикация нового маршрута водителем."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO trips (driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_available, distance_km, price_per_seat)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
        """, (driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_total, distance_km, price_per_seat))
        new_id = cursor.lastrowid
        conn.commit()
        log_event("ADD_TRIP", f"Водитель {driver_name} создал поездку #{new_id} ({from_city} -> {to_city})", db_name)
        return new_id

def get_all_trips(from_city=None, to_city=None, db_name=DB_NAME):
    """Возвращает список всех активных поездок со свободными местами."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        query = "SELECT id, driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_available, price_per_seat, distance_km FROM trips WHERE seats_available > 0"
        params = []
        if from_city and from_city.strip():
            query += " AND LOWER(from_city) LIKE LOWER(?)"
            params.append(f"%{from_city.strip()}%")
        if to_city and to_city.strip():
            query += " AND LOWER(to_city) LIKE LOWER(?)"
            params.append(f"%{to_city.strip()}%")
        query += " ORDER BY trip_date ASC, trip_time ASC"
        cursor.execute(query, params)
        return cursor.fetchall()

def book_seat(trip_id, passenger_name, passenger_phone, seats_count, db_name=DB_NAME):
    """Оформление бронирования пассажиром с контролем остатка мест."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT seats_available, price_per_seat, from_city, to_city FROM trips WHERE id = ?", (trip_id,))
        row = cursor.fetchone()
        if not row:
            raise ValueError(f"Поездка с ID {trip_id} не найдена.")
        
        seats_available, price_per_seat, from_c, to_c = row
        if seats_available < seats_count:
            raise ValueError(f"Недостаточно мест! Доступно: {seats_available}, запрошено: {seats_count}.")
        
        total_cost = seats_count * price_per_seat
        now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        
        cursor.execute("UPDATE trips SET seats_available = seats_available - ? WHERE id = ?", (seats_count, trip_id))
        cursor.execute("""
            INSERT INTO bookings (trip_id, passenger_name, passenger_phone, seats_booked, total_cost, booking_date, status)
            VALUES (?, ?, ?, ?, ?, ?, 'Подтверждено');
        """, (trip_id, passenger_name, passenger_phone, seats_count, total_cost, now_str))
        booking_id = cursor.lastrowid
        conn.commit()
        
        log_event("BOOK_SEAT", f"Бронирование #{booking_id}: Пассажир {passenger_name} забронировал {seats_count} мест(а) на рейс #{trip_id} ({from_c}->{to_c})", db_name)
        return booking_id, total_cost

def cancel_booking(booking_id, db_name=DB_NAME):
    """Отменяет бронирование и возвращает места в поездку."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT trip_id, seats_booked, status FROM bookings WHERE id = ?", (booking_id,))
        row = cursor.fetchone()
        if not row:
            raise ValueError(f"Бронирование #{booking_id} не найдено.")
        
        trip_id, seats_booked, status = row
        if status == "Отменено":
            raise ValueError("Бронирование уже было отменено ранее.")
        
        # Обновляем статус бронирования
        cursor.execute("UPDATE bookings SET status = 'Отменено' WHERE id = ?", (booking_id,))
        # Возвращаем места
        cursor.execute("UPDATE trips SET seats_available = seats_available + ? WHERE id = ?", (seats_booked, trip_id))
        conn.commit()
        
        log_event("CANCEL_BOOKING", f"Бронирование #{booking_id} отменено, {seats_booked} мест возвращено в поездку #{trip_id}", db_name)
        return True

def get_all_bookings(db_name=DB_NAME):
    """Возвращает список всех бронирований с деталями маршрута."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("""
            SELECT b.id, b.trip_id, (t.from_city || ' -> ' || t.to_city) AS route,
                   b.passenger_name, b.passenger_phone, b.seats_booked, b.total_cost,
                   b.booking_date, b.status
            FROM bookings b
            JOIN trips t ON b.trip_id = t.id
            ORDER BY b.id DESC;
        """)
        return cursor.fetchall()

def save_experiment(route_name, distance_km, fuel_consumption, fuel_price, depreciation_per_km, passengers_count, total_cost, cost_per_person, driver_savings, annual_savings=0.0, db_name=DB_NAME):
    """Сохранение результатов эксперимента в БД SQLite."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        cursor.execute("""
            INSERT INTO cost_experiments (route_name, distance_km, fuel_consumption, fuel_price, depreciation_per_km, passengers_count, total_cost, cost_per_person, driver_savings, annual_savings, created_at)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
        """, (route_name, distance_km, fuel_consumption, fuel_price, depreciation_per_km, passengers_count, total_cost, cost_per_person, driver_savings, annual_savings, now_str))
        new_id = cursor.lastrowid
        conn.commit()
        log_event("SAVE_EXPERIMENT", f"Сохранен расчет #{new_id} для маршрута {route_name} ({passengers_count} попутч.)", db_name)
        return new_id

def export_trips_to_csv(filepath, db_name=DB_NAME):
    """Экспорт всех поездок в файл формата CSV."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT id, driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_available, distance_km, price_per_seat FROM trips")
        rows = cursor.fetchall()
        
        with open(filepath, mode="w", newline="", encoding="utf-8-sig") as csv_file:
            writer = csv.writer(csv_file, delimiter=";")
            writer.writerow(["ID", "Водитель", "Телефон", "Автомобиль", "Откуда", "Куда", "Дата", "Время", "Всего мест", "Свободно мест", "Расстояние (км)", "Цена за место (руб)"])
            writer.writerows(rows)
        return len(rows)

def export_bookings_to_csv(filepath, db_name=DB_NAME):
    """Экспорт всех бронирований в файл формата CSV."""
    bookings = get_all_bookings(db_name)
    with open(filepath, mode="w", newline="", encoding="utf-8-sig") as csv_file:
        writer = csv.writer(csv_file, delimiter=";")
        writer.writerow(["ID брони", "ID поездки", "Маршрут", "Пассажир", "Телефон", "Кол-во мест", "Сумма (руб)", "Дата бронирования", "Статус"])
        writer.writerows(bookings)
    return len(bookings)
