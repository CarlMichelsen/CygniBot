using System.Threading.Channels;
using App.Queue;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace App.Slack.Handler;

public class AttendanceButtonClickHandler(Channel<ButtonQueueAction> channel) : IBlockActionHandler<ButtonAction>
{
    public const string ActionId = "attending_click";
    
    public async Task Handle(ButtonAction action, BlockActionRequest request) => await channel.Writer.WriteAsync(new ButtonQueueAction(action, request));
    /*{
        logger.LogInformation("Hello {ActionValue}", action.Value);
        var attendanceDayKey = action.Value.Split('|').First();
        var blockId = $"{attendanceDayKey}|block";
        
        logger.LogInformation("Block ID: {blockId} {DayKey}", blockId, attendanceDayKey);
        
        var blocks = request.Message.Blocks.ToList();
        
        logger.LogInformation("markdown blocks\n{BlockId}", string.Join('\n', blocks.Select(b => b.BlockId)));
        
        var markdownBlock = blocks
            .OfType<SectionBlock>()
            .FirstOrDefault(b => b.BlockId == blockId);;
        if (markdownBlock is null)
        {
            logger.LogWarning("No block found for {BlockId}", blockId);
            return;
        }
        
        var newMarkdownBlock = new SectionBlock
        {
            BlockId = blockId,
            Text = new Markdown("yeehaw"),
        };
        
        var index = blocks.IndexOf(markdownBlock);
        blocks[index] = newMarkdownBlock;


        var updatedMessage = new FixedMessageUpdate
        {
            ChannelId = request.Message.Channel,
            Ts = request.Message.Ts,
            Blocks = blocks
        };
        slackApiClient.Post("", new Dictionary<string, object>
        {
            ["channel"] = updatedMessage.ChannelId,
            ["ts"] = updatedMessage.Ts,
            ["blocks"] = updatedMessage.Blocks,
        });
        
        logger.LogInformation("Updated message for {Day}", attendanceDayKey);
    }

    private class FixedMessageUpdate : MessageUpdate
    {
        [JsonPropertyName("channel")]
        public string Channel => ChannelId;
    }*/
}