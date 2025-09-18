namespace WebUI.Models.Workouts;

public interface IQualityWorkout
{
    int Repetitions { get; set; }
    double RunDuration { get; set; }
    double CoolDownDuration { get; set; }
}
