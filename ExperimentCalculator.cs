namespace CarpoolingApp;

/// <summary>
/// Математический модуль вычислительного эксперимента для сервиса поиска попутчиков (Версия 1.0).
/// Реализует расчет топливных, амортизационных затрат и справедливого деления расходов.
/// </summary>
public static class ExperimentCalculator
{
    /// <summary>
    /// Расчет прямых затрат на топливо: C_fuel = (D * C_cons / 100) * P_fuel.
    /// </summary>
    public static double CalculateFuelCost(double distanceKm, double fuelConsumption, double fuelPrice)
    {
        if (distanceKm <= 0 || fuelConsumption <= 0 || fuelPrice <= 0)
            throw new ArgumentException("Расстояние, расход и цена топлива должны быть строго больше 0.");

        return Math.Round((distanceKm * fuelConsumption / 100.0) * fuelPrice, 2);
    }

    /// <summary>
    /// Расчет амортизационных расходов: C_deprec = D * k_deprec.
    /// </summary>
    public static double CalculateDepreciationCost(double distanceKm, double depreciationPerKm)
    {
        if (distanceKm <= 0 || depreciationPerKm < 0)
            throw new ArgumentException("Расстояние должно быть > 0, норма амортизации >= 0.");

        return Math.Round(distanceKm * depreciationPerKm, 2);
    }

    /// <summary>
    /// Расчет полной себестоимости поездки: C_total = C_fuel + C_deprec.
    /// </summary>
    public static double CalculateTotalTripCost(
        double distanceKm,
        double fuelConsumption,
        double fuelPrice,
        double depreciationPerKm = 1.8)
    {
        double fuelCost = CalculateFuelCost(distanceKm, fuelConsumption, fuelPrice);
        double deprecCost = CalculateDepreciationCost(distanceKm, depreciationPerKm);
        return Math.Round(fuelCost + deprecCost, 2);
    }

    /// <summary>
    /// Моделирование распределения затрат между водителем и N попутчиками.
    /// </summary>
    public static ExperimentResult CalculateCostSharing(
        double distanceKm,
        double fuelConsumption,
        double fuelPrice,
        double depreciationPerKm,
        int passengersCount)
    {
        if (passengersCount < 1 || passengersCount > 6)
            throw new ArgumentException("Количество попутчиков должно быть от 1 до 6.");

        double fuelCost = CalculateFuelCost(distanceKm, fuelConsumption, fuelPrice);
        double deprecCost = CalculateDepreciationCost(distanceKm, depreciationPerKm);
        double totalCost = Math.Round(fuelCost + deprecCost, 2);

        int totalPersons = passengersCount + 1;
        double costPerPerson = Math.Round(totalCost / totalPersons, 2);
        double driverSavings = Math.Round(totalCost - costPerPerson, 2);
        double coveragePercent = Math.Round((driverSavings / totalCost) * 100.0, 1);

        return new ExperimentResult
        {
            PassengersCount = passengersCount,
            TotalPersons = totalPersons,
            FuelCost = fuelCost,
            DepreciationCost = deprecCost,
            TotalCost = totalCost,
            CostPerPerson = costPerPerson,
            DriverSavings = driverSavings,
            CoveragePercent = coveragePercent
        };
    }
}
