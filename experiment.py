# -*- coding: utf-8 -*-
"""
Модуль вычислительного эксперимента для сервиса поиска попутчиков.
Реализует математическую модель себестоимости поездки и распределения затрат
между участниками совместной поездки (Carpooling Cost-Sharing & Optimization Model).
"""

def calculate_fuel_cost(distance_km: float, fuel_consumption: float, fuel_price: float) -> float:
    """
    Расчет прямых расходов на топливо:
    C_fuel = (D * C_cons / 100) * P_fuel
    где D - расстояние (км), C_cons - расход л/100км, P_fuel - цена руб/л.
    """
    if distance_km <= 0 or fuel_consumption <= 0 or fuel_price <= 0:
        raise ValueError("Параметры расстояния, расхода и цены должны быть положительными числами.")
    return (distance_km * fuel_consumption / 100.0) * fuel_price

def calculate_depreciation_cost(distance_km: float, depreciation_per_km: float) -> float:
    """
    Расчет сопутствующих амортизационных расходов (износ шин, ТО, техжидкости):
    C_deprec = D * k_deprec
    """
    if distance_km <= 0 or depreciation_per_km < 0:
        raise ValueError("Расстояние должно быть > 0, норма амортизации >= 0.")
    return distance_km * depreciation_per_km

def calculate_total_trip_cost(distance_km: float, fuel_consumption: float, fuel_price: float, depreciation_per_km: float = 1.8) -> float:
    """
    Расчет полной себестоимости поездки:
    C_total = C_fuel + C_deprec
    """
    c_fuel = calculate_fuel_cost(distance_km, fuel_consumption, fuel_price)
    c_deprec = calculate_depreciation_cost(distance_km, depreciation_per_km)
    return round(c_fuel + c_deprec, 2)

def calculate_cost_sharing(total_cost: float, passengers_count: int) -> dict:
    """
    Математическое моделирование распределения расходов между водителем и попутчиками:
    - Общее количество участников в автомобиле = passengers_count + 1 (водитель + попутчики)
    - Справедливая доля на одного человека: Cost_per_person = C_total / (passengers_count + 1)
    - Экономия водителя: Savings_driver = C_total - Cost_per_person
    - Доля покрытия расходов: Coverage_percent = (Savings_driver / C_total) * 100%
    """
    if total_cost <= 0:
        raise ValueError("Полная себестоимость поездки должна быть больше 0.")
    if passengers_count < 0:
        raise ValueError("Количество пассажиров не может быть отрицательным.")
    
    total_persons = passengers_count + 1
    cost_per_person = round(total_cost / total_persons, 2)
    driver_savings = round(total_cost - cost_per_person, 2)
    coverage_percent = round((driver_savings / total_cost) * 100.0, 1) if total_cost > 0 else 0.0
    
    return {
        "passengers_count": passengers_count,
        "total_persons": total_persons,
        "total_cost": total_cost,
        "cost_per_person": cost_per_person,
        "driver_savings": driver_savings,
        "coverage_percent": coverage_percent
    }

def run_experiment_grid(distance_km: float, fuel_consumption: float = 8.5, fuel_price: float = 58.5, depreciation_per_km: float = 1.8) -> list:
    """
    Проведение вычислительного эксперимента: расчет сетки распределения затрат
    для различного количества попутчиков (от 1 до 4 пассажиров).
    """
    total_cost = calculate_total_trip_cost(distance_km, fuel_consumption, fuel_price, depreciation_per_km)
    results = []
    for n in range(1, 5):
        res = calculate_cost_sharing(total_cost, n)
        results.append(res)
    return results
