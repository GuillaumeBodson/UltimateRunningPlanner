using WebUI.Models;
using WebUI.Services.Dtos;

namespace WebUI.Mappers;

public static class PerformanceMapper
{
    public static PerformancePrediction ToPerformancePrediction(this PerformancePredictionDto dto)
    {
        return new PerformancePrediction
        {
            Distance = dto.Distance,
            Time = dto.Time,
            RiegelParameters = new RiegelParameters(dto.RiegelParameters.A, dto.RiegelParameters.B),
            Pace = new Pace((int)Math.Round(dto.Time.TotalSeconds / (dto.Distance / 1000d)))
        };
    }

    public static SimplePerformancePrediction ToSimplePerformancePrediction(this SimplePerformancePredictionDto dto)
    {
        return new SimplePerformancePrediction
        {
            Distance = dto.Distance,
            Pace = new Pace((int)Math.Round(dto.Time.TotalSeconds / (dto.Distance / 1000d)))
        };
    }

    public static PerformanceDto ToDto(this Performance performance) => new()
    {
        DistanceMeters = performance.Distance,
        TimeSeconds = performance.TimeSeconds,
        //ElevationGainMeters = performance.ElevationGainMeters
    };

    public static MultiplePerformancesPrediction ToMultiplePerformancesPrediction(this MultiplePerformancesPredictionDto dto)
    {
        return new MultiplePerformancesPrediction
        {
            Predictions = dto.Predictions.Select(p => p.ToSimplePerformancePrediction()).ToArray(),
            RiegelParameters = new RiegelParameters(dto.RiegelParameters.A, dto.RiegelParameters.B)
        };
    }
}
