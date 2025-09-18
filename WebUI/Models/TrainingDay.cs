namespace WebUI.Models;

/// <summary>
/// Represents a scheduled training day with a specific training type.
/// </summary>
public readonly struct TrainingDay : IEquatable<TrainingDay>
{
    /// <summary>
    /// Gets the day of the week for this training.
    /// </summary>
    public DayOfWeek DayOfWeek { get; }

    /// <summary>
    /// Gets the type of training for this day.
    /// </summary>
    public TrainingType TrainingType { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingDay"/> struct.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <param name="trainingType">The type of training.</param>
    public TrainingDay(DayOfWeek dayOfWeek, TrainingType trainingType)
    {
        DayOfWeek = dayOfWeek;
        TrainingType = trainingType;
    }

    /// <summary>
    /// Creates a training day with easy training type.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with easy training type.</returns>
    public static TrainingDay Easy(DayOfWeek dayOfWeek) => new(dayOfWeek, TrainingType.Easy);

    /// <summary>
    /// Creates a training day with tempo training type.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with tempo training type.</returns>
    public static TrainingDay Tempo(DayOfWeek dayOfWeek) => new(dayOfWeek, TrainingType.Tempo);

    /// <summary>
    /// Creates a training day with intervals training type.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with intervals training type.</returns>
    public static TrainingDay Intervals(DayOfWeek dayOfWeek) => new(dayOfWeek, TrainingType.Intervals);

    /// <summary>
    /// Creates a training day with long run training type.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with long run training type.</returns>
    public static TrainingDay LongRun(DayOfWeek dayOfWeek) => new(dayOfWeek, TrainingType.LongRun);

    /// <summary>
    /// Creates a training day with steady training type.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with steady training type.</returns>
    public static TrainingDay Steady(DayOfWeek dayOfWeek) => new(dayOfWeek, TrainingType.Steady);

    /// <summary>
    /// Creates a training day with race training type.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with race training type.</returns>
    public static TrainingDay Race(DayOfWeek dayOfWeek) => new(dayOfWeek, TrainingType.Race);

    /// <summary>
    /// Implicitly converts a DayOfWeek to a TrainingDay with Easy training type.
    /// This maintains backward compatibility.
    /// </summary>
    /// <param name="dayOfWeek">The day of the week.</param>
    /// <returns>A training day with easy training type.</returns>
    public static implicit operator TrainingDay(DayOfWeek dayOfWeek) => Easy(dayOfWeek);

    /// <summary>
    /// Implicitly converts a TrainingDay to a DayOfWeek.
    /// This maintains backward compatibility.
    /// </summary>
    /// <param name="trainingDay">The training day.</param>
    /// <returns>The day of the week.</returns>
    public static implicit operator DayOfWeek(TrainingDay trainingDay) => trainingDay.DayOfWeek;

    public bool Equals(TrainingDay other) => DayOfWeek == other.DayOfWeek && TrainingType == other.TrainingType;

    public override bool Equals(object? obj) => obj is TrainingDay other && Equals(other);

    public override int GetHashCode() => HashCode.Combine(DayOfWeek, TrainingType);

    public static bool operator ==(TrainingDay left, TrainingDay right) => left.Equals(right);

    public static bool operator !=(TrainingDay left, TrainingDay right) => !left.Equals(right);

    public override string ToString() => $"{DayOfWeek}: {TrainingType}";
}