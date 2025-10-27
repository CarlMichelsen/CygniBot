using System.Globalization;

namespace App;

public static class DateHelpers
{
    public static string GetIdentifier(this DateOnly date) => date
        .ToString("yyyy-MM-dd")
        .Replace('-', '.');
    
    public static int GetCurrentWeek(this TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow().DateTime;
        return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(
            now,
            CalendarWeekRule.FirstFourDayWeek,
            DayOfWeek.Monday);
    }

    public static int GetCurrentYear(this TimeProvider timeProvider) => timeProvider.GetUtcNow().Year;
    
    public static int GetYearFromNextWeek(this TimeProvider timeProvider)
    {
        var now = timeProvider.GetUtcNow().DateTime;
        var calendar = CultureInfo.InvariantCulture.Calendar;
        var currentWeek = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        var nextWeek = currentWeek % 52 + 1;
    
        return nextWeek == 1 ? now.Year + 1 : now.Year;
    }

    public static List<DateOnly> GetDaysWeek(this TimeProvider timeProvider, int forWeek, params DayOfWeek[] days)
    {
        var now = timeProvider.GetUtcNow().DateTime;
        var year = now.Year;
        var calendar = CultureInfo.InvariantCulture.Calendar;
        
        // Adjust year for edge cases
        var currentWeek = calendar.GetWeekOfYear(now, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
        switch (currentWeek)
        {
            case >= 52 when now.Month == 1:
                year--;
                break;
            case 1 when now.Month == 12:
                year++;
                break;
        }
        
        // Start from Jan 4 (always in week 1 by ISO 8601), then find Monday of that week
        var jan4 = new DateTime(year, 1, 4);
        var daysToMonday = ((int)DayOfWeek.Monday - (int)jan4.DayOfWeek + 7) % 7;
        var firstMondayOfYear = jan4.AddDays(-daysToMonday);
        var firstDayOfTargetWeek = firstMondayOfYear.AddDays((forWeek - 1) * 7);
        
        var result = new List<DateOnly>();
        
        for (var i = 0; i < 7; i++)
        {
            var date = firstDayOfTargetWeek.AddDays(i);
            if (days.Length == 0 || days.Contains(date.DayOfWeek))
            {
                result.Add(DateOnly.FromDateTime(date));
            }
        }
        
        return result;
    }
    
    public static DateOnly GetDateFromWeek(int year, int week, DayOfWeek dayOfWeek)
    {
        var jan4 = new DateTime(year, 1, 4);
        var firstMondayOfYear = jan4.AddDays(-((int)jan4.DayOfWeek + 6) % 7);
        var firstDayOfTargetWeek = firstMondayOfYear.AddDays((week - 1) * 7);
        var targetDate = firstDayOfTargetWeek.AddDays(((int)dayOfWeek + 6) % 7);
        return DateOnly.FromDateTime(targetDate);
    }

    public static int GetNextWeek(this int week) => week % 52 + 1;

    public static DayOfWeek[] WorkDays => [DayOfWeek.Monday, DayOfWeek.Tuesday, DayOfWeek.Wednesday, DayOfWeek.Thursday, DayOfWeek.Friday];
}