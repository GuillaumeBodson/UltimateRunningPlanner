using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Services.Dtos;

namespace WebUI.Services.Interfaces;
public interface IPlanningBuilder
{
    Planning BuildPlanning(DateOnly startDate, List<WorkoutDto> workouts, Athlete athlete, List<TrainingTemplate>? trainingTemplates = null);
}