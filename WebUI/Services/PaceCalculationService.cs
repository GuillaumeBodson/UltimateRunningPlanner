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

    public async Task<Pace> CalculatePaceAsync(double distance, params Performance[] performances)
    {
        var result = await _paceCalculatorClient.EstimateAsync(distance, [.. performances.Select(x => x.ToDto())]);
        var pace = new Pace((int)Math.Round(result));
        return pace;
    }

    public async Task<Dictionary<int, Pace>> CalculatePacesForStandardDistancesAsync(params Performance[] performances)
    {
        var paces = new Dictionary<int, Pace>();
        var missingDistances = distances.Where(d => !performances.Select(p => p.DistanceMeters).Contains(d)).ToList();
        var apiCalls = new Task<Pace>[missingDistances.Count];
        int j = 0;
        for (int i = 0; i < distances.Length; i++)
        {
            int distance = distances[i];
            if (performances.FirstOrDefault(p => p.DistanceMeters == distance) is Performance performance && performance is not null)
            {
                var secondsPerKm = performance.TimeSeconds / ((double)performance.DistanceMeters / 1000);
                paces[distance] = new Pace((int)Math.Round(secondsPerKm));
            }
            else
            {
                apiCalls[j] = CalculatePaceAsync(distance, performances);
                j++;
            }
        }

        var results = await Task.WhenAll(apiCalls);
        for (int i = 0; i < missingDistances.Count; i++)
        {
            int distance = missingDistances[i];
            paces[distance] = results[i];
        }
        return paces;
    }
}
