using System.ComponentModel;
using App.Slack.Model;

namespace App.Slack;

public class WeeklyMessageFactory
{
    public WeeklyAttendanceMessage CreateMessage(int week, int year, params DayOfWeek[] days)
    {
        var blocks = days
            .OrderBy(d => (int)d)
            .Select(d => CreateDayBlock(week, year, d))
            .ToList();
        return new WeeklyAttendanceMessage
        {
            Title = $":spiral_calendar_pad: Week {week} CBY Office Attendance",
            Blocks = blocks
        };
    }

    private DayBlock CreateDayBlock(int week, int year, DayOfWeek day)
    {
        var dayTitle = Enum.GetName(day) ?? throw new InvalidEnumArgumentException();
        var date = DateHelpers.GetDateFromWeek(year, week, day);
        
        return new DayBlock
        {
            Title = $"{dayTitle} - {date.ToString("dd MMM")}",
            Date = DateHelpers.GetDateFromWeek(year, week, day),
            Attending = [],
        };
    }
}