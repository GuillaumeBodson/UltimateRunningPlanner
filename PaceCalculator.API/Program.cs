using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/estimate", ([FromBody]EstimationRequest request) =>
{
    try
    {
        var performancesTuples = request.Performances.Select(x => x.ToTuple()).ToList();
        var result = RunGuesser.RiegelModel.Predict(request.DistanceMeters, performancesTuples);
        return Results.Ok(result.PaceSecondsPerKm);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }
}).WithDescription("Estimate pace for a given distance based on previous performances using Riegel's formula.")
  .WithName("EstimateRunTime")
  .Produces<double>(StatusCodes.Status200OK)
  .Produces<string>(StatusCodes.Status400BadRequest)
  .Produces<string>(StatusCodes.Status500InternalServerError);

app.Run();


internal record Performance
{
    public double DistanceMeters { get; set; }
    public double TimeSeconds { get; set; }

    public (double DistanceMeters, double TimeSeconds) ToTuple() => (DistanceMeters, TimeSeconds);
}

internal record EstimationRequest 
{ 
    public double DistanceMeters { get; set; }
    public List<Performance> Performances { get; set; } = [];
}