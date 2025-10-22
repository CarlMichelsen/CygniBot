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

app.UseSlackNet();

app.MapOpenApi();

app.MapScalarApiReference(options =>
{
    options
        .WithTitle(ApplicationConstants.Name)
        .WithTheme(ScalarTheme.Mars);
});

app.MapGet("/version", ([FromServices] ILogger<Program> logger) => Results.Ok(ApplicationConstants.Version));

using (var scope = app.Services.CreateScope())
{
    var slackApiClient = scope
        .ServiceProvider
        .GetRequiredService<ISlackApiClient>();

    await slackApiClient.Chat.PostMessage(new Message
    {
        Channel = "#development_slack_integration",
        Blocks =
        [
            new SectionBlock
            {
                Text = new Markdown { Text = "Hello from C# 🎉" }
            },
            new ActionsBlock
            {
                Elements =
                [
                    new Button
                    {
                        ActionId = AttendanceButtonClickHandler.ActionId,
                        Text = new PlainText { Text = "Hello!" },
                        Value = "A|green"
                    },
                ]
            }
        ]
    });
}

app.Run();