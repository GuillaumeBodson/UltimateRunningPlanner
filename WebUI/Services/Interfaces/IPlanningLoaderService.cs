using GarminRunerz.Workout.Services.Models;
using WebUI.Mappers;

namespace WebUI.Services.Interfaces;

public interface IPlanningLoaderService
{
    Task<List<CustomWorkout>> LoadPlanningFromFileAsync(string filePath);
    Task<List<CustomWorkout>> LoadPlanningAsync(Stream fileStream);
    Task<List<GarminWorkout>> LoadPlanningAsync2(Stream fileStream);
}