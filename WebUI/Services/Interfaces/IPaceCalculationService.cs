using WebUI.Models;

namespace WebUI.Services.Interfaces;

public interface IPaceCalculationService
{
    Task<Pace> CalculatePaceAsync(double distance, params Performance[] performances);
    Task<Dictionary<int, Pace>> CalculatePacesForStandardDistancesAsync(params Performance[] performances);
}