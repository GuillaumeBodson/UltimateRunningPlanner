using GarminRunerz.Workout.Services.Models;

namespace WebUI.Services.Dtos;

public class WorkoutDetailsDto
{
    public int Repetitions { get; set; }
    public int EffortDuration { get; set; }
    public int RecoveryDuration { get; set; }
    public PaceType PaceType { get; set; }
    public decimal Pace { get; set; }
    public WorkoutDetailsDto? Details { get; set; } = null;
    public List<WorkoutDetailsDto>? DetailsCollection { get; set; } = null;
}