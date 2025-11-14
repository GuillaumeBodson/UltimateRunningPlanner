using RunGuesser;

internal record EstimationWithRParameterRequest
{
    public double DistanceMeters { get; init; }
    public RiegelParameters RParameter { get; init; }
}
