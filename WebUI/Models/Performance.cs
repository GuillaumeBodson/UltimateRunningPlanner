namespace WebUI.Models;

public class Performance
{
    public double DistanceMeters { get; set; }
    public int TimeSeconds { get; set; }
    public int ElevationGainMeters { get; set; } = 0;
}
