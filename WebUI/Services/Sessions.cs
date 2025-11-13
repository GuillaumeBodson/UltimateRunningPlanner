using Blazored.LocalStorage;
using WebUI.Models;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

public sealed class AthleteSession : Session<Athlete>, ISession<Athlete>
{
    protected override string Key => Constants.AthleteKey;

    public AthleteSession(ILocalStorageService localStorageService) : base(localStorageService)
    {
    }
}

public sealed class PlanningSession : Session<Planning>, ISession<Planning>
{
    protected override string Key => Constants.PlanningKey;
    public PlanningSession(ILocalStorageService localStorageService) : base(localStorageService)
    {
    }
}