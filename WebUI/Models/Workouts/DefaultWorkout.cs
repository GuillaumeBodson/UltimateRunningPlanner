using MudBlazor;

namespace WebUI.Models.Workouts;

public sealed class DefaultWorkout : PlannedWorkout
{
    public override string Name
    {
        get
        {
            double km = EstimatedDistance / 1000.0;           
            return $"W{WeekNumber} Run ({km}km)";
        }
    }

    public override Color CalendarColor => Color.Secondary;
    public override string EventLabel => "Other";
}