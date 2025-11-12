using GarminRunerz.Workout.Services.Models;
using WebUI.Models;
using WebUI.Models.Workouts;
using WebUI.Services.Dtos;

namespace WebUI.Mappers;

public static class WorkoutMapper
{
    public static WorkoutDto ToWorkoutDto(this CustomWorkoutModel model, int weekNumber, Athlete athlete, int id)
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

        var details = model.Repetitions > 0 ? new WorkoutDetailsDto
        {
            Repetitions = model.Repetitions,
            EffortDuration = (int)model.RunDuration,
            RecoveryDuration = (int)model.CoolDownDuration,
            Pace = pace.ToMeterPerSeconds(),
            PaceType = model.RunType switch
            {
                RunType.Easy => PaceType.EasyPace,
                RunType.LongRun => PaceType.MarathonPace,
                RunType.Tempo => PaceType.SemiMarathonPace,
                RunType.Intervals => PaceType.MasPace,
                RunType.Recovery => PaceType.EasyPace,
                RunType.Steady => PaceType.SemiMarathonPace,
                _ => PaceType.EasyPace
            }
        } : null;
        return new WorkoutDto
        {
            WeekNumber = weekNumber,
            RunType = model.RunType,
            TotalDuration = model.TotalDuration,
            DetailsCollection = details != null ? [details] : null,
            Description = model.Description,
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
            Description = w.Description
        };

        if (w is StructuredWorkout s)
        {
            result.DetailsCollection = s.DetailsCollection?.Select(d => new CustomWorkoutDetails
            {
                Repetitions = d.Repetitions,
                EffortDuration = d.EffortDuration,
                RecoveryDuration = d.RecoveryDuration,
                PaceType = d.PaceType,
                Pace = d.Pace.ToMeterPerSeconds(),
                Details = d.Details != null ? new CustomWorkoutDetails
                {
                    Repetitions = d.Details.Repetitions,
                    EffortDuration = d.Details.EffortDuration,
                    RecoveryDuration = d.Details.RecoveryDuration,
                    PaceType = d.Details.PaceType,
                    Pace = d.Details.Pace.ToMeterPerSeconds()
                } : null
            }).ToList();    
        }

        return result;
    }

    public static List<CustomWorkout> ToCustomWorkouts(this IEnumerable<PlannedWorkout> workouts)
        => workouts.Select(ToCustomWorkout).ToList();

    public static WorkoutDto FromCsvLine(string[] line)
    {
        // a) n * (t, p, r)
        WorkoutDetailsDto ExtractSimpleDetails(string detailsString)
        {
            var repetitionString = detailsString.Split(" * ");
            var durationString = repetitionString[1].TrimStart('(').Split(", ");
            return new WorkoutDetailsDto
            {
                Repetitions = int.Parse(repetitionString[0].Trim()),
                EffortDuration = int.Parse(durationString[0]),
                PaceType = Enum.Parse<PaceType>(durationString[1], true),
                RecoveryDuration = int.Parse(durationString[2])
            };
        }

        WorkoutDto garminWorkout = new WorkoutDto
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
        WorkoutDetailsDto? details = null;
        if (!string.IsNullOrWhiteSpace(line[3]))
        {
            var detailsString = line[3];
            var detailsSplit = detailsString.Split("), ");

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
                var garminDetailsInner = new WorkoutDetailsDto
                {
                    Repetitions = int.Parse(repetitionString[1].TrimStart('(').Trim()),
                    EffortDuration = int.Parse(innerDurationString[0].TrimStart('(')),
                    PaceType = Enum.Parse<PaceType>(innerDurationString[1], true),
                    RecoveryDuration = int.Parse(innerDurationString[2])
                };

                details = new WorkoutDetailsDto
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
        if(details != null)
            garminWorkout.DetailsCollection = [details];

        return garminWorkout;
    }

    public static List<WorkoutDetails> ToWorkoutDetails(this IEnumerable<WorkoutDetailsDto> detailsCollection, Athlete athlete)
    {
        List<WorkoutDetails> result = [];
        foreach (var detail in detailsCollection)
        {
            var workoutDetail = new WorkoutDetails
            {
                Repetitions = detail.Repetitions,
                EffortDuration = detail.EffortDuration,
                RecoveryDuration = detail.RecoveryDuration,
                PaceType = detail.PaceType,
                Pace = detail.PaceType switch
                {
                    PaceType.MasPace => athlete.MasPace,
                    PaceType.FiveKPace => athlete.FiveKPace,
                    PaceType.TenKPace => athlete.TenKPace,
                    PaceType.SemiMarathonPace => athlete.SemiMarathonPace,
                    PaceType.MarathonPace => athlete.MarathonPace,
                    PaceType.EasyPace => athlete.EasyPace,
                    _ => athlete.EasyPace
                }
            };
            if (detail.Details != null)
            {
                workoutDetail.DetailsCollection = new List<WorkoutDetails>
                {
                    new WorkoutDetails
                    {
                        Repetitions = detail.Details.Repetitions,
                        EffortDuration = detail.Details.EffortDuration,
                        RecoveryDuration = detail.Details.RecoveryDuration,
                        PaceType = detail.Details.PaceType,
                        Pace = detail.Details.PaceType switch
                        {
                            PaceType.MasPace => athlete.MasPace,
                            PaceType.FiveKPace => athlete.FiveKPace,
                            PaceType.TenKPace => athlete.TenKPace,
                            PaceType.SemiMarathonPace => athlete.SemiMarathonPace,
                            PaceType.MarathonPace => athlete.MarathonPace,
                            PaceType.EasyPace => athlete.EasyPace,
                            _ => athlete.EasyPace
                        }
                    }
                };
            }
            result.Add(workoutDetail);
        }
        return result;
    }
}

