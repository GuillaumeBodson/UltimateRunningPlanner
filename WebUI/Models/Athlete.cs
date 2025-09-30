namespace WebUI.Models;

public class Athlete
{
    public Pace EasyPace { get; set; }
    public Pace MarathonPace { get; set; }
    public Pace SemiMarathonPace { get; set; }
    public Pace VmaPace { get; set; }

    public HashSet<TrainingTemplate> TrainingTemplates { get; set; } = [];
}

public class AthleteCreation
{
    public double EasyPace { get; set; }
    public double MarathonPace { get; set; }
    public double SemiMarathonPace { get; set; }
    public double VmaPace { get; set; }
    public Athlete ToAthlete()
        => new Athlete
        {
            EasyPace = Pace.FromMinutesDotSeconds(EasyPace),
            MarathonPace = Pace.FromMinutesDotSeconds(MarathonPace),
            SemiMarathonPace = Pace.FromMinutesDotSeconds(SemiMarathonPace),
            VmaPace = Pace.FromMinutesDotSeconds(VmaPace)
        };

    public AthleteCreation() { }
    public AthleteCreation(Athlete athlete)
    {
        EasyPace = athlete.EasyPace.ToMinutesDotSeconds();
        MarathonPace = athlete.MarathonPace.ToMinutesDotSeconds();
        SemiMarathonPace = athlete.SemiMarathonPace.ToMinutesDotSeconds();
        VmaPace = athlete.VmaPace.ToMinutesDotSeconds();
    }
}
