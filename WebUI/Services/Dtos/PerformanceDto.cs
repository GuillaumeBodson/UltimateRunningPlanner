namespace WebUI.Services.Dtos;

public record PerformanceDto
{
    public double DistanceMeters { get; init; }
    public double TimeSeconds { get; init; }
}