using GarminRunerz.Workout.Services.Models;

namespace User.API.Data.Models;

public class Athlete
{
    public int Id { get; set; }
    public decimal EasyPace { get; set; }
    public decimal MarathonPace { get; set; }
    public decimal SemiMarathonPace { get; set; }
    public decimal VmaPace { get; set; }
    public HashSet<Dictionary<DayOfWeek, RunType?>> TrainingTemplates { get; set; } = [];

    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
