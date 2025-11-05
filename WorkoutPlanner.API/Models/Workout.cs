using GarminRunerz.Workout.Services.Models;

namespace WorkoutPlanner.API.Models;

public class Workout
{
    public int Id { get; set; }
    public int ComputedId { get; set; }
    public int WeekNumber { get; set; }
    public RunType RunType { get; set; }
    public int TotalDuration { get; set; }
    public string Description { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public int EstimatedDistance { get; set; }
    public int EstimatedTime { get; set; }
    public decimal Pace { get; set; }
    public string Name { get; set; } = string.Empty;

    public int PlanningId { get; set; }
    public Planning Planning { get; set; } = null!;
}
