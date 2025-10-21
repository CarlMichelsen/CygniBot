using App;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SlackNet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddCygniBotDependencies();

var app = builder.Build();

app.MapOpenApi();

app.MapScalarApiReference();

app.UseSlackNet(options =>
{
    options.UseSocketMode(true);
});

app.MapGet("/version", ([FromServices] ILogger<Program> logger) =>
{
    var version = ApplicationConstants.Version;
    logger.LogInformation("Version endpoint hit {Version}", version);
    return Results.Ok(version);
});

app.Run();