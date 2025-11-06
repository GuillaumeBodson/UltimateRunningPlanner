using WebUI.Services.Dtos;

namespace WebUI.Services.Interfaces;

public interface IPaceCalculatorClient
{
    Task<double> EstimateAsync(double distanceMeters, IReadOnlyList<PerformanceDto> performances, CancellationToken ct = default);
}