# -*- coding: utf-8 -*-
"""
Главный модуль приложения (Версия 2.0).
Точка входа в программу: поддерживает запуск современного графического интерфейса (GUI на Tkinter)
или консольного режима (флаг --cli).
Студент: Касимов Дмитрий Андреевич, гр. ИСП 31-9
ПМ.03 Ревьюирование программных продуктов
"""
import sys

def main():
    if "--cli" in sys.argv:
        import database
        from experiment import run_simulation
        database.init_db()
        print("=" * 65)
        print("   ПРОГРАММА МОДЕЛИРОВАНИЯ И ВЫЧИСЛИТЕЛЬНОГО ЭКСПЕРИМЕНТА")
        print("           Версия 2.0 (Консольный режим + SQLite)")
        print("=" * 65)
        res = run_simulation("PaymentModule", 25000, 0.003, 100)
        exp_id = database.save_experiment(res)
        print(f"Эксперимент завершен! P*(T)={res['empirical_p']}, MTTF={res['mttf']} ч. ID: {exp_id}")
    else:
        # По умолчанию запуск полноценного оконного GUI интерфейса
        import gui
        gui.start_gui()

if __name__ == '__main__':
    main()
