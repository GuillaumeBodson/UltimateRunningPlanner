namespace WebUI.Models;

/// <summary>
/// Represents the type of training for a workout.
/// </summary>
public enum TrainingType
{
    /// <summary>
    /// Easy-paced recovery runs.
    /// </summary>
    Easy,
    
    /// <summary>
    /// Tempo runs at lactate threshold pace.
    /// </summary>
    Tempo,
    
    /// <summary>
    /// High-intensity interval training.
    /// </summary>
    Intervals,
    
    /// <summary>
    /// Long endurance runs.
    /// </summary>
    LongRun,
    
    /// <summary>
    /// Steady-state runs at moderate effort.
    /// </summary>
    Steady,
    
    /// <summary>
    /// Race day or race-pace workouts.
    /// </summary>
    Race
}