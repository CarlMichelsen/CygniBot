using App.Configuration.Options;
using App.Slack.Mapper;
using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.Interaction;
using SlackNet.WebApi;

namespace App.Slack.Handler;

public class SendMessageCommandHandler(
    TimeProvider timeProvider,
    ISlackApiClient slackClient,
    IOptionsSnapshot<SlackOptions> slackOptions,
    WeeklyMessageFactory weeklyMessageFactory) : ISlashCommandHandler
{
    public const string CommandIdentifier = "/sendmessage";
    
    public async Task<SlashCommandResponse> Handle(SlashCommand command)
    {
        var allowedChannel = slackOptions.Value.Channel.Replace("#", string.Empty);
        if (command.ChannelName != allowedChannel)
        {
            return new SlashCommandResponse
            {
                Message = new Message
                {
                    Text = $"Only commands sent from '{allowedChannel}' will be accepted - this command was sent from '{command.ChannelName}'"
                },
                ResponseType = ResponseType.Ephemeral,
            };
        }
        
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