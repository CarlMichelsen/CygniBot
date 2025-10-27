using SlackNet.Blocks;
using SlackNet.Interaction;

namespace App.Queue;

public record ButtonQueueAction(
    ButtonAction Action,
    BlockActionRequest Request);