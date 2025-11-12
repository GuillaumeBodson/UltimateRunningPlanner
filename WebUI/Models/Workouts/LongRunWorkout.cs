using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class LongRunWorkout : StructuredWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;
            if (IsEmpty)
                return $"W{WeekNumber} Long run ({km}km)";

            return $"W{WeekNumber} Long run with {StringifyDetails()}";
        }
    }

    protected override Color CalendarColor => Color.Success;
    protected override string EventLabel => "Long Run";
}