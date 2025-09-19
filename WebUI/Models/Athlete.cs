namespace WebUI.Models;

public class Athlete
{
    public Pace EasyPace { get; set; }
    public Pace MarathonPace { get; set; }
    public Pace SemiMarathonPace { get; set; }
    public Pace VmaPace { get; set; }

    // Training templates suitable for this athlete (different day counts, distributions, periods, etc.)
    private readonly TrainingTemplateCollection _trainingTemplates = [];
    public TrainingTemplateCollection TrainingTemplates => _trainingTemplates;

    public void AddTemplate(TrainingTemplate template)
    {
        ArgumentNullException.ThrowIfNull(template);
        _trainingTemplates.TryAdd(template);
    }

    public TrainingTemplate? GetTemplateForTrainingDays(int trainingDays) =>
        _trainingTemplates.FirstOrDefault(t => t.TrainingDaysCount == trainingDays);
}

public class AthleteCreation
{
    public double EasyPace { get; set; }
    public double MarathonPace { get; set; }
    public double SemiMarathonPace { get; set; }
    public double VmaPace { get; set; }
    public Athlete ToAthlete()
    {

        return new Athlete
        {
            EasyPace = new Pace(CalculateTotalSeconds(EasyPace)),
            MarathonPace = new Pace(CalculateTotalSeconds(MarathonPace)),
            SemiMarathonPace = new Pace(CalculateTotalSeconds(SemiMarathonPace)),
            VmaPace = new Pace(CalculateTotalSeconds(VmaPace))
        };
    }

    public AthleteCreation() { }
    public AthleteCreation(Athlete athlete)
    {
        EasyPace = athlete.EasyPace.ToMinutesDotSeconds();
        MarathonPace = athlete.MarathonPace.ToMinutesDotSeconds();
        SemiMarathonPace = athlete.SemiMarathonPace.ToMinutesDotSeconds();
        VmaPace = athlete.VmaPace.ToMinutesDotSeconds();
    }

    private static int CalculateTotalSeconds(double minutesDotSeconds)
    {
        int minutes = (int)Math.Floor(minutesDotSeconds);
        int seconds = (int)Math.Round((minutesDotSeconds-minutes)*100);
        seconds += minutes * 60;

        return seconds;
    }
}
