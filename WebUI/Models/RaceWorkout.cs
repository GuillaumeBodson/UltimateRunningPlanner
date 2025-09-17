using MudBlazor;

namespace WebUI.Models;

public sealed class RaceWorkout : PlannedWorkout
{
    public override string Name => "Race day!";

    protected override Color CalendarColor => Color.Error;
    protected override string EventLabel => "Race";
}