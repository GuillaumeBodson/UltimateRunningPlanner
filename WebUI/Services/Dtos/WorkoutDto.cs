using GarminRunerz.Workout.Services.Models;

namespace WebUI.Services.Dtos;

public class WorkoutDto
{
    public int Id { get; set; }

    public int WeekNumber { get; set; }

    public RunType RunType { get; set; }

    public int TotalDuration { get; set; }

    public string Description { get; set; } = string.Empty;

    public bool HasDetails => DetailsCollection != null && DetailsCollection.Count > 0;

    public List<WorkoutDetailsDto>? DetailsCollection { get; set; } = null;
}
