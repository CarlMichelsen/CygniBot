using SlackNet.Blocks;
using SlackNet.Interaction;

namespace App.Slack.Handler;

public class AttendanceButtonClickHandler(
    ILogger<AttendanceButtonClickHandler> logger) : IBlockActionHandler<ButtonAction>
{
    public const string ActionId = "attending_click";
    
    public Task Handle(ButtonAction action, BlockActionRequest request)
    {
        logger.LogInformation("Hello {ActionValue}", action.Value);
        return Task.CompletedTask;
    }
}