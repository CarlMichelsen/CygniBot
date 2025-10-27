using App;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SlackNet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.AddCygniBotDependencies();

var app = builder.Build();

app.UseSlackNet();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
        .WithTitle(ApplicationConstants.Name)
        .WithTheme(ScalarTheme.Mars);
});

app.MapGet("/version", ([FromServices] ILogger<Program> logger) => Results.Ok(ApplicationConstants.Version));

// await app.Services.SendTestMessage();

app.Run();