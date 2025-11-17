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

    public override Color CalendarColor => Color.Info;
    public override string EventLabel => "Steady";
}