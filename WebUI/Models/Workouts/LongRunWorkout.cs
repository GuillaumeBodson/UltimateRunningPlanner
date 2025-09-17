using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class LongRunWorkout : PlannedWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (Repetitions == 0)
                return $"W{WeekNumber} Long run ({km}km)";

            return $"W{WeekNumber} Long run with {Repetitions}x{RunDuration}\" @ {FormatPace()}min/km ({km}km)";
        }
    }

    protected override Color CalendarColor => Color.Success;
    protected override string EventLabel => "Long Run";
}