# -*- coding: utf-8 -*-
"""
Модуль работы с базой данных SQLite.
Версия 2.0 (Расширенная реляционная схема, логирование событий и экспорт данных).
"""
import sqlite3
import os
import csv
from datetime import datetime

DB_NAME = "experiments_v2.db"

def get_connection():
    """Создает и возвращает подключение к базе данных SQLite."""
    db_path = os.path.join(os.path.dirname(os.path.abspath(__file__)), DB_NAME)
    conn = sqlite3.connect(db_path)
    conn.row_factory = sqlite3.Row
    conn.execute("PRAGMA foreign_keys = ON")
    return conn

def init_db():
    """Инициализирует расширенную структуру базы данных SQLite (v2.0)."""
    with get_connection() as conn:
        cursor = conn.cursor()
        
        # Основная таблица серий экспериментов
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
                std_dev REAL NOT NULL,
                ci_lower REAL NOT NULL,
                ci_upper REAL NOT NULL,
                relative_error REAL NOT NULL,
                calc_time_ms REAL NOT NULL,
                status TEXT NOT NULL
            )
        """)
        
        # Индекс для быстрого поиска по наименованию модуля
        cursor.execute("""
            CREATE INDEX IF NOT EXISTS idx_module_name ON experiments(module_name)
        """)
        
        # Дочерняя таблица подробного журнала событий (логирования)
        cursor.execute("""
            CREATE TABLE IF NOT EXISTS experiment_logs (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                experiment_id INTEGER NOT NULL,
                log_time TEXT NOT NULL,
                event_type TEXT NOT NULL,
                message TEXT NOT NULL,
                FOREIGN KEY (experiment_id) REFERENCES experiments(id) ON DELETE CASCADE
            )
        """)
        
        conn.commit()

def save_experiment(res: dict):
    """
    Сохраняет результаты эксперимента и журнал событий в базу данных.
    """
    created_at = datetime.now().strftime("%Y-%m-%d %H:%M:%S")
    with get_connection() as conn:
        cursor = conn.cursor()
        cursor.execute("""
            INSERT INTO experiments (
                created_at, module_name, iterations, lambda_rate, time_limit,
                theoretical_p, empirical_p, mttf, std_dev, ci_lower, ci_upper,
                relative_error, calc_time_ms, status
            ) VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)
        """, (
            created_at, res["module_name"], res["iterations"], res["lambda_rate"], res["time_limit"],
            res["theoretical_p"], res["empirical_p"], res["mttf"], res["std_dev"],
            res["ci_lower"], res["ci_upper"], res["relative_error"], res["calc_time_ms"], res["status"]
        ))
        exp_id = cursor.lastrowid
        
        # Добавление записей в журнал аудита (experiment_logs)
        cursor.execute("""
            INSERT INTO experiment_logs (experiment_id, log_time, event_type, message)
            VALUES (?, ?, ?, ?)
        """, (exp_id, created_at, "INIT", f"Запуск расчета для модуля '{res['module_name']}', N={res['iterations']}"))
        
        cursor.execute("""
            INSERT INTO experiment_logs (experiment_id, log_time, event_type, message)
            VALUES (?, ?, ?, ?)
        """, (exp_id, created_at, "RESULT", f"P*={res['empirical_p']}, MTTF={res['mttf']} ч., Статус: {res['status']}"))
        
        conn.commit()
        return exp_id

def get_all_experiments():
    """Возвращает список всех проведенных экспериментов."""
    with get_connection() as conn:
        cursor = conn.cursor()
        cursor.execute("SELECT * FROM experiments ORDER BY id DESC")
        return cursor.fetchall()

def delete_experiment(exp_id: int):
    """Удаляет запись об эксперименте по его ID (включая связанные логи)."""
    with get_connection() as conn:
        cursor = conn.cursor()
        cursor.execute("DELETE FROM experiments WHERE id = ?", (exp_id,))
        conn.commit()

def export_to_csv(filepath: str):
    """Экспортирует историю экспериментов из SQLite в CSV-файл."""
    experiments = get_all_experiments()
    if not experiments:
        return 0
    with open(filepath, "w", newline="", encoding="utf-8-sig") as f:
        writer = csv.writer(f, delimiter=";")
        writer.writerow([
            "ID", "Дата и время", "Модуль", "Итераций (N)", "Интенсивность lambda",
            "Время T", "Теор. P(T)", "Эмп. P*(T)", "MTTF (ч)", "Погрешность (%)",
            "Время расчета (мс)", "Статус"
        ])
        for row in experiments:
            writer.writerow([
                row["id"], row["created_at"], row["module_name"], row["iterations"],
                row["lambda_rate"], row["time_limit"], row["theoretical_p"],
                row["empirical_p"], row["mttf"], row["relative_error"],
                row["calc_time_ms"], row["status"]
            ])
    return len(experiments)
