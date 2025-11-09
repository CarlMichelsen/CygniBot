using App.Configuration.Options;
using App.Queue;
using App.Slack.Mapper;
using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.WebApi;

namespace App.Slack;

public class AttendanceVoteProcessor(
    IOptionsSnapshot<SlackOptions> slackOptions,
    ISlackApiClient slackApiClient)
{
    public async Task Process(
        AttendanceVote attendanceVote)
    {
        // Re-fetching the message in order to guarantee that it has the latest content
        var historyResponse = await slackApiClient.Conversations.History(
            channelId: attendanceVote.ChannelId,
            latestTs: attendanceVote.MessageTs,
            inclusive: true,
            limit: 1);
        
        var slackWeeklyAttendanceMessage = historyResponse.Messages.First();
        var weeklyAttendanceMessage = WeeklyAttendanceMessageMapper
            .GetMessageFromMetadata(slackWeeklyAttendanceMessage)
            .ToLatestVersion();

        var block = weeklyAttendanceMessage
            .Blocks
            .Find(b => b.Date == attendanceVote.Value)
                ?? throw new Exception("Unable to find a weekly attendance message block for the given date");

        var isAttending = block.Attending.Find(a => a == attendanceVote.UserId);
        if (isAttending is null)
        {
            block.Attending.Add(attendanceVote.UserId);
        }
        else
        {
            block.Attending.Remove(isAttending);
        }
        
        var message = WeeklyAttendanceMessageMapper.MapMessage(
            weeklyAttendanceMessage,
            slackOptions.Value.Channel);
        await slackApiClient.Chat.Update(new MessageUpdate
        {
            ChannelId = attendanceVote.ChannelId,
            Ts = attendanceVote.MessageTs,
            Blocks = message.Blocks,
            Text = message.Text,
        });
    }
}