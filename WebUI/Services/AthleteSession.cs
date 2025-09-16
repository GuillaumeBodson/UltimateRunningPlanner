using Blazored.LocalStorage;
using WebUI.Models;

namespace WebUI.Services;

public sealed class AthleteSession : IAthleteSession
{
    private Athlete? _athlete;
    private readonly ILocalStorageService _localStorageService;

    public Athlete? Current => _athlete;
    public bool HasValue => _athlete is not null;

    public AthleteSession(ILocalStorageService localStorageService)
    {
        _localStorageService = localStorageService;
    }

    public void Set(Athlete? athlete) => _athlete = athlete ?? throw new ArgumentNullException(nameof(athlete));
    public void Clear() => _athlete = null;
    public async Task<Athlete?> GetAndSetAsync()
    {
        if (HasValue)
        {
            return _athlete;
        }

        var athlete = await _localStorageService.GetItemAsync<Athlete>(Constants.AthleteKey);
        if (athlete is null)
        {
            return null;
        }
        Set(athlete);
        return athlete;
    }

    public async Task StoreAsync(Athlete? athlete)
    {
        Set(athlete);
        if (!HasValue)
        {
            throw new InvalidOperationException("No athlete in session to store.");
        }
        await _localStorageService.SetItemAsync(Constants.AthleteKey, _athlete);
    }

    public async Task RemoveAsync()
    {
        Clear();
        await _localStorageService.RemoveItemAsync(Constants.AthleteKey);
    }
}