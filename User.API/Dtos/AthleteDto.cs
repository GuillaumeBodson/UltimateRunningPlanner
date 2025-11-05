using GarminRunerz.Workout.Services.Models;

namespace User.API.Dtos;

public class AthleteDto
{
    public int Id { get; set; }
    public decimal EasyPace { get; set; }
    public decimal MarathonPace { get; set; }
    public decimal SemiMarathonPace { get; set; }
    public decimal VmaPace { get; set; }
    public HashSet<Dictionary<DayOfWeek, RunType?>> TrainingTemplates { get; set; } = [];
}
