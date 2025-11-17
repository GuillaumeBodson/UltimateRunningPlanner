using WebUI.Models.Workouts;

namespace WebUI.Services.Interfaces;

/// <summary>
/// Selects the appropriate component type for rendering a workout template.
/// </summary>
public interface IWorkoutTemplateSelector
{
    /// <summary>
    /// Selects the component type to render for the given workout.
    /// </summary>
    /// <param name="workout">The workout to select a template for.</param>
    /// <returns>The component type to use for rendering.</returns>
    Type Select(PlannedWorkout workout);
}