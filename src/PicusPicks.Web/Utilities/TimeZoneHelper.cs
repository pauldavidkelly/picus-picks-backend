using System;

namespace PicusPicks.Web.Utilities;

public static class TimeZoneHelper
{
    public static string FormatGameTime(DateTimeOffset gameTime)
    {
        // Just use the time format without hardcoding the timezone
        // The timezone will be handled by the browser
        return gameTime.ToLocalTime().ToString("h:mm tt");
    }

    public static string FormatGameDate(DateTimeOffset gameTime)
    {
        return gameTime.ToLocalTime().ToString("MMM d, yyyy");
    }

    public static string FormatGameDateTime(DateTimeOffset gameTime)
    {
        return gameTime.ToLocalTime().ToString("MMM d, yyyy h:mm tt");
    }
}
