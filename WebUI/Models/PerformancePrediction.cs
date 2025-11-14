namespace WebUI.Models;

public readonly record struct PerformancePrediction
{
    public int Distance { get; init; }
    public TimeSpan Time { get; init; }
    public Pace Pace { get; init; }
    public RiegelParameters RiegelParameters { get; init; }
}

public readonly record struct RiegelParameters(double A, double B);


public record PerformancesPrediction
{
    public List<SimplePerformancePrediction> Predictions { get; set; } = [];
    public RiegelParameters RiegelParameters { get; set; }
}

public readonly record struct SimplePerformancePrediction
{
    public int Distance { get; init; }
    public Pace Pace { get; init; }
}

public record MultiplePerformancesPrediction
{
    public IReadOnlyList<SimplePerformancePrediction> Predictions { get; init; } = [];
    public RiegelParameters RiegelParameters { get; init; }
}