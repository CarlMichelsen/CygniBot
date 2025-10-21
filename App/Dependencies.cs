using App.Configuration.Options;
using App.Extensions;
using App.Slack.Handler;
using SlackNet.AspNetCore;
using SlackNet.Blocks;

namespace App;

public static class Dependencies
{
    public const string SlackRootPath = "api/v1/slack";
    
    public static WebApplicationBuilder AddCygniBotDependencies(
        this WebApplicationBuilder builder)
    {
        // Configuration
        builder.Configuration
            .AddJsonFile(
                "secrets.json",
                optional: true,
                reloadOnChange: true)
            .AddEnvironmentVariables();
        
        // Configuration-Options
        builder.Services
            .AddConfigurationOptions<SlackOptions>(builder.Configuration);
        
        // Logging
        builder.ApplicationUseSerilog();
        
        // OpenApi
        builder.Services.AddOpenApi();
        
        // Handlers
        builder.Services.AddSingleton<AttendanceButtonClickHandler>();
        
        // Slack
        var slackEndpointConfiguration = new SlackEndpointConfiguration()
            .MapToPrefix(SlackRootPath);
        builder.Services.AddSingleton(slackEndpointConfiguration);
        builder.Services.AddSlackNet((AspNetSlackServiceConfiguration configure) =>
        {
            var slackOptions = builder
                .Configuration
                .GetSection(SlackOptions.SectionName)
                .Get<SlackOptions>() ?? throw new ArgumentNullException(nameof(SlackOptions));
            
            configure.UseApiToken(slackOptions.BotToken);
            configure.RegisterBlockActionHandler<ButtonAction, AttendanceButtonClickHandler>(AttendanceButtonClickHandler.ActionId);
        });

        return builder;
    }
}