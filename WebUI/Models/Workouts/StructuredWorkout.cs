using System.Diagnostics.CodeAnalysis;
using Garmin = GarminRunerz.Workout.Services;
namespace WebUI.Models.Workouts;

/// <summary>
/// Base for workouts composed of repetitions with run + recovery segments.
/// Consolidates common interval properties to remove duplication.
/// </summary>
public abstract class StructuredWorkout : PlannedWorkout, IStructuredWorkout
{
    private int _repetitions;
    private int _intervalDuration;
    private int _recoveryDuration;

    public int Repetitions
    {
        get => _repetitions;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "Repetitions cannot be negative.");
            _repetitions = value;
        }
    }

    /// <summary>
    /// Duration (seconds) of each repetition's quality segment (currently double for backward compatibility).
    /// </summary>
    public int IntervalDuration
    {
        get => _intervalDuration;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "IntervalDuration cannot be negative.");
            _intervalDuration = value;
        }
    }

    /// <summary>
    /// Duration (seconds) of recovery between repetitions (not the final post-session cool-down).
    /// </summary>
    public int RecoveryDuration
    {
        get => _recoveryDuration;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "RecoveryDuration cannot be negative.");
            _recoveryDuration = value;
        }
    }

    public bool IsEmpty => Repetitions == 0 || IntervalDuration <= 0;
    
    // Override template methods for interval-based calculations
    /// <summary>
    /// Gets the appropriate speed for intervals based on run type.
    /// </summary>
    protected override decimal GetEffortSpeed(Athlete athlete)
    {
        return RunType switch
        {
            Garmin.Models.RunType.Tempo => athlete.SemiMarathonPace.ToMeterPerSeconds(),
            Garmin.Models.RunType.Intervals => athlete.VmaPace.ToMeterPerSeconds(),
            _ => Pace.ToMeterPerSeconds()
        };
    }
    
    /// <summary>
    /// Calculates distance for structured workouts with intervals.
    /// </summary>
    protected override double CalculateWorkoutDistanceCore(decimal easySpeed, decimal effortSpeed)
    {
        if (IsEmpty)
        {
            return TotalDuration * (double)easySpeed;
        }
        
        // Calculate interval components
        double effortDistance = (double)effortSpeed * IntervalDuration * Repetitions;
        double recoveryDistance = (double)easySpeed * RecoveryDuration * Repetitions;
        
        if (RunType is Garmin.Models.RunType.Tempo or Garmin.Models.RunType.Intervals)
        {
            // Fixed warm-up/cool-down for tempo/intervals
            double warmUpDistance = 15 * 60 * (double)easySpeed;
            double coolDownDistance = 10 * 60 * (double)easySpeed;
            return effortDistance + recoveryDistance + warmUpDistance + coolDownDistance;
        }
        else
        {
            // For other structured runs (e.g. long runs with intervals)
            // Use remaining time after intervals for warm-up
            double warmUpDuration = TotalDuration - (IntervalDuration + RecoveryDuration) * Repetitions;
            if (warmUpDuration < 0) warmUpDuration = 0; // Safety check
            
            double warmUpDistance = warmUpDuration * (double)easySpeed;
            return effortDistance + recoveryDistance + warmUpDistance;
        }
    }
    
    /// <summary>
    /// Calculates core duration for structured workouts.
    /// </summary>
    protected override int CalculateWorkoutDurationCore()
    {
        return (IntervalDuration + RecoveryDuration) * Repetitions;
    }
}