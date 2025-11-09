namespace App.Slack.Model.V1;

public class WeeklyAttendanceMessageV1 : BaseWeeklyAttendanceMessage
{
    public override string Version { get; } = "1";
    
    public required string Title { get; set; }

    public required List<DayBlockV1> Blocks { get; init; }
    
    protected override BaseWeeklyAttendanceMessage? ToNextVersion() => null;
}