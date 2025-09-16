using WebUI.Models;

namespace WebUI.Services;

public interface IAthleteSession
{
    Athlete? Current { get; }
    bool HasValue { get; }
    void Set(Athlete athlete);
    void Clear();
    Task<Athlete?> GetAndSetAsync();
    Task StoreAsync(Athlete? athlete);
    Task RemoveAsync();
}