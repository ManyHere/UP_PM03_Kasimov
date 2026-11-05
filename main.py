# -*- coding: utf-8 -*-
"""
Главная точка входа сервиса поиска попутчиков (Версия 1.0 - Консольный режим).
Предоставляет текстовый интерфейс для просмотра поездок, бронирования мест,
добавления маршрутов и проведения вычислительного эксперимента.
"""

import sys
import os
from database import init_database, get_all_trips, add_trip, book_seat, save_experiment
from experiment import calculate_total_trip_cost, calculate_cost_sharing, run_experiment_grid

def print_header():
    print("=" * 70)
    print("       СЕРВИС ПОИСКА ПОПУТЧИКОВ (Версия 1.0 - Консольный режим)")
    print("=" * 70)

def show_trips():
    print("\n--- СПИСОК ДОСТУПНЫХ ПОЕЗДОК СО СВОБОДНЫМИ МЕСТАМИ ---")
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

def handle_booking():
    show_trips()
    print("\n--- БРОНИРОВАНИЕ МЕСТА В ПОЕЗДКЕ ---")
    try:
        trip_id = int(input("Введите ID выбранной поездки: ").strip())
        name = input("Введите ваше имя: ").strip()
        phone = input("Введите ваш телефон: ").strip()
        seats = int(input("Количество мест для бронирования (1-4): ").strip())
        
        booking_id, total_cost = book_seat(trip_id, name, phone, seats)
        print("\n[УСПЕХ] Бронирование успешно подтверждено!")
        print(f"Номер брони: #{booking_id}")
        print(f"Пассажир: {name} ({phone})")
        print(f"Забронировано мест: {seats}")
        print(f"К оплате водителю: {total_cost:.2f} руб.")
    except Exception as e:
        print(f"[ОШИБКА] Не удалось оформить бронирование: {e}")

def handle_add_trip():
    print("\n--- ДОБАВЛЕНИЕ НОВОЙ ПОЕЗДКИ ВОДИТЕЛЯ ---")
    try:
        driver = input("ФИО водителя: ").strip()
        phone = input("Телефон: ").strip()
        car = input("Марка и модель авто: ").strip()
        from_c = input("Город отправления: ").strip()
        to_c = input("Город назначения: ").strip()
        t_date = input("Дата поездки (ГГГГ-ММ-ДД): ").strip()
        t_time = input("Время отправления (ЧЧ:ММ): ").strip()
        seats = int(input("Всего свободных мест: ").strip())
        dist = float(input("Расстояние между городами (км): ").strip())
        price = float(input("Стоимость за 1 место (руб): ").strip())
        
        trip_id = add_trip(driver, phone, car, from_c, to_c, t_date, t_time, seats, dist, price)
        print(f"\n[УСПЕХ] Поездка #{trip_id} успешно опубликована и доступна для поиска!")
    except Exception as e:
        print(f"[ОШИБКА] Не удалось добавить поездку: {e}")

def handle_experiment():
    print("\n--- ВЫЧИСЛИТЕЛЬНЫЙ ЭКСПЕРИМЕНТ: СЕБЕСТОИМОСТЬ И ЭКОНОМИЯ ПОЕЗДКИ ---")
    try:
        route_name = input("Наименование маршрута (например, Ижевск - Казань): ").strip()
        dist = float(input("Расстояние маршрута (км, например 390): ").strip())
        consumption = float(input("Средний расход топлива (л/100 км, по умолчанию 8.5): ") or 8.5)
        fuel_price = float(input("Цена бензина за литр (руб/л, по умолчанию 58.5): ") or 58.5)
        deprec = float(input("Норма амортизации на 1 км (руб/км, по умолчанию 1.8): ") or 1.8)
        
        total_cost = calculate_total_trip_cost(dist, consumption, fuel_price, deprec)
        grid = run_experiment_grid(dist, consumption, fuel_price, deprec)
        
        print("\n" + "=" * 75)
        print(f"РЕЗУЛЬТАТЫ ЭКСПЕРИМЕНТА ДЛЯ МАРШРУТА: {route_name.upper()} ({dist} км)")
        print(f"Полная себестоимость поездки (топливо + амортизация): {total_cost:.2f} руб.")
        print("=" * 75)
        print(f"{'Попутчиков':<12} | {'Всего людей':<12} | {'Доля на чел. (руб)':<20} | {'Экономия водителя (руб)':<22} | {'Покрытие затрат'}")
        print("-" * 75)
        
        for row in grid:
            print(f"{row['passengers_count']:<12} | {row['total_persons']:<12} | {row['cost_per_person']:<20.2f} | {row['driver_savings']:<22.2f} | {row['coverage_percent']}%")
        print("=" * 75)
        
        # Сохранение в БД SQLite
        for row in grid:
            save_experiment(route_name, dist, consumption, fuel_price, deprec, row['passengers_count'], row['total_cost'], row['cost_per_person'], row['driver_savings'])
        print("[ИНФОРМАЦИЯ] Данные вычислительного эксперимента успешно сохранены в SQLite (cost_experiments).")
        
    except Exception as e:
        print(f"[ОШИБКА] Ошибка при проведении эксперимента: {e}")

def main():
    init_database()
    while True:
        print_header()
        print("1. Показать список доступных поездок")
        print("2. Забронировать место в поездке")
        print("3. Опубликовать новую поездку (для водителей)")
        print("4. Провести вычислительный эксперимент (расчет себестоимости и экономии)")
        print("5. Выход из программы")
        choice = input("\nВыберите действие (1-5): ").strip()
        
        if choice == "1":
            show_trips()
        elif choice == "2":
            handle_booking()
        elif choice == "3":
            handle_add_trip()
        elif choice == "4":
            handle_experiment()
        elif choice == "5":
            print("\nЗавершение работы программы. До свидания!")
            break
        else:
            print("\nНекорректный выбор. Пожалуйста, введите число от 1 до 5.")
        input("\nНажмите Enter для продолжения...")

if __name__ == "__main__":
    main()
