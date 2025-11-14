namespace WebUI.Models;

public interface IPerformance
{
    int Distance { get; }
    Pace Pace { get; }
}

public class Performance : IPerformance
{
    public int Distance { get; set; }
    public int TimeSeconds { get; set; }
    public int ElevationGainMeters { get; set; } = 0;
    public Pace Pace => new Pace((int)Math.Round(TimeSeconds / (Distance / 1000d)));
}
