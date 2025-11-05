using App.Queue;
using App.Slack.Mapper;
using SlackNet;

namespace App.Slack;

public class AttendanceVoteProcessor(
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
        var msg = historyResponse.Messages.First();
        
        var weeklyAttendanceMessage = WeeklyAttendanceMessageMapper.Map(msg);
        
        var identifier = attendanceVote.Value;
        var dateOnly = WeeklyAttendanceMessageMapper.IdentifierToDateOnly(identifier);
        var clickerTag = $"<@{attendanceVote.UserId}>";

        var day = weeklyAttendanceMessage.Blocks.FirstOrDefault(b => b.Date == dateOnly);
        var attendingOnDay = day?.Attending ?? [];
        if (attendingOnDay.Exists(s => s == clickerTag))
        {
            attendingOnDay.Remove(clickerTag);
        }
        else
        {
            attendingOnDay.Add(clickerTag);
        }
        
        var messageUpdate = WeeklyAttendanceMessageMapper.MapMessageUpdate(
            weeklyAttendanceMessage,
            attendanceVote.ChannelId,
            attendanceVote.MessageTs);
        await slackApiClient.Chat.Update(messageUpdate);
    }
}