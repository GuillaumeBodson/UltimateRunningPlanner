using System.Diagnostics.CodeAnalysis;

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
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "RunDuration cannot be negative.");
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
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value), "CoolDownDuration cannot be negative.");
            _recoveryDuration = value;
        }
    }

    /// <summary>
    /// Indicates whether the structured block is effectively empty (no interval component).
    /// </summary>
    [MemberNotNullWhen(false, nameof(Repetitions))]
    public bool IsEmpty => Repetitions == 0 || IntervalDuration <= 0;
    
}