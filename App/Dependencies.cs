using App.Configuration.Options;
using App.Extensions;
using App.Slack.Handler;
using SlackNet.AspNetCore;
using SlackNet.Blocks;

namespace App;

public static class Dependencies
{
    public const string SlackPrefix = "api/v1/slack";

    public static WebApplicationBuilder AddCygniBotDependencies(this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration
            .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Options
        builder.Services.Configure<SlackOptions>(
            builder.Configuration.GetSection(SlackOptions.SectionName));

        // Logging
        builder.ApplicationUseSerilog();
        builder.Services.AddOpenApi();

        // Slack handlers
        builder.Services.AddSingleton<AttendanceButtonClickHandler>();
        
        var conf = new SlackEndpointConfiguration();
        conf.MapToPrefix(SlackPrefix);
        conf.UseSocketMode(false);
        builder.Services.AddSingleton(conf);

        // SlackNet
        builder.Services.AddSlackNet(c =>
        {
            var slackOptions = builder.Configuration.GetSection(SlackOptions.SectionName)
                .Get<SlackOptions>() ?? throw new ArgumentNullException(nameof(SlackOptions));

            // API token
            c.UseApiToken(slackOptions.BotToken);

            // Verify that requests coming in are actually from slack...
            c.UseSigningSecret(slackOptions.SigningSecret);

            // Handlers
            c.RegisterBlockActionHandler<ButtonAction, AttendanceButtonClickHandler>(
                AttendanceButtonClickHandler.ActionId);
        });

        return builder;
    }
}