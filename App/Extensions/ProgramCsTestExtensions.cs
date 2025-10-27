using App.Configuration.Options;
using App.Slack;
using App.Slack.Mapper;
using Microsoft.Extensions.Options;
using SlackNet;

namespace App.Extensions;

public static class ProgramCsTestExtensions
{
    public static async Task SendTestMessage(this IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var slackClient = scope
            .ServiceProvider
            .GetRequiredService<ISlackApiClient>();

        var messageSender = scope
            .ServiceProvider
            .GetRequiredService<WeeklyMessageFactory>();

        var slackOptions = scope
            .ServiceProvider
            .GetRequiredService<IOptionsSnapshot<SlackOptions>>();

        var message = messageSender.CreateMessage(
            TimeProvider.System.GetCurrentWeek(),
            TimeProvider.System.GetCurrentYear(),
            DateHelpers.WorkDays);

        await slackClient.Chat.PostMessage(
            WeeklyAttendanceMessageMapper.MapMessage(
                message,
                slackOptions.Value.Channel));
    }
}