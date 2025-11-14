internal record MultipleEstimationRequest
{
    public List<int> Distances { get; set; } = [];
    public List<Performance> Performances { get; init; } = [];
}