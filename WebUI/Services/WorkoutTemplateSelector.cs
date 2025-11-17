using WebUI.Components.WorkoutTemplates;
using WebUI.Models.Workouts;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

/// <summary>
/// Maps workout types to their corresponding Razor component types for rendering.
/// </summary>
public sealed class WorkoutTemplateSelector : IWorkoutTemplateSelector
{
    private static readonly Dictionary<Type, Type> _map = new()
    {
        { typeof(EasyWorkout), typeof(GenericWorkoutTemplate)  },
        { typeof(LongRunWorkout), typeof(StructuredWorkoutTemplate)  },
        { typeof(SteadyWorkout), typeof(StructuredWorkoutTemplate) },
        { typeof(RaceWorkout), typeof(GenericWorkoutTemplate) },
        { typeof(IntervalWorkout), typeof(StructuredWorkoutTemplate) },
        { typeof(TempoWorkout), typeof(StructuredWorkoutTemplate) },
        // Add more mappings here as new PlannedWorkout subtypes are introduced.
    };

    /// <summary>
    /// Selects the appropriate component type for the given workout.
    /// Returns <see cref="GenericWorkoutTemplate"/> as the default if no specific mapping exists.
    /// </summary>
    /// <param name="workout">The workout to select a template for.</param>
    /// <returns>The component type to use for rendering the workout.</returns>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="workout"/> is null.</exception>
    public Type Select(PlannedWorkout workout)
    {
        ArgumentNullException.ThrowIfNull(workout);
        return _map.TryGetValue(workout.GetType(), out var componentType)
            ? componentType
            : typeof(GenericWorkoutTemplate);
    }
}