# -*- coding: utf-8 -*-
"""
Модуль работы с базой данных SQLite для сервиса поиска попутчиков (Версия 1.0 - Консольная).
Содержит функции инициализации таблиц, добавления поездок, поиска и бронирования мест.
"""

import sqlite3
import os
from datetime import datetime

DB_NAME = "carpooling_v1.db"

def get_connection(db_name=DB_NAME):
    """Создает и возвращает подключение к базе данных SQLite."""
    conn = sqlite3.connect(db_name)
    conn.execute("PRAGMA foreign_keys = ON;")
    return conn

def init_database(db_name=DB_NAME):
    """Инициализирует структуру базы данных и заполняет начальными данными."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        
        # Таблица поездок
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
                FOREIGN KEY (trip_id) REFERENCES trips (id) ON DELETE CASCADE
            );
        """)
        
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
                created_at TEXT NOT NULL
            );
        """)
        
        # Проверка наличия данных и первичное наполнение
        cursor.execute("SELECT COUNT(*) FROM trips;")
        if cursor.fetchone()[0] == 0:
            initial_trips = [
                ("Касимов Дмитрий", "+7 (912) 850-12-34", "Lada Vesta", "Ижевск", "Казань", "2026-11-10", "08:00", 3, 3, 390.0, 950.0),
                ("Иванов Алексей", "+7 (909) 111-22-33", "Skoda Octavia", "Ижевск", "Пермь", "2026-11-11", "09:30", 4, 4, 280.0, 750.0),
                ("Петров Сергей", "+7 (950) 444-55-66", "Kia Rio", "Ижевск", "Воткинск", "2026-11-10", "14:00", 3, 3, 58.0, 200.0),
                ("Смирнова Анна", "+7 (919) 777-88-99", "Hyundai Solaris", "Ижевск", "Набережные Челны", "2026-11-12", "11:00", 3, 3, 200.0, 550.0)
            ]
            cursor.executemany("""
                INSERT INTO trips (driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_available, distance_km, price_per_seat)
                VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
            """, initial_trips)
            conn.commit()

def add_trip(driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, distance_km, price_per_seat, db_name=DB_NAME):
    """Добавляет новую поездку водителя."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO trips (driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_available, distance_km, price_per_seat)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
        """, (driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_total, seats_total, distance_km, price_per_seat))
        conn.commit()
        return cursor.lastrowid

def get_all_trips(from_city=None, to_city=None, db_name=DB_NAME):
    """Возвращает список всех активных поездок со свободными местами."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        query = "SELECT id, driver_name, driver_phone, car_model, from_city, to_city, trip_date, trip_time, seats_available, price_per_seat, distance_km FROM trips WHERE seats_available > 0"
        params = []
        if from_city:
            query += " AND LOWER(from_city) LIKE LOWER(?)"
            params.append(f"%{from_city}%")
        if to_city:
            query += " AND LOWER(to_city) LIKE LOWER(?)"
            params.append(f"%{to_city}%")
        query += " ORDER BY trip_date ASC, trip_time ASC"
        cursor.execute(query, params)
        return cursor.fetchall()

def book_seat(trip_id, passenger_name, passenger_phone, seats_count, db_name=DB_NAME):
    """Бронирует места в поездке с транзакционным контролем."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT seats_available, price_per_seat FROM trips WHERE id = ?", (trip_id,))
        row = cursor.fetchone()
        if not row:
            raise ValueError(f"Поездка с ID {trip_id} не найдена.")
        
        seats_available, price_per_seat = row
        if seats_available < seats_count:
            raise ValueError(f"Недостаточно свободных мест. Доступно: {seats_available}, запрошено: {seats_count}.")
        
        total_cost = seats_count * price_per_seat
        now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        
        # Уменьшаем число доступных мест
        cursor.execute("UPDATE trips SET seats_available = seats_available - ? WHERE id = ?", (seats_count, trip_id))
        
        # Добавляем запись о бронировании
        cursor.execute("""
            INSERT INTO bookings (trip_id, passenger_name, passenger_phone, seats_booked, total_cost, booking_date)
            VALUES (?, ?, ?, ?, ?, ?);
        """, (trip_id, passenger_name, passenger_phone, seats_count, total_cost, now_str))
        
        conn.commit()
        return cursor.lastrowid, total_cost

def save_experiment(route_name, distance_km, fuel_consumption, fuel_price, depreciation_per_km, passengers_count, total_cost, cost_per_person, driver_savings, db_name=DB_NAME):
    """Сохраняет результаты вычислительного эксперимента."""
    with get_connection(db_name) as conn:
        cursor = conn.cursor()
        now_str = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
        cursor.execute("""
            INSERT INTO cost_experiments (route_name, distance_km, fuel_consumption, fuel_price, depreciation_per_km, passengers_count, total_cost, cost_per_person, driver_savings, created_at)
            VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?);
        """, (route_name, distance_km, fuel_consumption, fuel_price, depreciation_per_km, passengers_count, total_cost, cost_per_person, driver_savings, now_str))
        conn.commit()
        return cursor.lastrowid
