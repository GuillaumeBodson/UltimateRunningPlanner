using GarminRunerz.Workout.Services.Models;

namespace WebUI.Models.Workouts;

public class CustomWorkoutModel
{
    public RunType RunType { get; set; }

    public int TotalDuration { get; set; }

    public int Repetitions { get; set; }

    public double RunDuration { get; set; }

    public double CoolDownDuration { get; set; }

    public string Description { get; set; } = string.Empty;

}
