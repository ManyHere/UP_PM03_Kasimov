# -*- coding: utf-8 -*-
"""
Графический интерфейс пользователя (GUI) для сервиса поиска попутчиков (Версия 2.0).
Разработан на библиотеке Tkinter / ttk в рамках освоения ПМ.03.
Студент: Касимов Д.А., Группа: ИСП 31-9
"""

import tkinter as tk
from tkinter import ttk, messagebox, filedialog
from datetime import datetime
import os

from database import (
    init_database, get_all_trips, add_trip, book_seat,
    cancel_booking, get_all_bookings, save_experiment,
    export_trips_to_csv, export_bookings_to_csv
)
from experiment import (
    calculate_fuel_cost, calculate_depreciation_cost,
    calculate_total_trip_cost, run_experiment_grid
)

class CarpoolingApp(tk.Tk):
    def __init__(self):
        super().__init__()
        
        self.title("Сервис поиска попутчиков — Версия 2.0 (Касимов Д.А., ИСП 31-9)")
        self.geometry("1060x720")
        self.minsize(980, 650)
        
        # Настройка системной темы и стилей
        self.style = ttk.Style(self)
        try:
            self.style.theme_use("clam")
        except Exception:
            pass
            
        self.style.configure(".", font=("Segoe UI", 10))
        self.style.configure("Treeview.Heading", font=("Segoe UI", 10, "bold"))
        self.style.configure("Title.TLabel", font=("Segoe UI", 14, "bold"), foreground="#1a365d")
        self.style.configure("Subtitle.TLabel", font=("Segoe UI", 11, "bold"), foreground="#2b6cb0")
        self.style.configure("Primary.TButton", font=("Segoe UI", 10, "bold"), foreground="#ffffff", background="#2b6cb0")
        self.style.configure("Success.TButton", font=("Segoe UI", 10, "bold"), foreground="#ffffff", background="#2f855a")
        
        init_database()
        self.create_widgets()
        self.load_trips()
        self.load_bookings()

    def create_widgets(self):
        # Верхний информационный баннер
        top_banner = ttk.Frame(self, padding=(15, 8), relief="groove")
        top_banner.pack(fill="x", side="top")
        
        ttk.Label(
            top_banner,
            text="СЕРВИС ПОИСКА ПОПУТЧИКОВ | Информационная система совместных поездок (Версия 2.0)",
            style="Title.TLabel"
        ).pack(side="left")
        
        ttk.Label(
            top_banner,
            text="Студент: Касимов Д.А. | ИСП 31-9 | ПМ.03",
            font=("Segoe UI", 9, "italic"),
            foreground="#4a5568"
        ).pack(side="right")
        
        # Набор вкладок Notebook
        self.notebook = ttk.Notebook(self)
        self.notebook.pack(fill="both", expand=True, padx=10, pady=10)
        
        # Вкладка 1: Поиск и бронирование
        self.tab_search = ttk.Frame(self.notebook, padding=10)
        self.notebook.add(self.tab_search, text="🔍 Поиск и бронирование")
        self.build_search_tab()
        
        # Вкладка 2: Добавить поездку
        self.tab_add = ttk.Frame(self.notebook, padding=15)
        self.notebook.add(self.tab_add, text="➕ Опубликовать поездку")
        self.build_add_tab()
        
        # Вкладка 3: Вычислительный эксперимент
        self.tab_experiment = ttk.Frame(self.notebook, padding=12)
        self.notebook.add(self.tab_experiment, text="📊 Вычислительный эксперимент")
        self.build_experiment_tab()
        
        # Вкладка 4: Управление бронированиями и Экспорт
        self.tab_bookings = ttk.Frame(self.notebook, padding=10)
        self.notebook.add(self.tab_bookings, text="📋 Управление бронями и Экспорт")
        self.build_bookings_tab()
        
        # Строка состояния
        self.status_var = tk.StringVar(value="Система готова к работе. База данных SQLite подключена.")
        status_bar = ttk.Label(self, textvariable=self.status_var, relief="sunken", anchor="w", padding=(10, 4))
        status_bar.pack(fill="x", side="bottom")

    # ==================== ВКЛАДКА 1: ПОИСК И БРОНИРОВАНИЕ ====================
    def build_search_tab(self):
        # Панель фильтров
        filter_frame = ttk.LabelFrame(self.tab_search, text="Параметры поиска поездок", padding=10)
        filter_frame.pack(fill="x", side="top", pady=(0, 10))
        
        ttk.Label(filter_frame, text="Откуда:").grid(row=0, column=0, padx=5, sticky="w")
        self.search_from_entry = ttk.Entry(filter_frame, width=18)
        self.search_from_entry.grid(row=0, column=1, padx=5, sticky="w")
        self.search_from_entry.insert(0, "Ижевск")
        
        ttk.Label(filter_frame, text="Куда:").grid(row=0, column=2, padx=5, sticky="w")
        self.search_to_entry = ttk.Entry(filter_frame, width=18)
        self.search_to_entry.grid(row=0, column=3, padx=5, sticky="w")
        
        btn_find = ttk.Button(filter_frame, text="🔍 Найти поездки", command=self.load_trips)
        btn_find.grid(row=0, column=4, padx=8)
        
        btn_reset = ttk.Button(filter_frame, text="🔄 Сбросить фильтр", command=self.reset_search_filter)
        btn_reset.grid(row=0, column=5, padx=5)
        
        # Таблица доступных рейсов
        tree_frame = ttk.Frame(self.tab_search)
        tree_frame.pack(fill="both", expand=True, pady=(0, 10))
        
        columns = ("id", "route", "datetime", "car", "driver", "phone", "seats", "price", "dist")
        self.tree_trips = ttk.Treeview(tree_frame, columns=columns, show="headings", selectmode="browse", height=10)
        
        self.tree_trips.heading("id", text="ID")
        self.tree_trips.heading("route", text="Маршрут поездки")
        self.tree_trips.heading("datetime", text="Дата и время")
        self.tree_trips.heading("car", text="Автомобиль")
        self.tree_trips.heading("driver", text="Водитель")
        self.tree_trips.heading("phone", text="Телефон")
        self.tree_trips.heading("seats", text="Свободно мест")
        self.tree_trips.heading("price", text="Цена за место")
        self.tree_trips.heading("dist", text="Дистанция (км)")
        
        self.tree_trips.column("id", width=45, anchor="center")
        self.tree_trips.column("route", width=200, anchor="w")
        self.tree_trips.column("datetime", width=130, anchor="center")
        self.tree_trips.column("car", width=130, anchor="w")
        self.tree_trips.column("driver", width=150, anchor="w")
        self.tree_trips.column("phone", width=130, anchor="center")
        self.tree_trips.column("seats", width=95, anchor="center")
        self.tree_trips.column("price", width=105, anchor="e")
        self.tree_trips.column("dist", width=95, anchor="center")
        
        scroll_y = ttk.Scrollbar(tree_frame, orient="vertical", command=self.tree_trips.yview)
        self.tree_trips.configure(yscrollcommand=scroll_y.set)
        
        self.tree_trips.pack(side="left", fill="both", expand=True)
        scroll_y.pack(side="right", fill="y")
        
        self.tree_trips.bind("<<TreeviewSelect>>", self.on_trip_selected)
        
        # Панель оформления бронирования
        book_frame = ttk.LabelFrame(self.tab_search, text="Оформление бронирования для пассажира", padding=10)
        book_frame.pack(fill="x", side="bottom")
        
        self.lbl_selected_trip = ttk.Label(book_frame, text="Выберите поездку из таблицы выше для бронирования места.", style="Subtitle.TLabel")
        self.lbl_selected_trip.grid(row=0, column=0, columnspan=6, sticky="w", pady=(0, 8))
        
        ttk.Label(book_frame, text="ФИО пассажира:").grid(row=1, column=0, padx=5, sticky="w")
        self.book_name_entry = ttk.Entry(book_frame, width=22)
        self.book_name_entry.grid(row=1, column=1, padx=5, sticky="w")
        self.book_name_entry.insert(0, "Касимов Дмитрий")
        
        ttk.Label(book_frame, text="Телефон пассажира:").grid(row=1, column=2, padx=5, sticky="w")
        self.book_phone_entry = ttk.Entry(book_frame, width=18)
        self.book_phone_entry.grid(row=1, column=3, padx=5, sticky="w")
        self.book_phone_entry.insert(0, "+7 (912) 850-12-34")
        
        ttk.Label(book_frame, text="Кол-во мест:").grid(row=1, column=4, padx=5, sticky="w")
        self.book_seats_spin = ttk.Spinbox(book_frame, from_=1, to=4, width=5)
        self.book_seats_spin.grid(row=1, column=5, padx=5, sticky="w")
        self.book_seats_spin.set(1)
        
        self.btn_book = ttk.Button(book_frame, text="✅ Забронировать место", command=self.do_booking, state="disabled")
        self.btn_book.grid(row=1, column=6, padx=15, sticky="e")

    def reset_search_filter(self):
        self.search_from_entry.delete(0, tk.END)
        self.search_to_entry.delete(0, tk.END)
        self.load_trips()

    def load_trips(self):
        for item in self.tree_trips.get_children():
            self.tree_trips.delete(item)
            
        from_c = self.search_from_entry.get().strip() if hasattr(self, "search_from_entry") else None
        to_c = self.search_to_entry.get().strip() if hasattr(self, "search_to_entry") else None
        
        trips = get_all_trips(from_c, to_c)
        for t in trips:
            t_id, drv_name, drv_phone, car, f_c, t_c, t_date, t_time, seats, price, dist = t
            route = f"{f_c} → {t_c}"
            dt = f"{t_date} {t_time}"
            price_str = f"{price:.1f} руб."
            dist_str = f"{dist:.0f}"
            self.tree_trips.insert("", "end", values=(t_id, route, dt, car, drv_name, drv_phone, seats, price_str, dist_str))
            
        self.status_var.set(f"Загружено доступных поездок: {len(trips)}")
        if hasattr(self, "btn_book"):
            self.btn_book.config(state="disabled")
            self.lbl_selected_trip.config(text="Выберите поездку из таблицы выше для бронирования места.")

    def on_trip_selected(self, event):
        selected = self.tree_trips.selection()
        if not selected:
            return
        vals = self.tree_trips.item(selected[0], "values")
        self.selected_trip_id = int(vals[0])
        route = vals[1]
        dt = vals[2]
        price = vals[7]
        available_seats = int(vals[6])
        
        self.lbl_selected_trip.config(text=f"Выбрана поездка #{self.selected_trip_id}: {route} ({dt}) | Стоимость: {price} | Доступно мест: {available_seats}")
        self.book_seats_spin.config(to=max(1, available_seats))
        self.btn_book.config(state="normal")

    def do_booking(self):
        if not hasattr(self, "selected_trip_id"):
            messagebox.showwarning("Внимание", "Пожалуйста, выберите поездку из списка.")
            return
            
        name = self.book_name_entry.get().strip()
        phone = self.book_phone_entry.get().strip()
        try:
            seats = int(self.book_seats_spin.get())
        except ValueError:
            messagebox.showerror("Ошибка", "Количество мест должно быть числом.")
            return
            
        if not name or not phone:
            messagebox.showwarning("Внимание", "Заполните ФИО и контактный телефон пассажира.")
            return
            
        try:
            booking_id, total_cost = book_seat(self.selected_trip_id, name, phone, seats)
            messagebox.showinfo(
                "Успешное бронирование",
                f"Бронь #{booking_id} успешно оформлена!\n"
                f"Пассажир: {name}\n"
                f"Забронировано мест: {seats}\n"
                f"Сумма к оплате водителю: {total_cost:.2f} руб."
            )
            self.load_trips()
            self.load_bookings()
            self.status_var.set(f"Бронь #{booking_id} успешно создана.")
        except Exception as e:
            messagebox.showerror("Ошибка бронирования", str(e))

    # ==================== ВКЛАДКА 2: ОПУБЛИКОВАТЬ ПОЕЗДКУ ====================
    def build_add_tab(self):
        card = ttk.LabelFrame(self.tab_add, text="Данные новой поездки водителя", padding=20)
        card.pack(fill="x", pady=10)
        
        # Поля формы
        fields = [
            ("ФИО водителя:", "driver_name", "Касимов Дмитрий Андреевич"),
            ("Контактный телефон:", "driver_phone", "+7 (912) 850-12-34"),
            ("Марка и модель авто:", "car_model", "Lada Vesta SW Cross"),
            ("Город отправления:", "from_city", "Ижевск"),
            ("Город назначения:", "to_city", "Уфа"),
            ("Дата поездки (ГГГГ-ММ-ДД):", "trip_date", "2026-11-15"),
            ("Время отправления (ЧЧ:ММ):", "trip_time", "07:30"),
            ("Количество свободных мест:", "seats_total", "3"),
            ("Расстояние маршрута (км):", "distance_km", "340.0"),
            ("Стоимость 1 места (руб):", "price_per_seat", "850.0")
        ]
        
        self.add_entries = {}
        for idx, (label_text, key, def_val) in enumerate(fields):
            r = idx // 2
            c = (idx % 2) * 2
            ttk.Label(card, text=label_text).grid(row=r, column=c, padx=10, pady=8, sticky="w")
            ent = ttk.Entry(card, width=28)
            ent.grid(row=r, column=c+1, padx=10, pady=8, sticky="w")
            ent.insert(0, def_val)
            self.add_entries[key] = ent
            
        btn_box = ttk.Frame(card)
        btn_box.grid(row=5, column=0, columnspan=4, pady=20)
        
        btn_submit = ttk.Button(btn_box, text="🚀 Опубликовать поездку в системе", command=self.do_add_trip, style="Success.TButton")
        btn_submit.pack(side="left", padx=10)
        
        btn_fill_sample = ttk.Button(btn_box, text="Заполнить маршрут Ижевск — Сарапул", command=self.fill_sample_route)
        btn_fill_sample.pack(side="left", padx=10)

    def fill_sample_route(self):
        samples = {
            "from_city": "Ижевск",
            "to_city": "Сарапул",
            "distance_km": "65.0",
            "price_per_seat": "220.0",
            "seats_total": "4"
        }
        for k, v in samples.items():
            if k in self.add_entries:
                self.add_entries[k].delete(0, tk.END)
                self.add_entries[k].insert(0, v)

    def do_add_trip(self):
        try:
            drv_name = self.add_entries["driver_name"].get().strip()
            drv_phone = self.add_entries["driver_phone"].get().strip()
            car = self.add_entries["car_model"].get().strip()
            from_c = self.add_entries["from_city"].get().strip()
            to_c = self.add_entries["to_city"].get().strip()
            t_date = self.add_entries["trip_date"].get().strip()
            t_time = self.add_entries["trip_time"].get().strip()
            seats = int(self.add_entries["seats_total"].get().strip())
            dist = float(self.add_entries["distance_km"].get().strip())
            price = float(self.add_entries["price_per_seat"].get().strip())
            
            if not drv_name or not drv_phone or not from_c or not to_c:
                raise ValueError("Все текстовые поля обязательны для заполнения.")
            if seats <= 0 or dist <= 0 or price < 0:
                raise ValueError("Числовые параметры мест, расстояния и цены должны быть положительными.")
                
            trip_id = add_trip(drv_name, drv_phone, car, from_c, to_c, t_date, t_time, seats, dist, price)
            messagebox.showinfo(
                "Успех",
                f"Поездка #{trip_id} успешно опубликована!\n"
                f"Маршрут: {from_c} → {to_c}\n"
                f"Свободных мест: {seats}\n"
                f"Тариф: {price:.2f} руб./место"
            )
            self.load_trips()
            self.notebook.select(self.tab_search)
            self.status_var.set(f"Опубликована новая поездка #{trip_id} ({from_c} → {to_c})")
        except Exception as e:
            messagebox.showerror("Ошибка ввода данных", str(e))

    # ==================== ВКЛАДКА 3: ВЫЧИСЛИТЕЛЬНЫЙ ЭКСПЕРИМЕНТ ====================
    def build_experiment_tab(self):
        top_exp = ttk.LabelFrame(self.tab_experiment, text="Входные параметры вычислительного эксперимента", padding=10)
        top_exp.pack(fill="x", side="top", pady=(0, 10))
        
        ttk.Label(top_exp, text="Маршрут:").grid(row=0, column=0, padx=5, pady=4, sticky="w")
        self.exp_route_ent = ttk.Entry(top_exp, width=20)
        self.exp_route_ent.grid(row=0, column=1, padx=5, pady=4, sticky="w")
        self.exp_route_ent.insert(0, "Ижевск — Казань")
        
        ttk.Label(top_exp, text="Дистанция (км):").grid(row=0, column=2, padx=5, pady=4, sticky="w")
        self.exp_dist_ent = ttk.Entry(top_exp, width=12)
        self.exp_dist_ent.grid(row=0, column=3, padx=5, pady=4, sticky="w")
        self.exp_dist_ent.insert(0, "390.0")
        
        ttk.Label(top_exp, text="Расход (л/100 км):").grid(row=0, column=4, padx=5, pady=4, sticky="w")
        self.exp_cons_ent = ttk.Entry(top_exp, width=10)
        self.exp_cons_ent.grid(row=0, column=5, padx=5, pady=4, sticky="w")
        self.exp_cons_ent.insert(0, "8.5")
        
        ttk.Label(top_exp, text="Цена бензина (руб/л):").grid(row=1, column=0, padx=5, pady=4, sticky="w")
        self.exp_fuel_price_ent = ttk.Entry(top_exp, width=20)
        self.exp_fuel_price_ent.grid(row=1, column=1, padx=5, pady=4, sticky="w")
        self.exp_fuel_price_ent.insert(0, "58.5")
        
        ttk.Label(top_exp, text="Амортизация (руб/км):").grid(row=1, column=2, padx=5, pady=4, sticky="w")
        self.exp_deprec_ent = ttk.Entry(top_exp, width=12)
        self.exp_deprec_ent.grid(row=1, column=3, padx=5, pady=4, sticky="w")
        self.exp_deprec_ent.insert(0, "1.8")
        
        ttk.Label(top_exp, text="Поездок в год:").grid(row=1, column=4, padx=5, pady=4, sticky="w")
        self.exp_trips_year_ent = ttk.Entry(top_exp, width=10)
        self.exp_trips_year_ent.grid(row=1, column=5, padx=5, pady=4, sticky="w")
        self.exp_trips_year_ent.insert(0, "40")
        
        btn_run = ttk.Button(top_exp, text="▶ Провести вычислительный эксперимент", command=self.do_run_experiment, style="Primary.TButton")
        btn_run.grid(row=0, column=6, rowspan=2, padx=15, sticky="nsew")
        
        # Информационная сводка
        self.exp_summary_lbl = ttk.Label(
            self.tab_experiment,
            text="Нажмите «Провести эксперимент» для моделирования себестоимости и распределения расходов.",
            style="Subtitle.TLabel"
        )
        self.exp_summary_lbl.pack(fill="x", pady=5)
        
        # Таблица результатов симуляции для 1..4 попутчиков
        exp_table_frame = ttk.Frame(self.tab_experiment)
        exp_table_frame.pack(fill="both", expand=True, pady=5)
        
        exp_cols = ("passengers", "persons", "cost_per_person", "savings", "coverage", "annual")
        self.tree_exp = ttk.Treeview(exp_table_frame, columns=exp_cols, show="headings", height=6)
        
        self.tree_exp.heading("passengers", text="Число попутчиков (N)")
        self.tree_exp.heading("persons", text="Всего людей в салоне")
        self.tree_exp.heading("cost_per_person", text="Справедливая доля на человека")
        self.tree_exp.heading("savings", text="Экономия водителя за рейс")
        self.tree_exp.heading("coverage", text="Доля покрытия расходов")
        self.tree_exp.heading("annual", text="Прогноз годовой экономии")
        
        self.tree_exp.column("passengers", width=150, anchor="center")
        self.tree_exp.column("persons", width=150, anchor="center")
        self.tree_exp.column("cost_per_person", width=190, anchor="e")
        self.tree_exp.column("savings", width=190, anchor="e")
        self.tree_exp.column("coverage", width=160, anchor="center")
        self.tree_exp.column("annual", width=190, anchor="e")
        
        self.tree_exp.pack(fill="both", expand=True)
        
        # Нижняя панель сохранения эксперимента
        bot_exp = ttk.Frame(self.tab_experiment, padding=5)
        bot_exp.pack(fill="x", side="bottom")
        
        self.btn_save_exp = ttk.Button(bot_exp, text="💾 Сохранить расчет эксперимента в БД SQLite", command=self.do_save_experiment, state="disabled")
        self.btn_save_exp.pack(side="right")

    def do_run_experiment(self):
        try:
            route = self.exp_route_ent.get().strip()
            dist = float(self.exp_dist_ent.get().strip())
            cons = float(self.exp_cons_ent.get().strip())
            fuel_p = float(self.exp_fuel_price_ent.get().strip())
            deprec = float(self.exp_deprec_ent.get().strip())
            t_year = int(self.exp_trips_year_ent.get().strip())
            
            c_fuel = calculate_fuel_cost(dist, cons, fuel_p)
            c_deprec = calculate_depreciation_cost(dist, deprec)
            c_total = calculate_total_trip_cost(dist, cons, fuel_p, deprec)
            
            self.last_exp_data = {
                "route": route,
                "dist": dist,
                "cons": cons,
                "fuel_p": fuel_p,
                "deprec": deprec,
                "c_total": c_total,
                "grid": run_experiment_grid(dist, cons, fuel_p, deprec, t_year)
            }
            
            summary_text = (
                f"Маршрут: {route} ({dist:.0f} км) | Полная себестоимость рейса: {c_total:.2f} руб. "
                f"[Топливо: {c_fuel:.2f} руб. + Амортизация: {c_deprec:.2f} руб.] | Себестоимость 1 км: {(c_total/dist):.2f} руб./км"
            )
            self.exp_summary_lbl.config(text=summary_text)
            
            for item in self.tree_exp.get_children():
                self.tree_exp.delete(item)
                
            for row in self.last_exp_data["grid"]:
                self.tree_exp.insert(
                    "", "end",
                    values=(
                        f"{row['passengers_count']} чел.",
                        f"{row['total_persons']} чел. (с водителем)",
                        f"{row['cost_per_person']:.2f} руб.",
                        f"{row['driver_savings']:.2f} руб.",
                        f"{row['coverage_percent']}%",
                        f"{row['annual_savings']:.2f} руб."
                    )
                )
                
            self.btn_save_exp.config(state="normal")
            self.status_var.set(f"Вычислительный эксперимент для маршрута «{route}» успешно проведен.")
        except Exception as e:
            messagebox.showerror("Ошибка расчета", str(e))

    def do_save_experiment(self):
        if not hasattr(self, "last_exp_data"):
            return
        try:
            d = self.last_exp_data
            for row in d["grid"]:
                save_experiment(
                    d["route"], d["dist"], d["cons"], d["fuel_p"], d["deprec"],
                    row["passengers_count"], row["total_cost"], row["cost_per_person"],
                    row["driver_savings"], row["annual_savings"]
                )
            messagebox.showinfo("Успех", "Результаты эксперимента сохранены в таблицу cost_experiments базы данных SQLite.")
            self.btn_save_exp.config(state="disabled")
            self.status_var.set("Результаты эксперимента сохранены в SQLite.")
        except Exception as e:
            messagebox.showerror("Ошибка сохранения", str(e))

    # ==================== ВКЛАДКА 4: БРОНИ И ЭКСПОРТ ====================
    def build_bookings_tab(self):
        btn_bar = ttk.Frame(self.tab_bookings, padding=(0, 0, 0, 8))
        btn_bar.pack(fill="x", side="top")
        
        btn_refresh = ttk.Button(btn_bar, text="🔄 Обновить список", command=self.load_bookings)
        btn_refresh.pack(side="left", padx=5)
        
        btn_cancel = ttk.Button(btn_bar, text="❌ Отменить выбранную бронь", command=self.do_cancel_booking)
        btn_cancel.pack(side="left", padx=5)
        
        btn_exp_trips = ttk.Button(btn_bar, text="📥 Экспорт поездок в CSV", command=self.do_export_trips)
        btn_exp_trips.pack(side="right", padx=5)
        
        btn_exp_books = ttk.Button(btn_bar, text="📥 Экспорт бронирований в CSV", command=self.do_export_bookings)
        btn_exp_books.pack(side="right", padx=5)
        
        # Таблица броней
        b_frame = ttk.Frame(self.tab_bookings)
        b_frame.pack(fill="both", expand=True)
        
        cols = ("b_id", "trip_id", "route", "passenger", "phone", "seats", "cost", "date", "status")
        self.tree_bookings = ttk.Treeview(b_frame, columns=cols, show="headings", height=12)
        
        self.tree_bookings.heading("b_id", text="ID брони")
        self.tree_bookings.heading("trip_id", text="№ рейса")
        self.tree_bookings.heading("route", text="Маршрут")
        self.tree_bookings.heading("passenger", text="Пассажир")
        self.tree_bookings.heading("phone", text="Телефон")
        self.tree_bookings.heading("seats", text="Мест")
        self.tree_bookings.heading("cost", text="Сумма (руб)")
        self.tree_bookings.heading("date", text="Дата и время заявки")
        self.tree_bookings.heading("status", text="Статус")
        
        self.tree_bookings.column("b_id", width=65, anchor="center")
        self.tree_bookings.column("trip_id", width=65, anchor="center")
        self.tree_bookings.column("route", width=180, anchor="w")
        self.tree_bookings.column("passenger", width=160, anchor="w")
        self.tree_bookings.column("phone", width=130, anchor="center")
        self.tree_bookings.column("seats", width=65, anchor="center")
        self.tree_bookings.column("cost", width=95, anchor="e")
        self.tree_bookings.column("date", width=140, anchor="center")
        self.tree_bookings.column("status", width=110, anchor="center")
        
        sc_y = ttk.Scrollbar(b_frame, orient="vertical", command=self.tree_bookings.yview)
        self.tree_bookings.configure(yscrollcommand=sc_y.set)
        
        self.tree_bookings.pack(side="left", fill="both", expand=True)
        sc_y.pack(side="right", fill="y")

    def load_bookings(self):
        for item in self.tree_bookings.get_children():
            self.tree_bookings.delete(item)
        bookings = get_all_bookings()
        for b in bookings:
            b_id, trip_id, route, pass_name, phone, seats, cost, b_date, status = b
            cost_str = f"{cost:.1f}"
            self.tree_bookings.insert("", "end", values=(b_id, trip_id, route, pass_name, phone, seats, cost_str, b_date, status))

    def do_cancel_booking(self):
        selected = self.tree_bookings.selection()
        if not selected:
            messagebox.showwarning("Внимание", "Пожалуйста, выделите строку бронирования для отмены.")
            return
        vals = self.tree_bookings.item(selected[0], "values")
        b_id = int(vals[0])
        status = vals[8]
        if status == "Отменено":
            messagebox.showinfo("Информация", "Данное бронирование уже отменено.")
            return
            
        if messagebox.askyesno("Подтверждение", f"Вы действительно хотите отменить бронирование #{b_id}?"):
            try:
                cancel_booking(b_id)
                messagebox.showinfo("Успех", f"Бронирование #{b_id} успешно отменено. Места возвращены в рейс.")
                self.load_bookings()
                self.load_trips()
                self.status_var.set(f"Бронирование #{b_id} отменено.")
            except Exception as e:
                messagebox.showerror("Ошибка отмены", str(e))

    def do_export_trips(self):
        fp = filedialog.asksaveasfilename(
            defaultextension=".csv",
            filetypes=[("CSV files", "*.csv"), ("All files", "*.*")],
            initialfile="trips_export.csv",
            title="Сохранить поездки в CSV"
        )
        if fp:
            try:
                count = export_trips_to_csv(fp)
                messagebox.showinfo("Экспорт завершен", f"Успешно экспортировано {count} поездок в файл:\n{fp}")
                self.status_var.set(f"Экспортировано {count} поездок в CSV.")
            except Exception as e:
                messagebox.showerror("Ошибка экспорта", str(e))

    def do_export_bookings(self):
        fp = filedialog.asksaveasfilename(
            defaultextension=".csv",
            filetypes=[("CSV files", "*.csv"), ("All files", "*.*")],
            initialfile="bookings_export.csv",
            title="Сохранить бронирования в CSV"
        )
        if fp:
            try:
                count = export_bookings_to_csv(fp)
                messagebox.showinfo("Экспорт завершен", f"Успешно экспортировано {count} бронирований в файл:\n{fp}")
                self.status_var.set(f"Экспортировано {count} бронирований в CSV.")
            except Exception as e:
                messagebox.showerror("Ошибка экспорта", str(e))

def start_gui():
    app = CarpoolingApp()
    app.mainloop()

if __name__ == "__main__":
    start_gui()
