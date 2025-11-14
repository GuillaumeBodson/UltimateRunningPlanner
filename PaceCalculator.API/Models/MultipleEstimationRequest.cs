internal record MultipleEstimationRequest
{
    public List<int> Distances { get; init; } = [];
    public List<Performance> Performances { get; init; } = [];
}