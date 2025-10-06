using GarminRunerz.Workout.Services.Models;

namespace WorkoutPlanner.API.Models;

public class Planning
{
    public int Id { get; set; }
    public DateTime StartDate { get; set; }
    public List<Workout> Workouts { get; set; } = [];
    public int AthleteId { get; set; }
    public HashSet<Dictionary<DayOfWeek, RunType?>> TrainingTemplates { get; set; } = [];
}
