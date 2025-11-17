using WebUI.Models.Workouts;

namespace WebUI.Services.Interfaces;

public interface IWorkoutTemplateSelector
{
    Type Select(PlannedWorkout workout);
}