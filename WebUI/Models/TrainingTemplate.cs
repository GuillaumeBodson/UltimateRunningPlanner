using System.Collections.ObjectModel;

namespace WebUI.Models;

/// <summary>
/// Represents a training template that defines which days of the week to schedule workouts
/// based on the number of workouts per week, with support for specific training types.
/// </summary>
public class TrainingTemplate
{
    private readonly Dictionary<int, List<TrainingDay>> _weeklySchedules;

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
    /// Initializes a new instance of the <see cref="TrainingTemplate"/> class with custom training day schedules.
    /// </summary>
    /// <param name="name">The name of the template.</param>
    /// <param name="description">The description of the template.</param>
    /// <param name="customSchedules">Custom weekly schedules with training types.</param>
    public TrainingTemplate(string name, string description, Dictionary<int, List<TrainingDay>> customSchedules)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        
        if (customSchedules == null)
            throw new ArgumentNullException(nameof(customSchedules));

        _weeklySchedules = new Dictionary<int, List<TrainingDay>>(customSchedules);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TrainingTemplate"/> class with legacy custom schedules.
    /// This constructor maintains backward compatibility with the old Dictionary approach.
    /// </summary>
    /// <param name="name">The name of the template.</param>
    /// <param name="description">The description of the template.</param>
    /// <param name="customSchedules">Legacy custom weekly schedules (will be converted to Easy training type).</param>
    public TrainingTemplate(string name, string description, Dictionary<int, HashSet<DayOfWeek>> customSchedules)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? throw new ArgumentNullException(nameof(description));
        
        if (customSchedules == null)
            throw new ArgumentNullException(nameof(customSchedules));

        _weeklySchedules = customSchedules.ToDictionary(
            kvp => kvp.Key,
            kvp => kvp.Value.Select(day => TrainingDay.Easy(day)).ToList()
        );
    }

    /// <summary>
    /// Gets the training days for a specific number of workouts per week.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>A collection of training days for the week, or empty collection if not defined.</returns>
    public IReadOnlyList<TrainingDay> GetTrainingDays(int workoutsPerWeek)
    {
        return _weeklySchedules.TryGetValue(workoutsPerWeek, out var days) 
            ? days 
            : new List<TrainingDay>();
    }

    /// <summary>
    /// Gets the training days for a specific number of workouts per week (legacy version returning DayOfWeek).
    /// This maintains backward compatibility with the existing usage pattern.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>A read-only set of days of the week for training, or empty collection if not defined.</returns>
    public IReadOnlySet<DayOfWeek> GetTrainingDaysLegacy(int workoutsPerWeek)
    {
        var trainingDays = GetTrainingDays(workoutsPerWeek);
        return new HashSet<DayOfWeek>(trainingDays.Select(td => td.DayOfWeek));
    }

    /// <summary>
    /// Gets the training days for a specific number of workouts per week as an ordered queue.
    /// This maintains backward compatibility with the existing Queue-based usage pattern.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>A queue of days of the week for training in a specific order.</returns>
    public Queue<DayOfWeek> GetTrainingDaysQueue(int workoutsPerWeek)
    {
        var days = GetTrainingDaysLegacy(workoutsPerWeek);
        return new Queue<DayOfWeek>(days);
    }

    /// <summary>
    /// Gets the training days with their types for a specific number of workouts per week as an ordered queue.
    /// </summary>
    /// <param name="workoutsPerWeek">The number of workouts per week.</param>
    /// <returns>A queue of training days with their associated training types.</returns>
    public Queue<TrainingDay> GetTrainingDaysWithTypesQueue(int workoutsPerWeek)
    {
        var days = GetTrainingDays(workoutsPerWeek);
        return new Queue<TrainingDay>(days);
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
    /// Creates the default training schedules with specific training types for each day.
    /// </summary>
    private static Dictionary<int, List<TrainingDay>> CreateDefaultSchedules()
    {
        return new Dictionary<int, List<TrainingDay>>
        {
            // 5 workouts per week: Mix of training types
            [5] = 
            [
                TrainingDay.Easy(DayOfWeek.Tuesday),
                TrainingDay.Tempo(DayOfWeek.Wednesday), 
                TrainingDay.Intervals(DayOfWeek.Friday),
                TrainingDay.Easy(DayOfWeek.Saturday),
                TrainingDay.LongRun(DayOfWeek.Sunday)
            ],
            // 4 workouts per week: Focus on key sessions
            [4] = 
            [
                TrainingDay.Easy(DayOfWeek.Monday),
                TrainingDay.Tempo(DayOfWeek.Wednesday),
                TrainingDay.Intervals(DayOfWeek.Friday),
                TrainingDay.LongRun(DayOfWeek.Sunday)
            ],
            // 3 workouts per week: Essential training only
            [3] = 
            [
                TrainingDay.Easy(DayOfWeek.Monday),
                TrainingDay.Tempo(DayOfWeek.Wednesday),
                TrainingDay.LongRun(DayOfWeek.Saturday)
            ]
        };
    }
}