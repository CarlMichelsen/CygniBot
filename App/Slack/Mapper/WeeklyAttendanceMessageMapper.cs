using App.Slack.Handler;
using App.Slack.Model;
using SlackNet.Blocks;
using SlackNet.Events;
using SlackNet.WebApi;
using Button = SlackNet.Blocks.Button;

namespace App.Slack.Mapper;

public static class WeeklyAttendanceMessageMapper
{
    public const char AttendanceDelimiter = ',';
    
    public const string NoAttendanceString = "-";
    
    public static DateOnly IdentifierToDateOnly(string identifierString)
    {
        var identifierDateArr = identifierString.Split('.');
        return new DateOnly(
            year: int.Parse(identifierDateArr[0]),
            month: int.Parse(identifierDateArr[1]),
            day: int.Parse(identifierDateArr[2]));
    }
    
    public static WeeklyAttendanceMessage Map(MessageEvent messageEvent)
    {
        var blocks = messageEvent.Blocks ?? throw new NullReferenceException("Unable to find request message blocks");

        var titleBlock = blocks.OfType<SectionBlock>().First(b => b.BlockId == nameof(WeeklyAttendanceMessage.Title));
        var actionsBlocks = blocks
            .OfType<ActionsBlock>()
            .Where(b => b.BlockId.StartsWith("actions-"))
            .ToList();
        
        var dayTitleBlocks = blocks
            .OfType<SectionBlock>()
            .Where(b => b.BlockId.StartsWith("title-"))
            .ToList();
        
        var attendanceBlocks = blocks
            .OfType<SectionBlock>()
            .Where(b => b.BlockId.StartsWith("attending-"))
            .ToList();
        
        var days = new List<DayBlock>();
        foreach (var actionBlock in actionsBlocks)
        {
            var button = actionBlock.Elements.OfType<Button>().First();
            var identifier = button.Value;
            var title = dayTitleBlocks.First(b => b.BlockId.EndsWith(identifier)).Text.Text!;
            var attendingString = attendanceBlocks.FirstOrDefault(b => b.BlockId.EndsWith(identifier))?.Text.Text;
            var attending = attendingString == NoAttendanceString
                ? []
                : attendingString?.Split(AttendanceDelimiter).ToList() ?? [];
            
            days.Add(new DayBlock
            {
                Title = title,
                Attending = attending,
                Date = IdentifierToDateOnly(identifier)
            });
        }
        
        return new WeeklyAttendanceMessage
        {
            Title = titleBlock.Text.Text,
            Blocks = days
        };
    }
    
    public static MessageUpdate MapMessageUpdate(WeeklyAttendanceMessage weeklyAttendanceMessage, string channelId, string ts)
    {
        var message = MapMessage(weeklyAttendanceMessage, channelId);
        return new MessageUpdate
        {
            ChannelId = message.Channel,
            Ts = ts,
            Blocks = message.Blocks,
        };
    }
    
    public static Message MapMessage(WeeklyAttendanceMessage weeklyAttendanceMessage, string channel)
    {
        var msg = new Message
        {
            Channel = channel,
            Blocks =
            [
                CreateTitleBlock(weeklyAttendanceMessage),
                ..weeklyAttendanceMessage.Blocks.SelectMany(CreateDayBlock)
            ]
        };

        return msg;
    }

    private static Block CreateTitleBlock(WeeklyAttendanceMessage weeklyAttendanceMessage)
    {
        return new SectionBlock
        {
            BlockId = nameof(WeeklyAttendanceMessage.Title),
            Text = weeklyAttendanceMessage.Title,
        };
    }
    
    private static List<Block> CreateDayBlock(DayBlock dayBlock)
    {
        var identifier = dayBlock.Date.GetIdentifier();
        List<Block> list =
        [
            new SectionBlock
            {
                BlockId = $"title-{identifier}",
                Text = new Markdown(dayBlock.Title),
            },
            new ActionsBlock
            {
                BlockId = $"actions-{identifier}",
                Elements = [
                    new Button
                    {
                        ActionId = AttendanceButtonClickHandler.ActionId,
                        Text = "️✅ Attending",
                        Value = identifier,
                        Style = ButtonStyle.Primary,
                    },
                ]
            },
        ];

        var attendingText = string.Join(AttendanceDelimiter, dayBlock.Attending);
        list.Add(new SectionBlock
        {
            BlockId = $"attending-{identifier}",
            Text = string.IsNullOrWhiteSpace(attendingText) ? NoAttendanceString : new Markdown(attendingText)
        });
        
        list.Add(new DividerBlock());
        
        return list;
    }
}