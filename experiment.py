# -*- coding: utf-8 -*-
"""
Модуль проведения вычислительного эксперимента.
Версия 2.0 (Оптимизированная версия с расширенным статистическим анализом).
Математическая модель: Моделирование надежности методом Монте-Карло с расчетом дисперсии,
доверительного интервала 95% и классификации отказоустойчивости.
"""
import math
import random
import time

def run_simulation(module_name: str, iterations: int, lambda_rate: float, time_limit: float):
    """
    Проводит вычислительный эксперимент методом Монте-Карло с углубленным статистическим анализом.
    
    Параметры:
      - module_name: наименование тестируемого программного модуля
      - iterations: количество итераций (испытаний) N
      - lambda_rate: интенсивность отказов (лямбда > 0, отказов/час)
      - time_limit: контрольное время безотказной работы T (часы)
      
    Возвращает словарь с рассчитанными показателями.
    """
    start_time = time.perf_counter()
    
    # 1. Теоретическая вероятность безотказной работы по экспоненциальному закону надежности
    theoretical_p = math.exp(-lambda_rate * time_limit)
    
    # 2. Вычислительный эксперимент методом Монте-Карло
    successful_runs = 0
    ttf_values = []
    
    for _ in range(iterations):
        u = random.random()
        while u >= 1.0:
            u = random.random()
        # Время наработки до отказа: t = -ln(1 - U) / lambda
        ttf = -math.log(1.0 - u) / lambda_rate
        ttf_values.append(ttf)
        
        if ttf >= time_limit:
            successful_runs += 1
            
    calc_time_ms = (time.perf_counter() - start_time) * 1000.0
    
    # 3. Эмпирические статистические оценки
    empirical_p = successful_runs / iterations
    mttf = sum(ttf_values) / iterations  # Средняя наработка на отказ (Mean Time To Failure)
    
    # Выборочная дисперсия и стандартное отклонение наработки на отказ
    if iterations > 1:
        variance = sum((t - mttf) ** 2 for t in ttf_values) / (iterations - 1)
        std_dev = math.sqrt(variance)
        # 95% доверительный интервал для среднего времени наработки
        margin_error = 1.96 * (std_dev / math.sqrt(iterations))
        ci_lower = max(0.0, mttf - margin_error)
        ci_upper = mttf + margin_error
    else:
        variance = 0.0
        std_dev = 0.0
        ci_lower = mttf
        ci_upper = mttf
        
    # Относительная погрешность эксперимента (%)
    if theoretical_p > 0:
        relative_error = (abs(theoretical_p - empirical_p) / theoretical_p) * 100.0
    else:
        relative_error = 0.0
        
    # 4. Классификация уровня надежности программного модуля
    if empirical_p >= 0.90:
        status = "Высокая надежность"
    elif empirical_p >= 0.70:
        status = "Удовлетворительная надежность"
    else:
        status = "Требуется доработка"
        
    return {
        "module_name": module_name,
        "iterations": iterations,
        "lambda_rate": lambda_rate,
        "time_limit": time_limit,
        "theoretical_p": round(theoretical_p, 4),
        "empirical_p": round(empirical_p, 4),
        "mttf": round(mttf, 2),
        "std_dev": round(std_dev, 2),
        "ci_lower": round(ci_lower, 2),
        "ci_upper": round(ci_upper, 2),
        "relative_error": round(relative_error, 2),
        "calc_time_ms": round(calc_time_ms, 2),
        "status": status
    }
