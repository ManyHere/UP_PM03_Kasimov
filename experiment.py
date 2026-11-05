# -*- coding: utf-8 -*-
"""
Модуль проведения вычислительного эксперимента.
Версия 1.0 (Базовая версия).
Математическая модель: Моделирование надежности программного модуля методом Монте-Карло.
"""
import math
import random
import time

def run_simulation(module_name: str, iterations: int, lambda_rate: float, time_limit: float):
    """
    Проводит вычислительный эксперимент методом Монте-Карло.
    
    Параметры:
      - module_name: наименование программного модуля
      - iterations: число испытаний (N)
      - lambda_rate: интенсивность отказов (лямбда > 0)
      - time_limit: контрольное время безотказной работы (T)
      
    Возвращает словарь с результатами вычислений.
    """
    start_time = time.perf_counter()
    
    # Теоретическая вероятность безотказной работы по экспоненциальному закону: P(T) = exp(-lambda * T)
    theoretical_p = math.exp(-lambda_rate * time_limit)
    
    successful_runs = 0
    ttf_sum = 0.0  # Сумма времен наработки до отказа
    
    for _ in range(iterations):
        # Моделирование случайного времени наработки до отказа методом обратной функции: t = -ln(1 - U) / lambda
        u = random.random()
        # Защита от u = 1.0
        while u >= 1.0:
            u = random.random()
        ttf = -math.log(1.0 - u) / lambda_rate
        ttf_sum += ttf
        
        # Если время работы до отказа превышает контрольное время T, испытание успешно
        if ttf >= time_limit:
            successful_runs += 1
            
    calc_time_ms = (time.perf_counter() - start_time) * 1000.0
    
    # Эмпирическая вероятность безотказной работы
    empirical_p = successful_runs / iterations
    
    # Средняя наработка на отказ (MTTF)
    mttf = ttf_sum / iterations
    
    # Относительная погрешность эксперимента (%)
    if theoretical_p > 0:
        relative_error = (abs(theoretical_p - empirical_p) / theoretical_p) * 100.0
    else:
        relative_error = 0.0
        
    return {
        "module_name": module_name,
        "iterations": iterations,
        "lambda_rate": lambda_rate,
        "time_limit": time_limit,
        "theoretical_p": round(theoretical_p, 4),
        "empirical_p": round(empirical_p, 4),
        "mttf": round(mttf, 2),
        "relative_error": round(relative_error, 2),
        "calc_time_ms": round(calc_time_ms, 2)
    }
