using System.Threading.Channels;
using App.Queue;
using App.Slack.Mapper;
using SlackNet;
using SlackNet.Events;

namespace App.HostedServices;

public class ButtonQueueActionBackgroundService(
    ILogger<ButtonQueueActionBackgroundService> logger,
    Channel<ButtonQueueAction> channel,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    // TODO: Move business-logic into another class
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var buttonQueueAction = await channel.Reader.ReadAsync(stoppingToken);
                var slackClient = scope.ServiceProvider.GetRequiredService<ISlackApiClient>();
                
                // Re-fetching the message in order to guarantee that it has the latest content
                MessageEvent? historyMessage = null;
                try
                {
                    var historyResponse = await slackClient.Conversations.History(
                        channelId: buttonQueueAction.Request.Channel.Id,
                        latestTs: buttonQueueAction.Request.Message.Ts,
                        inclusive: true,
                        limit: 1,
                        cancellationToken: stoppingToken);
                    historyMessage = historyResponse.Messages.FirstOrDefault();
                }
                catch (Exception e)
                {
                    logger.LogWarning(
                        e, 
                        "Slackbot may not have been invited to {Channel} - using potentially outdated messagecontent",
                        buttonQueueAction.Request.Channel.Name);
                }
                
                var msg = historyMessage ?? buttonQueueAction.Request.Message;
                var weeklyAttendanceMessage = WeeklyAttendanceMessageMapper.Map(msg);
                
                var identifier = buttonQueueAction.Action.Value;
                var dateOnly = WeeklyAttendanceMessageMapper.IdentifierToDateOnly(identifier);
                var clickerTag = $"<@{buttonQueueAction.Request.User.Id}>";

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
                    buttonQueueAction.Request.Channel.Id,
                    buttonQueueAction.Request.Message.Ts);
                await slackClient.Chat.Update(messageUpdate, stoppingToken);
                
                // Avoid spamming slack
                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"Unhandled exception in {nameof(ButtonQueueActionBackgroundService)}");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Wait a little bit before continuing after error
            }
        }
    }
}