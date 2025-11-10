using FluentValidation;
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

    public static GarminWorkout ToGarminWorkout(string[] line)
    {
        // a) n * (t, p, r)
        GarminWorkoutDetails ExtractSimpleDetails(string detailsString)
        {
            var repetitionString = detailsString.Split(" * ");
            var durationString = repetitionString[1].TrimStart('(').Split(", ");
            return new GarminWorkoutDetails
            {
                Repetitions = int.Parse(repetitionString[0].Trim()),
                EffortDuration = int.Parse(durationString[0]),
                Pace = Enum.Parse<PaceType>(durationString[1], true),
                RecoveryDuration = int.Parse(durationString[2])
            };
        }

        GarminWorkout garminWorkout = new GarminWorkout
        {
            WeekNumber = int.Parse(line[0]),
            RunType = Enum.Parse<RunType>(line[1], true),
            TotalDuration = int.Parse(line[2]),
            Description = line[4],
        };

        /*
         the possible workout's format of details:
	        a) n * (t, p, r)	
	        b) n1 * (t1, p1, r1), n2 * (t2, p2, r2), nx * (tx, px, rx)
	        c) n1 * (n2 * (t, p, r2), r1)

         */
        if (line.Length != 5)
        {
            throw new ArgumentException($"Invalid CSV line. Expected 5 fields but got {line.Length}.");
        }
        GarminWorkoutDetails? details = null;
        if (!string.IsNullOrWhiteSpace(line[3]))
        {
            var deatilsString = line[3];
            var detailsSplit = deatilsString.Split("), ");

            // a) n * (t, p, r)
            if (detailsSplit.Length == 1)
            {
                details = ExtractSimpleDetails(detailsSplit[0].TrimEnd(')'));
            }
            //c) n1 * (n2 * (t, p, r2), r1)
            else if (detailsSplit[0].Count('*') == 2)
            {
                var repetitionString = detailsSplit[0].Split(" * ");

                var innerDurationString = repetitionString[2].Split(", ");
                var garminDetailsInner = new GarminWorkoutDetails
                {
                    Repetitions = int.Parse(repetitionString[1].TrimStart('(').Trim()),
                    EffortDuration = int.Parse(innerDurationString[0].TrimStart('(')),
                    Pace = Enum.Parse<PaceType>(repetitionString[1], true),
                    RecoveryDuration = int.Parse(repetitionString[2])
                };

                details = new GarminWorkoutDetails
                {
                    Repetitions = int.Parse(repetitionString[0].Trim()),
                    Details = garminDetailsInner,
                    RecoveryDuration = int.Parse(detailsSplit[1])
                };
            }
            else
            {
                garminWorkout.DetailsCollection ??= [];

                foreach (var detailS in detailsSplit)
                {
                    garminWorkout.DetailsCollection.Add(ExtractSimpleDetails(detailS.TrimEnd(')')));
                }
            }           
        }
        garminWorkout.Details = details;

        return garminWorkout;
    }
}
// temporary models that will be migrated to GarminRunerz.Workout.Services.Models later
public class GarminWorkout
{
    public int Id { get; set; }

    public int WeekNumber { get; set; }

    public RunType RunType { get; set; }

    public int TotalDuration { get; set; }

    public string Description { get; set; } = string.Empty;

    public GarminWorkoutDetails? Details { get; set; } = null;
    public List<GarminWorkoutDetails>? DetailsCollection { get; set; } = null;
}

public class GarminWorkoutDetails
{
    public int Repetitions { get; set; }
    public int EffortDuration { get; set; }
    public int RecoveryDuration { get; set; }
    public PaceType Pace { get; set; }
    public GarminWorkoutDetails? Details { get; set; } = null;
}

public enum PaceType
{
    MasPace,
    FiveKPace,
    TenKPace,
    SemiMarathonPace,
    MarathonPace,
    EasyPace
}

public class GarminWorkoutValidator: AbstractValidator<GarminWorkout>
{
    public GarminWorkoutValidator()
    {
        RuleFor(x => x.WeekNumber).GreaterThan(0).WithMessage("Week number must be greater than 0.");
        RuleFor(x => x.RunType).IsInEnum().WithMessage("Run type is not valid.");
        RuleFor(x => x.TotalDuration).GreaterThan(0).WithMessage("Total duration must be greater than 0.");
        RuleFor(x => x.Description).NotEmpty().WithMessage("Description must not be empty.");
    }
}