using SlackNet.Blocks;
using SlackNet.Interaction;

namespace App.Slack.Handler;

public class AttendanceButtonClickHandler : IBlockActionHandler<ButtonAction>
{
    public Task Handle(ButtonAction action, BlockActionRequest request)
    {
        throw new NotImplementedException();
    }
}