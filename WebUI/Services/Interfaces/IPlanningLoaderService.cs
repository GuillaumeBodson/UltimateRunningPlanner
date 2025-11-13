using WebUI.Models;
using WebUI.Services.Dtos;

namespace WebUI.Services.Interfaces;

public interface IPlanningLoaderService
{
    Task<List<WorkoutDto>> LoadPlanningFromFileAsync(string filePath);
    Task<List<WorkoutDto>> ReadCustomWorkoutsAsync(Stream fileStream);
}