using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class SteadyWorkout : StructuredWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (IsEmpty)
                return $"W{WeekNumber} Steady run ({km}km)";

            return $"W{WeekNumber} Steady run with {StringifyDetails()}";
        }
    }

    protected override Color CalendarColor => Color.Info;
    protected override string EventLabel => "Steady";
}