using WebUI.Models;

namespace WebUI.Services.Interfaces;

public interface IPaceCalculationService
{
    Task<PerformancePrediction> CalculatePaceAsync(double distance, params Performance[] performances);
    Task<MultiplePerformancesPrediction> CalculatePacesForStandardDistancesAsync(params Performance[] performances);
}