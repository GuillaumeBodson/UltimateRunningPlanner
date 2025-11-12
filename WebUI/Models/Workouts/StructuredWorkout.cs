namespace WebUI.Models.Workouts;

/// <summary>
/// Base for workouts composed of repetitions with run + recovery segments.
/// Consolidates common interval properties to remove duplication.
/// </summary>
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    public WorkoutDetails? Details
    {
        get
        {
            try
            {
                return DetailsCollection?.SingleOrDefault();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }

    public List<WorkoutDetails>? DetailsCollection { get; set; } = null;


    public bool IsEmpty => !((Details?.Repetitions ?? DetailsCollection?.Count) > 0);

    public TimeSpan WarmUp { get; set; }

    public TimeSpan CoolDown { get; set; }

    /// <summary>
    /// Calculates distance for structured workouts with intervals.
    /// </summary>
    protected override double CalculateWorkoutDistanceCore(decimal easySpeed)
    {
        if (IsEmpty)
        {
            return TotalDuration * (double)Pace.ToMeterPerSeconds();
        }

        // Calculate interval components
        decimal effortDistance = 0;
        int recoveryDuration = 0;
        foreach (var detail in DetailsCollection!)
        {
            if (detail.DetailsCollection?.Count > 0)
            {
                recoveryDuration += detail.RecoveryDuration * detail.Repetitions;

                foreach (var subDetail in detail.DetailsCollection!)
                {
                    recoveryDuration += subDetail.Repetitions * subDetail.RecoveryDuration;
                    int effortDuration = subDetail.Repetitions * subDetail.EffortDuration;
                    effortDistance += (subDetail.Pace.ToMeterPerSeconds() * effortDuration);
                }
            }
            else
            {
                int effortDuration = detail.Repetitions * detail.EffortDuration;
                recoveryDuration += detail.Repetitions * detail.RecoveryDuration;
                effortDistance += (detail.Pace.ToMeterPerSeconds() * effortDuration);
            }
        }
        var recoveryDistance = recoveryDuration * easySpeed;
        return (double)(effortDistance + recoveryDistance);
    }

    /// <summary>
    /// Calculates core duration for structured workouts.
    /// </summary>
    protected override int CalculateWorkoutDurationCore()
    {
        if (IsEmpty)
        {
            return 0;
        }
        int duration = 0;
        foreach (var detail in DetailsCollection!)
        {
            if (detail.DetailsCollection?.Count > 0)
            {
                foreach (var subDetail in detail.DetailsCollection!)
                {
                    duration += subDetail.Repetitions * (subDetail.EffortDuration + subDetail.RecoveryDuration);
                }
                duration += detail.Repetitions * detail.RecoveryDuration;
            }
            else
            {
                duration += detail.Repetitions * (detail.EffortDuration + detail.RecoveryDuration);
            }
        }
        return duration;
    }
    protected override int CalculateWarmUpCoolDownDuration(AthletePreferences preferences)
    {
        int warmUpSeconds = (int)WarmUp.TotalSeconds;
        int coolDownSeconds = (int)CoolDown.TotalSeconds;

        return warmUpSeconds + coolDownSeconds;
    }

    public int GetEffortDistance()
    {
        if (IsEmpty)
        {
            return 0;
        }
        decimal totalEffortDistance = 0;
        foreach (var detail in DetailsCollection!)
        {
            if (detail.DetailsCollection?.Count > 0)
            {
                totalEffortDistance += detail.Repetitions * detail.DetailsCollection!.Sum(subDetail =>
                {
                    int effortDuration = subDetail.Repetitions * subDetail.EffortDuration;
                    return subDetail.Pace.ToMeterPerSeconds() * effortDuration;
                });
            }
            else
            {
                int effortDuration = detail.Repetitions * detail.EffortDuration;
                totalEffortDistance += detail.Pace.ToMeterPerSeconds() * effortDuration;
            }
        }
        return (int)Math.Round(totalEffortDistance);
    }

    public int GetDetailsDuration()
    {
        return CalculateWorkoutDurationCore();
    }

    public string StringifyDetails()
    {
        if (IsEmpty)
        {
            return string.Empty;
        }
        List<string> parts = [];
        foreach (var detail in DetailsCollection!)
        {
            if (detail.DetailsCollection?.Count > 0)
            {
                List<string> subParts = new();
                foreach (var subDetail in detail.DetailsCollection!)
                {
                    subParts.Add($"{detail.Repetitions} x {subDetail.Repetitions} x {subDetail.EffortDuration} @{FormatPace()} min/km");
                }
                parts.Add(string.Join(", ", subParts));
            }
            else
            {
                parts.Add($"{detail.Repetitions} x {detail.EffortDuration} @{FormatPace()} min/km");
            }
        }
        return string.Join(", ", parts);
    }
}