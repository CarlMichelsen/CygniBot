using System.ComponentModel;
using App.Slack.Model.V1;

namespace App.Slack;

public class WeeklyMessageFactory
{
    public WeeklyAttendanceMessageV1 CreateMessage(int week, int year, params DayOfWeek[] days)
    {
        var blocks = days
            .OrderBy(d => (int)d)
            .Select(d => CreateDayBlock(week, year, d))
            .ToList();
        return new WeeklyAttendanceMessageV1
        {
            Title = $":spiral_calendar_pad: Week {week} CBY Office Attendance",
            Blocks = blocks,
        };
    }

    private DayBlockV1 CreateDayBlock(int week, int year, DayOfWeek day)
    {
        var dayTitle = Enum.GetName(day) ?? throw new InvalidEnumArgumentException();
        var date = DateHelpers.GetDateFromWeek(year, week, day);
        
        return new DayBlockV1
        {
            Title = $"{dayTitle} - {date.ToString("dd MMM")}",
            Date = DateHelpers.GetDateFromWeek(year, week, day),
            Attending = [],
        };
    }
}