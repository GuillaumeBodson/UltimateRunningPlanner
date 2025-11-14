namespace WebUI.Services.Dtos;

public readonly record struct PerformancePredictionDto
{
    public int Distance { get; init; }
    public TimeSpan Time { get; init; }
    public RiegelParametersDto RiegelParameters { get; init; }
}

public readonly record struct RiegelParametersDto(double A, double B);


public readonly record struct SimplePerformancePredictionDto
{
    public int Distance { get; init; }
    public TimeSpan Time { get; init; }
}

public record MultiplePerformancesPredictionDto
{
    public IReadOnlyList<SimplePerformancePredictionDto> Predictions { get; init; } = [];
    public RiegelParametersDto RiegelParameters { get; init; }
}