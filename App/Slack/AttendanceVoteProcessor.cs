using App.Configuration.Options;
using App.Queue;
using App.Slack.Mapper;
using Microsoft.Extensions.Options;
using SlackNet;
using SlackNet.WebApi;

namespace App.Slack;

public class AttendanceVoteProcessor(
    ILogger<AttendanceVoteProcessor> logger,
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

        var userInfo = await slackApiClient.Users.Info(attendanceVote.UserId)
            ?? throw new Exception($"Unable to get user info for voting user '{attendanceVote.UserId}'");

        var isAttending = block.Attending.Find(a => a == userInfo.Id);
        if (isAttending is null)
        {
            block.Attending.Add(userInfo.Id);
            logger.LogInformation(
                "'{Username}' <{UserId}> voted to attend on {DateOnly}",
                userInfo.Name,
                userInfo.Id,
                attendanceVote.Value);
        }
        else
        {
            block.Attending.Remove(isAttending);
            logger.LogInformation(
                "'{Username}' <{UserId}> removed their vote to attend on {DateOnly}",
                userInfo.Name,
                userInfo.Id,
                attendanceVote.Value);
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