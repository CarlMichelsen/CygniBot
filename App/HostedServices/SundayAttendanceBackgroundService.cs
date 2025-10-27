using App.Configuration.Options;
using App.Slack;
using App.Slack.Mapper;
using Microsoft.Extensions.Options;
using SlackNet;

namespace App.HostedServices;

public class SundayAttendanceBackgroundService(
    ILogger<SundayAttendanceBackgroundService> logger,
    TimeProvider timeProvider,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    private static readonly TimeOnly Time = new(9, 0);
    
    private static readonly TimeSpan Interval = TimeSpan.FromMinutes(30);
    
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("{ServiceName} is running.", nameof(SundayAttendanceBackgroundService));
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                logger.LogInformation(
                    "{ServiceName} is running an interval check.",
                    nameof(SundayAttendanceBackgroundService));

                await RunJob();
            
                await Task.Delay(Interval, stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogCritical(
                    e, 
                    "An error occured while running {ServiceName}",
                    nameof(SundayAttendanceBackgroundService));
                await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
            }
        }
    }

    private async Task RunJob()
    {
        var now = timeProvider.GetLocalNow().DateTime;
        if (now.DayOfWeek != DayOfWeek.Sunday)
        {
            return;
        }

        var timeTo = Time.ToTimeSpan() - now.TimeOfDay;
        if (timeTo > TimeSpan.Zero || -timeTo > Interval)
        {
            return;
        }
        
        using var scope = serviceScopeFactory.CreateScope();
        var weeklyMessageSender = scope.ServiceProvider.GetRequiredService<WeeklyMessageFactory>();
        var slackClient = scope.ServiceProvider.GetRequiredService<ISlackApiClient>();
        var slackOptions = scope.ServiceProvider.GetRequiredService<IOptionsSnapshot<SlackOptions>>();
        var message = weeklyMessageSender.CreateMessage(
            timeProvider.GetCurrentWeek().GetNextWeek(),
            timeProvider.GetYearFromNextWeek(),
            DateHelpers.WorkDays);
        await slackClient.Chat.PostMessage(WeeklyAttendanceMessageMapper.MapMessage(message, slackOptions.Value.Channel));
    }
}