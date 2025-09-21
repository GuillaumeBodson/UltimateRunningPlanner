using GarminRunerz.Workout.Services.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using ToolBox.File;
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
            var planning = await CsvFileReader.ReadAndValidateCsvStreamAsync(fileStream, line =>
            {
                return new CustomWorkout
                {
                    WeekNumber = int.Parse(line[0]),
                    RunType = Enum.Parse<RunType>(line[1], true),
                    TotalDuration = int.Parse(line[2]),
                    Repetitions = int.Parse(line[3]),
                    RunDuration = double.Parse(line[4]),
                    CoolDownDuration = double.Parse(line[5]),
                    Pace = decimal.Parse(line[6], CultureInfo.InvariantCulture),
                    Speed = decimal.Parse(line[7], CultureInfo.InvariantCulture),
                    Description = line[8]
                };
            }, new CustomWokoutValidator());

            if(planning.IsSuccess is false)
            {
                throw planning.Exception!;
            }
            _logger.LogInformation("Loaded {Count} workouts from planning file.", planning.Value!.ValidCount);

            if(planning.Value.InvalidCount > 0)
            {
                _logger.LogWarning("Planning contains {InvalidCount} invalid workouts.", planning.Value.InvalidCount);
            }
            return planning.Value.ValidEntities.ToList();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from stream", fileStream);
            throw;
        }
    }

    public List<CustomWorkout> LoadPlanning(string filePath)
    {
        _logger.LogInformation("Loading planning from CSV: {FilePath}", filePath);
        try
        {
            var planning = CsvFileReader.ReadCsvFile(filePath, line =>
            {
                return new CustomWorkout
                {
                    WeekNumber = int.Parse(line[0]),
                    RunType = Enum.Parse<RunType>(line[1], true),
                    TotalDuration = int.Parse(line[2]),
                    Repetitions = int.Parse(line[3]),
                    RunDuration = double.Parse(line[4]),
                    CoolDownDuration = double.Parse(line[5]),
                    Pace = decimal.Parse(line[6], CultureInfo.InvariantCulture),
                    Speed = decimal.Parse(line[7], CultureInfo.InvariantCulture),
                    Description = line[8]
                };
            });

            _logger.LogInformation("Loaded {Count} workouts from planning file.", planning.Count);
            return planning;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load planning from {FilePath}", filePath);
            throw;
        }
    }
}