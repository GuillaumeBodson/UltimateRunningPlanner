using Microsoft.AspNetCore.Mvc;
using RunGuesser;

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
        var result = RiegelModel.Predict(request.DistanceMeters, performancesTuples);
        return Results.Ok(result);
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
  .Produces<PerformancePrediction>(StatusCodes.Status200OK)
  .Produces<string>(StatusCodes.Status400BadRequest)
  .Produces<string>(StatusCodes.Status500InternalServerError);

app.MapPost("/estimate-with-r", ([FromBody] EstimationWithRParameterRequest request) =>
{
    try
    {
        var result = RiegelModel.Predict(request.DistanceMeters, request.RParameter);
        return Results.Ok(result);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }

}).WithDescription("Estimate pace for a given distance based on a previous performance and a specified Riegel parameter.")
  .WithName("EstimateRunTimeWithRParameter")
  .Produces<PerformancePrediction>(StatusCodes.Status200OK)
  .Produces<string>(StatusCodes.Status400BadRequest)
  .Produces<string>(StatusCodes.Status500InternalServerError);

app.MapPost("/estimate-mulitple", ([FromBody] MultipleEstimationRequest request) =>
{
    try
    {
        var performancesTuples = request.Performances.Select(x => x.ToTuple()).ToList();
        var results = RiegelModel.Predict(request.Distances.Select(d => (double)d).ToList(), performancesTuples);

        return Results.Ok(results);
    }
    catch (ArgumentException ex)
    {
        return Results.BadRequest(ex.Message);
    }
    catch (Exception ex)
    {
        return Results.InternalServerError(ex.Message);
    }
}).WithDescription("Estimate paces for multiple distances based on previous performances using Riegel's formula.")
  .WithName("EstimateMultipleRunTimes")
  .Produces<MultiplePerformancesPrediction>(StatusCodes.Status200OK)
  .Produces<string>(StatusCodes.Status400BadRequest)
  .Produces<string>(StatusCodes.Status500InternalServerError);

app.Run();
