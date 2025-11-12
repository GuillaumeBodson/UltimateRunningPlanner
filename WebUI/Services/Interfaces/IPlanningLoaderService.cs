using GarminRunerz.Workout.Services.Models;
using WebUI.Mappers;
using WebUI.Models;

namespace WebUI.Services.Interfaces;

public interface IPlanningLoaderService
{
    Planning GetPlanning();
    void LoadPlanning(Planning planning);
    Task<List<CustomWorkout>> LoadPlanningFromFileAsync(string filePath);
    Task<List<CustomWorkout>> ReadCustomWorkoutsAsync(Stream fileStream);
    //Task<List<GarminWorkout>> LoadPlanningAsync2(Stream fileStream);
}