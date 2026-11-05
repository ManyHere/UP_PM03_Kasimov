# -*- coding: utf-8 -*-
"""
Главная точка входа программного комплекса «Сервис поиска попутчиков» (Версия 2.0).
По умолчанию запускает современный графический интерфейс пользователя (GUI).
При передаче аргумента командной строки '--cli' запускает консольный режим.

Специальность: 09.02.07 Информационные системы и программирование
Студент: Касимов Д.А. (Группа ИСП 31-9)
"""

import sys
import os

from database import init_database, get_all_trips, add_trip, book_seat, save_experiment
from experiment import calculate_total_trip_cost, calculate_cost_sharing, run_experiment_grid

def print_cli_header():
    print("=" * 70)
    print("       СЕРВИС ПОИСКА ПОПУТЧИКОВ (Режим CLI / Консоль)")
    print("       Разработчик: Касимов Д.А., гр. ИСП 31-9")
    print("=" * 70)

def show_trips_cli():
    print("\n--- СПИСОК ДОСТУПНЫХ ПОЕЗДОК (АКТИВНЫЕ МАРШРУТЫ) ---")
    trips = get_all_trips()
    if not trips:
        print("На данный момент нет доступных поездок со свободными местами.")
        return
    
    print(f"{'ID':<4} | {'Маршрут':<28} | {'Дата/Время':<16} | {'Мест':<5} | {'Цена (руб)':<10} | {'Водитель'}")
    print("-" * 85)
    for t in trips:
        t_id, drv_name, drv_phone, car, from_c, to_c, t_date, t_time, seats, price, dist = t
        route = f"{from_c} -> {to_c}"
        dt = f"{t_date} {t_time}"
        print(f"{t_id:<4} | {route:<28} | {dt:<16} | {seats:<5} | {price:<10.1f} | {drv_name} ({car})")

def handle_booking_cli():
    show_trips_cli()
    print("\n--- ОФОРМЛЕНИЕ БРОНИРОВАНИЯ ---")
    try:
        trip_id = int(input("Введите ID выбранной поездки: ").strip())
        name = input("Введите ваше имя: ").strip()
        phone = input("Введите контактный телефон: ").strip()
        seats = int(input("Количество мест (1-4): ").strip())
        
        booking_id, total_cost = book_seat(trip_id, name, phone, seats)
        print("\n[УСПЕХ] Бронирование успешно оформлено!")
        print(f"Номер заявки: #{booking_id}")
        print(f"Пассажир: {name} ({phone})")
        print(f"Забронировано мест: {seats}")
        print(f"К оплате водителю: {total_cost:.2f} руб.")
    except Exception as e:
        print(f"[ОШИБКА] Не удалось оформить бронирование: {e}")

def handle_add_trip_cli():
    print("\n--- ПУБЛИКАЦИЯ НОВОЙ ПОЕЗДКИ ---")
    try:
        driver = input("ФИО водителя: ").strip()
        phone = input("Телефон: ").strip()
        car = input("Марка и модель авто: ").strip()
        from_c = input("Город отправления: ").strip()
        to_c = input("Город назначения: ").strip()
        t_date = input("Дата поездки (ГГГГ-ММ-ДД): ").strip()
        t_time = input("Время отправления (ЧЧ:ММ): ").strip()
        seats = int(input("Всего мест: ").strip())
        dist = float(input("Расстояние между городами (км): ").strip())
        price = float(input("Стоимость за 1 место (руб): ").strip())
        
        trip_id = add_trip(driver, phone, car, from_c, to_c, t_date, t_time, seats, dist, price)
        print(f"\n[УСПЕХ] Поездка #{trip_id} успешно опубликована!")
    except Exception as e:
        print(f"[ОШИБКА] Не удалось добавить поездку: {e}")

def handle_experiment_cli():
    print("\n--- ВЫЧИСЛИТЕЛЬНЫЙ ЭКСПЕРИМЕНТ: РАСПРЕДЕЛЕНИЕ РАСХОДОВ И ЭКОНОМИЯ ---")
    try:
        route_name = input("Наименование маршрута (например, Ижевск - Казань): ").strip()
        dist = float(input("Дистанция (км): ").strip())
        consumption = float(input("Средний расход топлива (л/100 км, по умолч. 8.5): ") or 8.5)
        fuel_price = float(input("Цена бензина (руб/л, по умолч. 58.5): ") or 58.5)
        deprec = float(input("Амортизация (руб/км, по умолч. 1.8): ") or 1.8)
        
        total_cost = calculate_total_trip_cost(dist, consumption, fuel_price, deprec)
        grid = run_experiment_grid(dist, consumption, fuel_price, deprec)
        
        print("\n" + "=" * 78)
        print(f"РЕЗУЛЬТАТЫ ЭКСПЕРИМЕНТА: {route_name.upper()} ({dist} км)")
        print(f"Полная себестоимость поездки: {total_cost:.2f} руб.")
        print("=" * 78)
        print(f"{'Попутчиков':<12} | {'Всего в авто':<14} | {'Доля на чел. (руб)':<20} | {'Экономия водителя':<18} | {'Покрытие'}")
        print("-" * 78)
        for row in grid:
            print(f"{row['passengers_count']:<12} | {row['total_persons']:<14} | {row['cost_per_person']:<20.2f} | {row['driver_savings']:<18.2f} | {row['coverage_percent']}%")
        print("=" * 78)
        
        for row in grid:
            save_experiment(route_name, dist, consumption, fuel_price, deprec, row['passengers_count'], row['total_cost'], row['cost_per_person'], row['driver_savings'], row['annual_savings'])
        print("[ИНФОРМАЦИЯ] Данные вычислительного эксперимента успешно сохранены в БД SQLite.")
    except Exception as e:
        print(f"[ОШИБКА] Ошибка эксперимента: {e}")

def run_cli():
    init_database()
    while True:
        print_cli_header()
        print("1. Показать список доступных поездок")
        print("2. Забронировать место в поездке")
        print("3. Опубликовать новую поездку")
        print("4. Провести вычислительный эксперимент")
        print("5. Выход из программы")
        choice = input("\nВыберите действие (1-5): ").strip()
        
        if choice == "1":
            show_trips_cli()
        elif choice == "2":
            handle_booking_cli()
        elif choice == "3":
            handle_add_trip_cli()
        elif choice == "4":
            handle_experiment_cli()
        elif choice == "5":
            print("\nЗавершение работы программы.")
            break
        else:
            print("\nНекорректный ввод.")
        input("\nНажмите Enter для продолжения...")

def main():
    if "--cli" in sys.argv:
        run_cli()
    else:
        try:
            from gui import start_gui
            start_gui()
        except Exception as e:
            print(f"[ПРЕДУПРЕЖДЕНИЕ] Не удалось запустить графический интерфейс ({e}). Переход в консольный режим.")
            run_cli()

if __name__ == "__main__":
    main()
