namespace App.Slack.Model;

public class WeeklyAttendanceMessage
{
    public required string Title { get; set; }

    public required List<DayBlock> Blocks { get; init; }
}