using App;
using App.Slack.Handler;
using Microsoft.AspNetCore.Mvc;
using Scalar.AspNetCore;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.WebApi;

var builder = WebApplication.CreateBuilder(args);

builder.AddCygniBotDependencies();

var app = builder.Build();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
        .WithTitle(ApplicationConstants.Name)
        .WithTheme(ScalarTheme.Mars);
});

app.UseSlackNet();

app.MapGet("/version", ([FromServices] ILogger<Program> logger) =>
{
    var version = ApplicationConstants.Version;
    logger.LogInformation("Version endpoint hit {Version}", version);
    return Results.Ok(version);
});

using (var scope = app.Services.CreateScope())
{
    var slackApiClient = scope
        .ServiceProvider
        .GetRequiredService<ISlackApiClient>();

    await slackApiClient.Chat.PostMessage(new Message
    {
        Channel = "#development_slack_integration",
        Text = "Hello from C# 🎉",
        Blocks =
        [
            new ActionsBlock
            {
                Elements =
                [
                    new Button
                    {
                        Text = new PlainText { Text = "Hello!" }, ActionId = AttendanceButtonClickHandler.ActionId, Value = "A|green"
                    },
                ]
            }
        ]
    });
}

app.Run();