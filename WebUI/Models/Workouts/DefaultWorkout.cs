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

    protected override Color CalendarColor => Color.Secondary;
    protected override string EventLabel => "Other";
}