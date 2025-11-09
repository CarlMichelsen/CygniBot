using System.Text;
using System.Text.Json;
using App.Slack.Handler;
using App.Slack.Model;
using App.Slack.Model.V1;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.WebApi;

namespace App.Slack.Mapper;

public static class WeeklyAttendanceMessageMapper
{
    public const char AttendanceDelimiter = ',';
    
    public const string NoAttendanceString = "-";

    public static BaseWeeklyAttendanceMessage GetMessageFromMetadata(MessageEvent messageEvent)
    {
        var metadataJson = Encoding.UTF8.GetString(Convert.FromBase64String(messageEvent.Text));

        var deserializedBaseWeeklyAttendanceMessage = JsonSerializer
            .Deserialize<BaseWeeklyAttendanceMessage>(metadataJson)!;
        ArgumentNullException.ThrowIfNull(deserializedBaseWeeklyAttendanceMessage);
        
        var message = deserializedBaseWeeklyAttendanceMessage.ToLatestVersion();
        ArgumentNullException.ThrowIfNull(message);

        return message;
    }
    
    public static Message MapMessage(
        BaseWeeklyAttendanceMessage baseWeeklyAttendanceMessage,
        string channel)
    {
        var message = baseWeeklyAttendanceMessage.ToLatestVersion();
        ArgumentNullException.ThrowIfNull(message);
        
        var msg = new Message
        {
            Channel = channel,
            Text = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize<BaseWeeklyAttendanceMessage>(message))),
            Blocks =
            [
                CreateTitleBlock(message),
                ..message.Blocks.SelectMany(CreateDayBlock)
            ]
        };

        return msg;
    }
    
    private static Block CreateTitleBlock(WeeklyAttendanceMessageV1 baseWeeklyAttendanceMessage)
    {
        return new SectionBlock
        {
            BlockId = nameof(WeeklyAttendanceMessageV1.Title),
            Text = baseWeeklyAttendanceMessage.Title,
        };
    }
    
    private static List<Block> CreateDayBlock(DayBlockV1 baseDayBlock)
    {
        var identifier = baseDayBlock.Date.GetIdentifier();
        List<Block> list =
        [
            new SectionBlock
            {
                BlockId = $"title-{identifier}",
                Text = new Markdown(baseDayBlock.Title),
            },
            new ActionsBlock
            {
                BlockId = $"actions-{identifier}",
                Elements = [
                    new Button
                    {
                        ActionId = AttendanceVoteHandler.ActionId,
                        Text = "️✅ Attending",
                        Value = identifier,
                        Style = ButtonStyle.Primary,
                    },
                ]
            },
        ];

        var attendingText = string.Join(AttendanceDelimiter, baseDayBlock.Attending.Select(userId => $"<@{userId}>"));
        list.Add(new SectionBlock
        {
            BlockId = $"attending-{identifier}",
            Text = string.IsNullOrWhiteSpace(attendingText) ? NoAttendanceString : new Markdown(attendingText)
        });
        
        list.Add(new DividerBlock());
        
        return list;
    }
}