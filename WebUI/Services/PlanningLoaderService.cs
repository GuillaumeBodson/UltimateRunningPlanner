using GarminRunerz.Workout.Services.Models;
using ToolBox.File;
using ToolBox.File.Core;
using WebUI.Mappers;
using WebUI.Models;
using WebUI.Services.Dtos;
using WebUI.Services.Interfaces;
using WebUI.Validators;

namespace WebUI.Services;

public sealed class PlanningLoaderService(ILogger<PlanningLoaderService> logger) : IPlanningLoaderService
{
    private readonly ILogger<PlanningLoaderService> _logger = logger;

    private Planning? _planning;

    public async Task<List<WorkoutDto>> ReadCustomWorkoutsAsync(Stream fileStream)
    {
        _logger.LogInformation("Loading csv information from stream ");        
        try
        {
            var validationResult = await CsvFileReader.ReadAndValidateCsvStreamAsync(fileStream, WorkoutMapper.FromCsvLine, new WorkoutDtoValidator(), allowQuotedFields:true);

            var result = ProcessValidationResult(validationResult);

            return result.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from stream");
            throw;
        }
    }

    public async Task<List<WorkoutDto>> LoadPlanningFromFileAsync(string filePath)
    {
        _logger.LogInformation("Loading planning from CSV: {FilePath}", filePath);
        try
        {
            var validationResult = await CsvFileReader.ReadAndValidateCsv(filePath, WorkoutMapper.FromCsvLine, new WorkoutDtoValidator());

            var result = ProcessValidationResult(validationResult);

            return result.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from {FilePath}", filePath);
            throw;
        }
    }

    private CsvValidationSummary<WorkoutDto> ProcessValidationResult(Result<CsvValidationSummary<WorkoutDto>> results)
    {
        if (results.Value is null)
        {
            throw new InvalidOperationException("No valid planning data found in the provided stream.");
        }

        if (results.IsSuccess is false)
        {
            throw results.Exception!;
        }
        _logger.LogInformation("Loaded {Count} workouts from planning file.", results.Value.ValidCount);

        if (results.Value.InvalidCount > 0)
        {
            _logger.LogWarning("Planning contains {InvalidCount} invalid workouts.", results.Value.InvalidCount);
        }

        return results.Value;
    }
}