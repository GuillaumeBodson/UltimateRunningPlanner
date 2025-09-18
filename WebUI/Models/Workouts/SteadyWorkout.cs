using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class SteadyWorkout : StructuredWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (Repetitions == 0)
                return $"W{WeekNumber} Steady run ({km}km)";

            return $"W{WeekNumber} Steady run with {IntervalDuration}\" @ {FormatPace()}min/km";
        }
    }

    protected override Color CalendarColor => Color.Info;
    protected override string EventLabel => "Steady";
}