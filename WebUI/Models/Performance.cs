using WebUI.Services.Dtos;

namespace WebUI.Models;

public record Performance
{
    public int DistanceMeters { get; set; }
    public int TimeSeconds { get; set; }
    public int ElevationGainMeters { get; set; } = 0;
}
