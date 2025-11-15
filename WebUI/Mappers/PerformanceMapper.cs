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
            Pace =  Pace.FromTimeAndDistance(dto.Time, dto.Distance)
        };
    }

    public static SimplePerformancePrediction ToSimplePerformancePrediction(this SimplePerformancePredictionDto dto)
    {
        return new SimplePerformancePrediction
        {
            Distance = dto.Distance,
            Pace = Pace.FromTimeAndDistance(dto.Time, dto.Distance)
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
