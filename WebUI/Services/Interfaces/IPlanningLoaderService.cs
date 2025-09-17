using GarminRunerz.Workout.Services.Models;

namespace WebUI.Services.Interfaces;

public interface IPlanningLoaderService
{
    List<CustomWorkout> LoadPlanning(string filePath);
    Task<List<CustomWorkout>> LoadPlanningAsync(Stream fileStream);
}