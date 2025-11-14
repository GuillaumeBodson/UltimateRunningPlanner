internal record EstimationRequest 
{ 
    public double DistanceMeters { get; init; }
    public List<Performance> Performances { get; init; } = [];
}
