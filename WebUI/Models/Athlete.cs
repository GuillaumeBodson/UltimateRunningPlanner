namespace WebUI.Models;

public class Athlete
{
    public Pace EasyPace { get; set; }
    public Pace MarathonPace { get; set; }
    public Pace SemiMarathonPace { get; set; }
    public Pace TenKPace { get; set; }
    public Pace FiveKPace { get; set; }
    public Pace MasPace { get; set; }

    public List<Performance> Performances { get; set; } = [];
    public HashSet<TrainingTemplate> TrainingTemplates { get; set; } = [];
}

public class AthleteCreation
{
    public double EasyPace { get; set; }
    public double MarathonPace { get; set; }
    public double SemiMarathonPace { get; set; }
    public double TenKPace { get; set; }
    public double FiveKPace { get; set; }
    public double MasPace { get; set; }
    public Athlete ToAthlete()
        => new Athlete
        {
            EasyPace = Pace.FromMinutesSeconds(EasyPace),
            MarathonPace = Pace.FromMinutesSeconds(MarathonPace),
            SemiMarathonPace = Pace.FromMinutesSeconds(SemiMarathonPace),
            TenKPace = Pace.FromMinutesSeconds(TenKPace),
            FiveKPace = Pace.FromMinutesSeconds(FiveKPace),
            MasPace = Pace.FromMinutesSeconds(MasPace)
        };

    public AthleteCreation() { }
    public AthleteCreation(Athlete athlete)
    {
        EasyPace = athlete.EasyPace.ToMinutesDotSeconds();
        MarathonPace = athlete.MarathonPace.ToMinutesDotSeconds();
        SemiMarathonPace = athlete.SemiMarathonPace.ToMinutesDotSeconds();
        TenKPace = athlete.TenKPace.ToMinutesDotSeconds();
        FiveKPace = athlete.FiveKPace.ToMinutesDotSeconds();
        MasPace = athlete.MasPace.ToMinutesDotSeconds();
    }
}
