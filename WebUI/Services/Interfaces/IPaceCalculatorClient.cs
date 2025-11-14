using WebUI.Services.Dtos;

namespace WebUI.Services.Interfaces;

public interface IPaceCalculatorClient
{
    Task<PerformancePredictionDto> EstimateAsync(double distanceMeters, IReadOnlyList<PerformanceDto> performances, CancellationToken ct = default);
    Task<MultiplePerformancesPredictionDto> EstimateMultipleAsync(List<int> distancesMeters, IReadOnlyList<PerformanceDto> performances, CancellationToken ct = default);
    Task<PerformancePredictionDto> EstimateWithRParameterAsync(double distanceMeters, double rParameter, CancellationToken ct = default);
}