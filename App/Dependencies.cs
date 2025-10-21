using App.Extensions;
using App.Slack.Handler;
using SlackNet;
using SlackNet.AspNetCore;
using SlackNet.Blocks;
using SlackNet.Extensions.DependencyInjection;

namespace App;

public static class Dependencies
{
    public static WebApplicationBuilder AddCygniBotDependencies(
        this WebApplicationBuilder builder)
    {
        // Logging
        builder.ApplicationUseSerilog();
        
        // OpenApi
        builder.Services.AddOpenApi();
        
        // Handlers
        builder.Services.AddScoped<AttendanceButtonClickHandler>();
        
        // Slack
        var slackEndpointConfiguration = new SlackEndpointConfiguration()
            .MapToPrefix("api/v1/slack");
        builder.Services.AddSingleton(slackEndpointConfiguration);
        ServiceCollectionExtensions.AddSlackNet(builder.Services, c =>
        {
            c.UseApiToken("<your bot or user OAuth token here>");
            c.RegisterBlockActionHandler<ButtonAction, AttendanceButtonClickHandler>("attending_click");
        });

        return builder;
    }
}