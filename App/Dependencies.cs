using App.Configuration.Options;
using App.Extensions;
using App.HostedServices;
using App.Queue;
using App.Slack;
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

        builder.Services
            .AddSingleton(TimeProvider.System);

        // Options
        builder.Services
            .AddConfigurationOptions<SlackOptions>(builder.Configuration);

        // Logging
        builder.ApplicationUseSerilog();
        builder.Services.AddOpenApi();

        // Slack handlers
        builder.Services
            .AddHostedService<AttendanceVoteQueueBackgroundService>()
            .AddHostedService<SundayAttendanceBackgroundService>()
            .AddSingleton<AttendanceVoteHandler>();
        
        // Queues
        builder.Services.AddSingleReaderChannel<AttendanceVote>();
        
        // SlackNet
        var conf = new SlackEndpointConfiguration();
        conf.MapToPrefix(SlackPrefix);
        conf.UseSocketMode(false);
        builder.Services.AddSingleton(conf);
        builder.Services.AddSlackNet(c =>
        {
            var slackOptions = builder.Configuration.GetSection(SlackOptions.SectionName)
                .Get<SlackOptions>() ?? throw new ArgumentNullException(nameof(SlackOptions));

            // API token
            c.UseApiToken(slackOptions.BotToken);

            // Verify that requests coming in are actually from slack...
            c.UseSigningSecret(slackOptions.SigningSecret);

            // Handlers
            c.RegisterBlockActionHandler<ButtonAction, AttendanceVoteHandler>(AttendanceVoteHandler.ActionId);
            c.RegisterSlashCommandHandler<SendMessageCommandHandler>(SendMessageCommandHandler.CommandIdentifier);
        });
        
        // General purpose services
        builder.Services
            .AddScoped<AttendanceVoteProcessor>()
            .AddScoped<WeeklyMessageFactory>();

        return builder;
    }
}