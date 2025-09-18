namespace WebUI.Models.Workouts;

public interface IStructuredWorkout
{
    int Repetitions { get; set; }
    int IntervalDuration { get; set; }
    int RecoveryDuration { get; set; }
    bool IsEmpty { get; }
}
