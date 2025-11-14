using WebUI.Services.Dtos;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

internal sealed class PaceCalculatorClient(HttpClient http) : IPaceCalculatorClient
{
    public async Task<PerformancePredictionDto> EstimateAsync(double distanceMeters, IReadOnlyList<PerformanceDto> performances, CancellationToken ct = default)
    {
        var request = new
        {
            distanceMeters,
            performances
        };

        using var response = await http.PostAsJsonAsync("/estimate", request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PerformancePredictionDto?>(cancellationToken: ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to deserialize response to PerformancePredictionDto.");
        return result;
    }

    public async Task<PerformancePredictionDto> EstimateWithRParameterAsync(double distanceMeters, double rParameter, CancellationToken ct = default)
    {
        var request = new
        {
            distanceMeters,
            rParameter
        };
        using var response = await http.PostAsJsonAsync("/estimate-with-r", request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PerformancePredictionDto?>(cancellationToken: ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to deserialize response to PerformancePredictionDto.");
        return result;
    }

    public async Task<MultiplePerformancesPredictionDto> EstimateMultipleAsync(IReadOnlyList<int> distances, IReadOnlyList<PerformanceDto> performances, CancellationToken ct = default)
    {
        var request = new
        {
            distances,
            performances
        };
        using var response = await http.PostAsJsonAsync("/estimate-multiple", request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<MultiplePerformancesPredictionDto>(cancellationToken: ct).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to deserialize response to MultiplePerformancesPredictionDto.");

        return result;
    }
}