using GarminRunerz.Workout.Services.Models;
using ToolBox.File;
using ToolBox.File.Core;
using WebUI.Mappers;
using WebUI.Services.Interfaces;
using WebUI.Validators;

namespace WebUI.Services;

public sealed class PlanningLoaderService : IPlanningLoaderService
{
    private readonly ILogger<PlanningLoaderService> _logger;

    public PlanningLoaderService(ILogger<PlanningLoaderService> logger)
    {
        _logger = logger;
    }

    public async Task<List<CustomWorkout>> LoadPlanningAsync(Stream fileStream)
    {
        _logger.LogInformation("Loading csv information from stream ");        
        try
        {
            var planning = await CsvFileReader.ReadAndValidateCsvStreamAsync(fileStream, CustomWorkoutMapper.FromCsvLine, new CustomWorkoutValidator());

            LogResult(planning);

            return planning.Value.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from stream");
            throw;
        }
    }

    public async Task<List<CustomWorkout>> LoadPlanningFromFileAsync(string filePath)
    {
        _logger.LogInformation("Loading planning from CSV: {FilePath}", filePath);
        try
        {
            var planning = await CsvFileReader.ReadAndValidateCsv(filePath, CustomWorkoutMapper.FromCsvLine, new CustomWorkoutValidator());

            LogResult(planning);

            return planning.Value.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from {FilePath}", filePath);
            throw;
        }
    }

    private void LogResult(Result<CsvValidationSummary<CustomWorkout>> results)
    {
        if (results.IsSuccess is false)
        {
            throw results.Exception!;
        }
        _logger.LogInformation("Loaded {Count} workouts from planning file.", results.Value!.ValidCount);

        if (results.Value.InvalidCount > 0)
        {
            _logger.LogWarning("Planning contains {InvalidCount} invalid workouts.", results.Value.InvalidCount);
        }
    }
}