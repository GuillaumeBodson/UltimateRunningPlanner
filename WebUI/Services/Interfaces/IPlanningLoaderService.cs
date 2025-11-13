using WebUI.Models;
using WebUI.Services.Dtos;

namespace WebUI.Services.Interfaces;

public interface IPlanningLoaderService
{
    Planning GetPlanning();
    void LoadPlanning(Planning planning);
    Task<List<WorkoutDto>> LoadPlanningFromFileAsync(string filePath);
    Task<List<WorkoutDto>> ReadCustomWorkoutsAsync(Stream fileStream);
}