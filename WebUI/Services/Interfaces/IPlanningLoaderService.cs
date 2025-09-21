using GarminRunerz.Workout.Services.Models;

namespace WebUI.Services.Interfaces;

public interface IPlanningLoaderService
{
    Task<List<CustomWorkout>> LoadPlanningFromFileAsync(string filePath);
    Task<List<CustomWorkout>> LoadPlanningAsync(Stream fileStream);
}