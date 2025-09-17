using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class EasyWorkout : PlannedWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (Repetitions == 0)
                return $"W{WeekNumber} Easy run ({km}km)";

            return $"W{WeekNumber} Easy run with {RunDuration}\" @ {FormatPace()}min/km";
        }
    }

    protected override Color CalendarColor => Color.Info;
    protected override string EventLabel => "Easy";
}