internal record Performance
{
    public double DistanceMeters { get; init; }
    public double TimeSeconds { get; init; }

    public (double DistanceMeters, double TimeSeconds) ToTuple() => (DistanceMeters, TimeSeconds);
}
