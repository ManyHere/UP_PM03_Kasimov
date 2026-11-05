# -*- coding: utf-8 -*-
"""
Графический интерфейс пользователя (GUI) на Tkinter/ttk.
Версия 2.0 (Оконное приложение с визуализацией расчетов, управлением базой данных и экспортом).
"""
import tkinter as tk
from tkinter import ttk, messagebox, filedialog
import os
import database
import experiment

class ReliabilityApp(tk.Tk):
    def __init__(self):
        super().__init__()
        self.title("Анализ надежности ПО (v2.0) — Касимов Д.А. ИСП 31-9")
        self.geometry("980x640")
        self.minsize(850, 550)
        
        # Инициализация базы данных SQLite
        database.init_db()
        
        self._setup_styles()
        self._create_widgets()
        self.refresh_history()

    def _setup_styles(self):
        self.style = ttk.Style(self)
        self.style.theme_use("clam")
        
        # Настройка шрифтов и цветов
        self.style.configure(".", font=("Segoe UI", 10))
        self.style.configure("Treeview.Heading", font=("Segoe UI", 10, "bold"), background="#e1e4e8")
        self.style.configure("Treeview", rowheight=24)
        self.style.configure("Primary.TButton", font=("Segoe UI", 10, "bold"), background="#0366d6", foreground="white")
        self.style.map("Primary.TButton", background=[("active", "#0056b3")])

    def _create_widgets(self):
        # 1. Верхний фрейм параметров и результатов
        top_frame = ttk.Frame(self, padding="10")
        top_frame.pack(fill=tk.X)
        
        # Левая колонка: Ввод параметров
        input_group = ttk.LabelFrame(top_frame, text=" Параметры вычислительного эксперимента ", padding="10")
        input_group.pack(side=tk.LEFT, fill=tk.BOTH, expand=True, padx=(0, 5))
        
        ttk.Label(input_group, text="Наименование модуля:").grid(row=0, column=0, sticky=tk.W, pady=3)
        self.entry_module = ttk.Entry(input_group, width=24)
        self.entry_module.insert(0, "PaymentModule")
        self.entry_module.grid(row=0, column=1, sticky=tk.W, pady=3)
        
        ttk.Label(input_group, text="Число итераций (N):").grid(row=1, column=0, sticky=tk.W, pady=3)
        self.entry_iterations = ttk.Entry(input_group, width=24)
        self.entry_iterations.insert(0, "20000")
        self.entry_iterations.grid(row=1, column=1, sticky=tk.W, pady=3)
        
        ttk.Label(input_group, text="Интенсивность отказов (λ):").grid(row=2, column=0, sticky=tk.W, pady=3)
        self.entry_lambda = ttk.Entry(input_group, width=24)
        self.entry_lambda.insert(0, "0.003")
        self.entry_lambda.grid(row=2, column=1, sticky=tk.W, pady=3)
        
        ttk.Label(input_group, text="Контрольное время (T, ч):").grid(row=3, column=0, sticky=tk.W, pady=3)
        self.entry_time = ttk.Entry(input_group, width=24)
        self.entry_time.insert(0, "100")
        self.entry_time.grid(row=3, column=1, sticky=tk.W, pady=3)
        
        btn_frame = ttk.Frame(input_group)
        btn_frame.grid(row=4, column=0, columnspan=2, pady=(10, 0), sticky=tk.EW)
        
        self.btn_run = ttk.Button(btn_frame, text="▶ Рассчитать и сохранить", style="Primary.TButton", command=self.on_run_experiment)
        self.btn_run.pack(side=tk.LEFT, padx=(0, 5), expand=True, fill=tk.X)
        
        self.btn_clear = ttk.Button(btn_frame, text="Очистить", command=self.on_clear_inputs)
        self.btn_clear.pack(side=tk.LEFT, fill=tk.X)

        # Правая колонка: Карточка результатов
        result_group = ttk.LabelFrame(top_frame, text=" Результаты текущего эксперимента ", padding="10")
        result_group.pack(side=tk.RIGHT, fill=tk.BOTH, expand=True, padx=(5, 0))
        
        self.lbl_p_theor = ttk.Label(result_group, text="Теоретическая надежность P(T): —", font=("Segoe UI", 10))
        self.lbl_p_theor.pack(anchor=tk.W, pady=2)
        
        self.lbl_p_emp = ttk.Label(result_group, text="Эмпирическая надежность P*(T): —", font=("Segoe UI", 10, "bold"))
        self.lbl_p_emp.pack(anchor=tk.W, pady=2)
        
        self.lbl_mttf = ttk.Label(result_group, text="Средняя наработка на отказ (MTTF): —", font=("Segoe UI", 10))
        self.lbl_mttf.pack(anchor=tk.W, pady=2)
        
        self.lbl_ci = ttk.Label(result_group, text="Доверительный интервал 95%: —", font=("Segoe UI", 10))
        self.lbl_ci.pack(anchor=tk.W, pady=2)
        
        self.lbl_error = ttk.Label(result_group, text="Относительная погрешность: —", font=("Segoe UI", 10))
        self.lbl_error.pack(anchor=tk.W, pady=2)
        
        self.lbl_time = ttk.Label(result_group, text="Время выполнения эксперимента: —", font=("Segoe UI", 9, "italic"))
        self.lbl_time.pack(anchor=tk.W, pady=2)
        
        self.lbl_status = ttk.Label(result_group, text="Статус: Ожидание запуска", font=("Segoe UI", 10, "bold"), foreground="#555")
        self.lbl_status.pack(anchor=tk.W, pady=4)

        # 2. Нижний фрейм: Таблица истории из базы данных SQLite
        history_group = ttk.LabelFrame(self, text=" База данных экспериментов (SQLite) ", padding="10")
        history_group.pack(fill=tk.BOTH, expand=True, padx="10", pady=(0, 10))
        
        # Панель инструментов таблицы
        toolbar = ttk.Frame(history_group)
        toolbar.pack(fill=tk.X, pady=(0, 6))
        
        ttk.Button(toolbar, text="🔄 Обновить", command=self.refresh_history).pack(side=tk.LEFT, padx=(0, 5))
        ttk.Button(toolbar, text="📥 Экспорт в CSV", command=self.on_export_csv).pack(side=tk.LEFT, padx=(0, 5))
        ttk.Button(toolbar, text="🗑 Удалить выбранную запись", command=self.on_delete_selected).pack(side=tk.LEFT)
        
        self.lbl_db_info = ttk.Label(toolbar, text="Всего записей: 0", font=("Segoe UI", 9, "italic"))
        self.lbl_db_info.pack(side=tk.RIGHT)

        # Таблица Treeview
        columns = ("id", "date", "module", "n", "p_th", "p_emp", "mttf", "error", "time", "status")
        self.tree = ttk.Treeview(history_group, columns=columns, show="headings", selectmode="browse")
        
        self.tree.heading("id", text="ID")
        self.tree.heading("date", text="Дата и время")
        self.tree.heading("module", text="Модуль")
        self.tree.heading("n", text="Итераций")
        self.tree.heading("p_th", text="P(T) теор.")
        self.tree.heading("p_emp", text="P*(T) эмп.")
        self.tree.heading("mttf", text="MTTF (ч)")
        self.tree.heading("error", text="Погрешн. (%)")
        self.tree.heading("time", text="Время (мс)")
        self.tree.heading("status", text="Статус")
        
        self.tree.column("id", width=40, anchor=tk.CENTER)
        self.tree.column("date", width=140, anchor=tk.CENTER)
        self.tree.column("module", width=130)
        self.tree.column("n", width=80, anchor=tk.CENTER)
        self.tree.column("p_th", width=85, anchor=tk.CENTER)
        self.tree.column("p_emp", width=85, anchor=tk.CENTER)
        self.tree.column("mttf", width=80, anchor=tk.CENTER)
        self.tree.column("error", width=90, anchor=tk.CENTER)
        self.tree.column("time", width=85, anchor=tk.CENTER)
        self.tree.column("status", width=140, anchor=tk.W)
        
        scrollbar = ttk.Scrollbar(history_group, orient=tk.VERTICAL, command=self.tree.yview)
        self.tree.configure(yscroll=scrollbar.set)
        
        self.tree.pack(side=tk.LEFT, fill=tk.BOTH, expand=True)
        scrollbar.pack(side=tk.RIGHT, fill=tk.Y)

    def on_run_experiment(self):
        try:
            module_name = self.entry_module.get().strip()
            if not module_name:
                raise ValueError("Наименование модуля не может быть пустым.")
                
            iterations = int(self.entry_iterations.get().strip())
            if iterations <= 0 or iterations > 1000000:
                raise ValueError("Количество итераций должно быть от 1 до 1 000 000.")
                
            lambda_rate = float(self.entry_lambda.get().strip())
            if lambda_rate <= 0:
                raise ValueError("Интенсивность отказов должна быть строго больше 0.")
                
            time_limit = float(self.entry_time.get().strip())
            if time_limit <= 0:
                raise ValueError("Контрольное время должно быть больше 0.")
        except ValueError as e:
            messagebox.showerror("Ошибка валидации", f"Некорректные параметры:\n{e}")
            return

        # Проведение вычислительного эксперимента
        res = experiment.run_simulation(module_name, iterations, lambda_rate, time_limit)
        
        # Обновление карточки результатов
        self.lbl_p_theor.config(text=f"Теоретическая надежность P(T): {res['theoretical_p']:.4f}")
        self.lbl_p_emp.config(text=f"Эмпирическая надежность P*(T): {res['empirical_p']:.4f}")
        self.lbl_mttf.config(text=f"Средняя наработка на отказ (MTTF): {res['mttf']:.2f} ч.")
        self.lbl_ci.config(text=f"Доверительный интервал 95%: [{res['ci_lower']:.2f}; {res['ci_upper']:.2f}] ч.")
        self.lbl_error.config(text=f"Относительная погрешность: {res['relative_error']:.2f}%")
        self.lbl_time.config(text=f"Время выполнения эксперимента: {res['calc_time_ms']:.2f} мс")
        
        status_color = "#28a745" if res["status"] == "Высокая надежность" else ("#fd7e14" if res["status"] == "Удовлетворительная надежность" else "#dc3545")
        self.lbl_status.config(text=f"Статус: {res['status']}", foreground=status_color)
        
        # Сохранение в базу данных SQLite
        exp_id = database.save_experiment(res)
        self.refresh_history()
        messagebox.showinfo("Эксперимент завершен", f"Вычислительный эксперимент успешно выполнен!\nРезультаты сохранены в SQLite (ID: {exp_id}).")

    def on_clear_inputs(self):
        self.entry_module.delete(0, tk.END)
        self.entry_iterations.delete(0, tk.END)
        self.entry_lambda.delete(0, tk.END)
        self.entry_time.delete(0, tk.END)

    def refresh_history(self):
        for item in self.tree.get_children():
            self.tree.delete(item)
            
        rows = database.get_all_experiments()
        for r in rows:
            self.tree.insert("", tk.END, values=(
                r["id"], r["created_at"], r["module_name"], f"{r['iterations']:,}",
                f"{r['theoretical_p']:.4f}", f"{r['empirical_p']:.4f}",
                f"{r['mttf']:.2f}", f"{r['relative_error']:.2f}%",
                f"{r['calc_time_ms']:.2f}", r["status"]
            ))
        self.lbl_db_info.config(text=f"Всего записей в SQLite: {len(rows)}")

    def on_delete_selected(self):
        selected = self.tree.selection()
        if not selected:
            messagebox.showwarning("Предупреждение", "Выберите запись для удаления.")
            return
        item = self.tree.item(selected[0])
        exp_id = item["values"][0]
        if messagebox.askyesno("Подтверждение", f"Удалить запись ID={exp_id} из базы данных?"):
            database.delete_experiment(exp_id)
            self.refresh_history()

    def on_export_csv(self):
        filepath = filedialog.asksaveasfilename(
            defaultextension=".csv",
            filetypes=[("CSV files", "*.csv"), ("All files", "*.*")],
            initialfile="experiments_report.csv"
        )
        if filepath:
            count = database.export_to_csv(filepath)
            messagebox.showinfo("Экспорт завершен", f"Экспортировано записей: {count}\nФайл: {os.path.basename(filepath)}")

def start_gui():
    app = ReliabilityApp()
    app.mainloop()

if __name__ == '__main__':
    start_gui()
