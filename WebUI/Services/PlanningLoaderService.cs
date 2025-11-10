using GarminRunerz.Workout.Services.Models;
using ToolBox.File;
using ToolBox.File.Core;
using WebUI.Mappers;
using WebUI.Services.Interfaces;
using WebUI.Validators;

namespace WebUI.Services;

public sealed class PlanningLoaderService(ILogger<PlanningLoaderService> logger) : IPlanningLoaderService
{
    private readonly ILogger<PlanningLoaderService> _logger = logger;

    public async Task<List<CustomWorkout>> LoadPlanningAsync(Stream fileStream)
    {
        _logger.LogInformation("Loading csv information from stream ");        
        try
        {
            var validationResult = await CsvFileReader.ReadAndValidateCsvStreamAsync(fileStream, CustomWorkoutMapper.FromCsvLine, new CustomWorkoutValidator());

            var test = await CsvFileReader.ReadAndValidateCsvStreamAsync(fileStream, CustomWorkoutMapper.ToGarminWorkout, new GarminWorkoutValidator());

            var result = ProcessValidationResult(validationResult);

            return result.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from stream");
            throw;
        }
    }

    public async Task<List<GarminWorkout>> LoadPlanningAsync2(Stream fileStream)
    {
        _logger.LogInformation("Loading csv information from stream ");
        try
        {
            var validationResult = await CsvFileReader.ReadAndValidateCsvStreamAsync(fileStream, CustomWorkoutMapper.ToGarminWorkout, new GarminWorkoutValidator(), allowQuotedFields:true);

            if(validationResult.IsFailure)
            {
                _logger.LogError("Failed to validate Garmin workouts from stream: {Error}", validationResult.Exception?.Message);
                throw validationResult.Exception!;
            }


            return validationResult.Value.ValidEntities.ToList();
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
            var validationResult = await CsvFileReader.ReadAndValidateCsv(filePath, CustomWorkoutMapper.FromCsvLine, new CustomWorkoutValidator());

            var result = ProcessValidationResult(validationResult);

            return result.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from {FilePath}", filePath);
            throw;
        }
    }

    private CsvValidationSummary<CustomWorkout> ProcessValidationResult(Result<CsvValidationSummary<CustomWorkout>> results)
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