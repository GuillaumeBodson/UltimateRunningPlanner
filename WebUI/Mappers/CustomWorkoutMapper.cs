using GarminRunerz.Workout.Services.Models;
using System.Globalization;
using WebUI.Models;
using WebUI.Models.Workouts;

namespace WebUI.Mappers;

public static class CustomWorkoutMapper
{
    public static CustomWorkout FromCsvLine(string[] line)
    {
        if (line.Length != 9)
        {
            throw new ArgumentException($"Invalid CSV line. Expected 9 fields but got {line.Length}.");
        }
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
    }

    public static CustomWorkout ToCustomWorkout(this CustomWorkoutModel model, int weekNumber, Athlete athlete, int id)
    {
        var pace = model.RunType switch
        {
            RunType.Easy => athlete.EasyPace,
            RunType.LongRun => athlete.MarathonPace,
            RunType.Tempo => athlete.SemiMarathonPace,
            RunType.Intervals => athlete.MasPace,
            RunType.Recovery => athlete.EasyPace,
            RunType.Steady => athlete.SemiMarathonPace,
            _ => athlete.EasyPace
        };
        return new CustomWorkout
        {
            WeekNumber = weekNumber,
            RunType = model.RunType,
            TotalDuration = model.TotalDuration,
            Repetitions = model.Repetitions,
            RunDuration = model.RunDuration,
            CoolDownDuration = model.CoolDownDuration,
            Description = model.Description,
            Pace = (decimal)pace.ToMinutesDouble(),
            Id = id
        };
    }

    public static CustomWorkout ToCustomWorkout(this PlannedWorkout w)
    {
        var result = new CustomWorkout
        {
            Id = w.Id,
            WeekNumber = w.WeekNumber,
            RunType = w.RunType,
            TotalDuration = w.TotalDuration,
            Description = w.Description,
            // Map Pace as minutes-per-km decimal; Speed as km/h
            Pace = (decimal)w.Pace.ToMinutesDouble(),
            Speed = (decimal)w.Pace.ToKmPerHour(),
        };

        if (w is StructuredWorkout s)
        {
            result.Repetitions = s.Repetitions;
            // Units: your domain uses seconds for durations; keep seconds here.
            result.RunDuration = s.IntervalDuration;
            result.CoolDownDuration = s.RecoveryDuration;
        }
        else
        {
            // Non-structured continuous run: single block
            result.Repetitions = 0;
            result.RunDuration = w.TotalDuration;
            result.CoolDownDuration = 0;
        }

        return result;
    }

    public static List<CustomWorkout> ToCustomWorkouts(this IEnumerable<PlannedWorkout> workouts)
        => workouts.Select(ToCustomWorkout).ToList();
}
