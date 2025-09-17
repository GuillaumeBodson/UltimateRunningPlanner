using MudBlazor;

namespace WebUI.Models;

public sealed class DefaultWorkout : PlannedWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (Repetitions == 0)
                return $"W{WeekNumber} Run ({km}km)";

            return $"W{WeekNumber} Run with {RunDuration}\" @ {FormatPace()}min/km";
        }
    }

    protected override Color CalendarColor => Color.Secondary;
    protected override string EventLabel => "Other";
}