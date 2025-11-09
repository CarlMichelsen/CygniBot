using System.Threading.Channels;
using App.Queue;
using App.Slack;

namespace App.HostedServices;

public class AttendanceVoteQueueBackgroundService(
    ILogger<AttendanceVoteQueueBackgroundService> logger,
    Channel<AttendanceVote> channel,
    IServiceScopeFactory serviceScopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (await channel.Reader.WaitToReadAsync(stoppingToken))
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var processor = scope.ServiceProvider.GetRequiredService<AttendanceVoteProcessor>();
                var attendanceVote = await channel.Reader.ReadAsync(stoppingToken);
                await processor.Process(attendanceVote);
                
                // Avoid spamming slack
                await Task.Delay(TimeSpan.FromMilliseconds(100), stoppingToken);
            }
            catch (Exception e)
            {
                logger.LogCritical(e, $"Unhandled exception in {nameof(AttendanceVoteQueueBackgroundService)}");
                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken); // Wait a little bit before continuing after error
            }
        }
    }
}