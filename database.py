# -*- coding: utf-8 -*-
"""
Модуль работы с базой данных SQLite.
Версия 1.0 (Базовая версия).
"""
import sqlite3
import os
from datetime import datetime

DB_NAME = "experiments_v1.db"

def get_connection():
    """Создает и возвращает подключение к базе данных SQLite."""
    db_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), DB_NAME)
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    return conn

def init_db():
    """Инициализирует структуру базы данных SQLite."""
    with get_connection() as conn:
        cursor = conn.cursor()
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS experiments (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                created_at TEXT NOT NULL,
                module_name TEXT NOT NULL,
                iterations INTEGER NOT NULL,
                lambda_rate REAL NOT NULL,
                time_limit REAL NOT NULL,
                theoretical_p REAL NOT NULL,
                empirical_p REAL NOT NULL,
                mttf REAL NOT NULL,
                relative_error REAL NOT NULL,
                calc_time_ms REAL NOT NULL
            )
        """)
        conn.commit()

def save_experiment(module_name, iterations, lambda_rate, time_limit,
                    theoretical_p, empirical_p, mttf, relative_error, calc_time_ms):
    """Сохраняет результаты вычислительного эксперимента в базу данных."""
    created_at = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    with get_connection() as conn:
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO experiments (
                created_at, module_name, iterations, lambda_rate, time_limit,
                theoretical_p, empirical_p, mttf, relative_error, calc_time_ms
            ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (created_at, module_name, iterations, lambda_rate, time_limit,
              theoretical_p, empirical_p, mttf, relative_error, calc_time_ms))
        conn.commit()
        return cursor.lastrowid

def get_all_experiments():
    """Возвращает список всех проведенных экспериментов из базы данных."""
    with get_connection() as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM experiments ORDER BY id DESC")
        return cursor.fetchall()
