namespace CarpoolingApp;

/// <summary>
/// Математический модуль вычислительного эксперимента (Версия 2.0).
/// Моделирует себестоимость поездки, деление расходов на 1..4 попутчиков и годовую экономию.
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
    /// Расчет параметров распределения затрат при заданном числе попутчиков и поездках в год.
    /// </summary>
    public static ExperimentResult CalculateCostSharing(
        double distanceKm,
        double fuelConsumption,
        double fuelPrice,
        double depreciationPerKm,
        int passengersCount,
        int tripsPerYear = 40)
    {
        if (passengersCount < 1 || passengersCount > 6)
            throw new ArgumentException("Количество попутчиков должно быть от 1 до 6.");
        if (tripsPerYear < 1)
            throw new ArgumentException("Количество поездок в год должно быть >= 1.");

        double fuelCost = CalculateFuelCost(distanceKm, fuelConsumption, fuelPrice);
        double deprecCost = CalculateDepreciationCost(distanceKm, depreciationPerKm);
        double totalCost = Math.Round(fuelCost + deprecCost, 2);
        double costPerKm = Math.Round(totalCost / distanceKm, 2);

        int totalPersons = passengersCount + 1;
        double costPerPerson = Math.Round(totalCost / totalPersons, 2);
        double driverSavings = Math.Round(totalCost - costPerPerson, 2);
        double coveragePercent = Math.Round((driverSavings / totalCost) * 100.0, 1);
        double annualSavings = Math.Round(driverSavings * tripsPerYear, 2);

        return new ExperimentResult
        {
            PassengersCount = passengersCount,
            TotalPersons = totalPersons,
            FuelCost = fuelCost,
            DepreciationCost = deprecCost,
            TotalCost = totalCost,
            CostPerKm = costPerKm,
            CostPerPerson = costPerPerson,
            DriverSavings = driverSavings,
            CoveragePercent = coveragePercent,
            AnnualSavings = annualSavings
        };
    }

    /// <summary>
    /// Проведение серии вычислительных экспериментов для 1..4 попутчиков.
    /// </summary>
    public static List<ExperimentResult> RunExperimentGrid(
        double distanceKm,
        double fuelConsumption = 8.5,
        double fuelPrice = 58.5,
        double depreciationPerKm = 1.8,
        int tripsPerYear = 40)
    {
        var results = new List<ExperimentResult>();
        for (int n = 1; n <= 4; n++)
        {
            results.Add(CalculateCostSharing(distanceKm, fuelConsumption, fuelPrice, depreciationPerKm, n, tripsPerYear));
        }
        return results;
    }
}
