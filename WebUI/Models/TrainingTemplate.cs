using System.Collections.ObjectModel;

namespace WebUI.Models;

/// <summary>
/// Represents a training template that defines which days of the week to schedule workouts
/// based on the number of workouts per week.
/// </summary>
public class TrainingTemplate
{
    private readonly Dictionary<int, HashSet<DayOfWeek>> _weeklySchedules;

    /// <summary>
    /// Gets the name of this training template.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the description of this training template.
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingTemplate"/> class with default schedules.
    /// </summary>
    public TrainingTemplate() : this("Default Template", "Standard running training template")
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingTemplate"/> class.
    /// </summary>
    /// <param name="name">The name of the template.</param>
    /// <param name="description">The description of the template.</param>
    public TrainingTemplate(string name, string description)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        
        _weeklySchedules = CreateDefaultSchedules();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingTemplate"/> class with custom schedules.
    /// </summary>
    /// <param name="name">The name of the template.</param>
    /// <param name="description">The description of the template.</param>
    /// <param name="customSchedules">Custom weekly schedules.</param>
    public TrainingTemplate(string name, string description, Dictionary<int, HashSet<DayOfWeek>> customSchedules)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        
        if (customSchedules == null)
            throw new ArgumentNullException(nameof(customSchedules));

        _weeklySchedules = new Dictionary<int, HashSet<DayOfWeek>>(customSchedules);
    }

    /// <summary>
    /// Gets the training days for a specific number of workouts per week.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>A collection of days of the week for training, or empty collection if not defined.</returns>
    public IReadOnlySet<DayOfWeek> GetTrainingDays(int workoutsPerWeek)
    {
        return _weeklySchedules.TryGetValue(workoutsPerWeek, out var days) 
            ? days 
            : new HashSet<DayOfWeek>();
    }

    /// <summary>
    /// Gets the training days for a specific number of workouts per week as an ordered queue.
    /// This maintains backward compatibility with the existing Queue-based usage pattern.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>A queue of days of the week for training in a specific order.</returns>
    public Queue<DayOfWeek> GetTrainingDaysQueue(int workoutsPerWeek)
    {
        var days = GetTrainingDays(workoutsPerWeek);
        return new Queue<DayOfWeek>(days);
    }

    /// <summary>
    /// Checks if a training schedule is defined for the specified number of workouts per week.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>True if a schedule is defined; otherwise, false.</returns>
    public bool HasScheduleFor(int workoutsPerWeek)
    {
        return _weeklySchedules.ContainsKey(workoutsPerWeek);
    }

    /// <summary>
    /// Gets all supported workout frequencies (number of workouts per week).
    /// </summary>
    /// <returns>A collection of supported workout frequencies.</returns>
    public IReadOnlyCollection<int> GetSupportedFrequencies()
    {
        return _weeklySchedules.Keys.ToList();
    }

    /// <summary>
    /// Creates the default training schedules.
    /// </summary>
    private static Dictionary<int, HashSet<DayOfWeek>> CreateDefaultSchedules()
    {
        return new Dictionary<int, HashSet<DayOfWeek>>
        {
            [5] = [DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Saturday, DayOfWeek.Sunday],
            [4] = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Friday, DayOfWeek.Sunday],
            [3] = [DayOfWeek.Monday, DayOfWeek.Wednesday, DayOfWeek.Saturday]
        };
    }
}