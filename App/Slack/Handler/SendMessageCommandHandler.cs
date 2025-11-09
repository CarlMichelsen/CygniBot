using App.Configuration.Options;
using App.Slack.Mapper;
using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.Interaction;

namespace App.Slack.Handler;

public class SendMessageCommandHandler(
    TimeProvider timeProvider,
    ISlackApiClient slackClient,
    IOptionsSnapshot<SlackOptions> slackOptions,
    WeeklyMessageFactory weeklyMessageFactory) : ISlashCommandHandler
{
    public const string CommandIdentifier = "/sendmessage";
    
    public async Task<SlashCommandResponse> Handle(SlashCommand _)
    {
        var message = weeklyMessageFactory.CreateMessage(
            timeProvider.GetCurrentWeek(),
            timeProvider.GetCurrentYear(),
            DateHelpers.WorkDays);
        
        await slackClient.Chat.PostMessage(
            WeeklyAttendanceMessageMapper.MapMessage(
                message,
                slackOptions.Value.Channel));
        
        return null!;
    }
}