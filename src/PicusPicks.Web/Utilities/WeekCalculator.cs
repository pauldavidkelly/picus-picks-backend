using System;

namespace PicusPicks.Web.Utilities;

public static class WeekCalculator
{
    public static int GetCurrentWeek(DateTimeOffset date)
    {
        return date.Month switch
        {
            1 when date.Day < 19 => 19,  // Wild Card
            1 when date.Day < 26 => 20,  // Divisional
            1 => 21,  // Conference Championships
            2 when date.Day < 15 => 22,  // Super Bowl
            9 or 10 or 11 or 12 => GetRegularSeasonWeek(date),  // Regular season
            _ => 1  // Default to week 1
        };
    }

    private static int GetRegularSeasonWeek(DateTimeOffset date)
    {
        return date.Day <= 7 ? 1 : 
               date.Day <= 14 ? 2 : 
               date.Day <= 21 ? 3 : 
               date.Day <= 28 ? 4 : 5;
    }
}
