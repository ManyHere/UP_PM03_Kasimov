# -*- coding: utf-8 -*-
"""
Главный модуль приложения (Версия 1.0).
Консольный интерфейс для проведения вычислительного эксперимента и взаимодействия с БД SQLite.
Студент: Касимов Д.А., гр. ИСП 31-9
ПМ.03 Ревьюирование программных продуктов
"""
import sys
from database import init_db, save_experiment, get_all_experiments
from experiment import run_simulation

def print_header():
    print("=" * 65)
    print("   ПРОГРАММА МОДЕЛИРОВАНИЯ И ВЫЧИСЛИТЕЛЬНОГО ЭКСПЕРИМЕНТА")
    print("           Версия 1.0 (Консольный режим + SQLite)")
    print("=" * 65)

def run_new_experiment():
    print("\n--- Ввод параметров вычислительного эксперимента ---")
    try:
        module_name = input("Введите наименование программного модуля [AuthModule]: ").strip()
        if not module_name:
            module_name = "AuthModule"
            
        iterations_str = input("Количество испытаний N (напр. 10000): ").strip()
        iterations = int(iterations_str) if iterations_str else 10000
        if iterations <= 0:
            print("Ошибка: число испытаний должно быть положительным!")
            return
            
        lambda_str = input("Интенсивность отказов lambda (напр. 0.005): ").strip()
        lambda_rate = float(lambda_str) if lambda_str else 0.005
        if lambda_rate <= 0:
            print("Ошибка: интенсивность отказов должна быть больше 0!")
            return
            
        time_limit_str = input("Контрольное время работы T (напр. 100): ").strip()
        time_limit = float(time_limit_str) if time_limit_str else 100.0
        if time_limit <= 0:
            print("Ошибка: контрольное время должно быть больше 0!")
            return
    except ValueError as e:
        print(f"Ошибка ввода числовых данных: {e}")
        return

    print("\nЗапуск вычислительного эксперимента методом Монте-Карло...")
    res = run_simulation(module_name, iterations, lambda_rate, time_limit)
    
    print("\n" + "-" * 50)
    print("           РЕЗУЛЬТАТЫ ЭКСПЕРИМЕНТА")
    print("-" * 50)
    print(f"Тестируемый модуль:                 {res['module_name']}")
    print(f"Число итераций:                     {res['iterations']:,}")
    print(f"Теоретическая надежность P(T):      {res['theoretical_p']:.4f}")
    print(f"Эмпирическая надежность P*(T):      {res['empirical_p']:.4f}")
    print(f"Средняя наработка на отказ (MTTF):  {res['mttf']:.2f} ч.")
    print(f"Относительная погрешность:          {res['relative_error']:.2f}%")
    print(f"Время вычислений:                   {res['calc_time_ms']:.2f} мс")
    print("-" * 50)
    
    exp_id = save_experiment(
        res['module_name'], res['iterations'], res['lambda_rate'], res['time_limit'],
        res['theoretical_p'], res['empirical_p'], res['mttf'], res['relative_error'], res['calc_time_ms']
    )
    print(f"[УСПЕХ] Результат сохранен в базу данных SQLite (ID записи: {exp_id}).")

def show_history():
    experiments = get_all_experiments()
    if not experiments:
        print("\nБаза данных пуста. Сначала проведите хотя бы один эксперимент.")
        return
        
    print("\n" + "=" * 90)
    print(f"{'ID':<4} {'Дата и время':<20} {'Модуль':<15} {'N':<8} {'P_теор':<8} {'P_эмп':<8} {'Погр(%)':<8} {'Время(мс)':<8}")
    print("-" * 90)
    for row in experiments:
        print(f"{row['id']:<4} {row['created_at']:<20} {row['module_name']:<15} {row['iterations']:<8} "
              f"{row['theoretical_p']:<8.4f} {row['empirical_p']:<8.4f} {row['relative_error']:<8.2f} {row['calc_time_ms']:<8.2f}")
    print("=" * 90)

def main():
    init_db()
    while True:
        print_header()
        print("1. Провести новый вычислительный эксперимент")
        print("2. Просмотреть историю экспериментов из базы данных SQLite")
        print("3. Выход")
        choice = input("\nВыберите действие (1-3): ").strip()
        
        if choice == "1":
            run_new_experiment()
        elif choice == "2":
            show_history()
        elif choice == "3":
            print("\nЗавершение работы программы.")
            break
        else:
            print("\nНеверный выбор. Пожалуйста, введите 1, 2 или 3.")
        input("\nНажмите Enter для продолжения...")

if __name__ == '__main__':
    main()
