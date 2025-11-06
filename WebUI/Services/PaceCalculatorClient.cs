using WebUI.Services.Dtos;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

internal sealed class PaceCalculatorClient(HttpClient http) : IPaceCalculatorClient
{
    // Preferred: requires API to expose POST /estimate accepting a request body.
    public async Task<double> EstimateAsync(double distanceMeters, IReadOnlyList<PerformanceDto> performances, CancellationToken ct = default)
    {
        // If your API keeps GET-only, you can forward to EstimateViaGetAsync here instead.
        var request = new
        {
            distanceMeters,
            performances
        };

        using var response = await http.PostAsJsonAsync("/estimate", request, ct).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<double>(cancellationToken: ct).ConfigureAwait(false);
        return result;
    }
}