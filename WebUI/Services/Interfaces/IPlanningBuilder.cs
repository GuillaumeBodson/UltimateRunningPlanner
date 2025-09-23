using GarminRunerz.Workout.Services.Models;
using WebUI.Models;

namespace WebUI.Services.Interfaces;
public interface IPlanningBuilder
{
    Planning BuildPlanning(DateOnly startDate, List<CustomWorkout> workouts, Athlete athlete, List<TrainingTemplate>? trainingTemplates = null);
}