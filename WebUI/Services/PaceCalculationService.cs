using WebUI.Mappers;
using WebUI.Models;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public class PaceCalculationService : IPaceCalculationService
{
    private readonly IPaceCalculatorClient _paceCalculatorClient;
    private static readonly int[] distances = [2_000, 5_000, 10_000, 21_097, 42_195, 500_000];

    public PaceCalculationService(IPaceCalculatorClient paceCalculatorClient)
    {
        _paceCalculatorClient = paceCalculatorClient;
    }

    public async Task<PerformancePrediction> CalculatePaceAsync(double distance, params Performance[] performances)
    {
        var result = await _paceCalculatorClient.EstimateAsync(distance, [.. performances.Select(x => x.ToDto())]);

        var prediction = result.ToPerformancePrediction();
        return prediction;
    }
    public async Task<MultiplePerformancesPrediction> CalculatePacesForStandardDistancesAsync(params Performance[] performances)
    {
        var missingDistances = distances.Where(d => !performances.Select(p => p.DistanceMeters).Contains(d)).ToList();
        var result = await _paceCalculatorClient.EstimateMultipleAsync([.. missingDistances], [.. performances.Select(x => x.ToDto())]);

        var predictions = result.ToMultiplePerformancesPrediction();

        return predictions;
    }
}
