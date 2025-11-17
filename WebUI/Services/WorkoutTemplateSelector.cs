using WebUI.Components.WorkoutTemplates;
using WebUI.Models.Workouts;
using WebUI.Services.Interfaces;

namespace WebUI.Services;

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

    public Type Select(PlannedWorkout workout)
    {
        ArgumentNullException.ThrowIfNull(workout);
        return _map.TryGetValue(workout.GetType(), out var componentType)
            ? componentType
            : typeof(GenericWorkoutTemplate);
    }
}