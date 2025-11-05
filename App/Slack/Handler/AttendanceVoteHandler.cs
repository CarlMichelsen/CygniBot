using System.Threading.Channels;
using App.Queue;
using SlackNet.Blocks;
using SlackNet.Interaction;

namespace App.Slack.Handler;

public class AttendanceVoteHandler(
    Channel<AttendanceVote> channel) : IBlockActionHandler<ButtonAction>
{
    public const string ActionId = "attending_click";

    public async Task Handle(ButtonAction action, BlockActionRequest request)
    {
        var vote = new AttendanceVote(
            request.User.Id,
            request.Channel.Id, 
            request.Message.Ts,
            action.Value);
        await channel.Writer.WriteAsync(vote);
    }
}