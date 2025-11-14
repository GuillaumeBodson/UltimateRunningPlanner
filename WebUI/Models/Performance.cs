namespace WebUI.Models;

public class Performance
{
    public int DistanceMeters { get; set; }
    public int TimeSeconds { get; set; }
    public int ElevationGainMeters { get; set; } = 0;
}
