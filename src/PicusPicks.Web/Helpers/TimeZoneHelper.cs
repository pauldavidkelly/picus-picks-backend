namespace PicusPicks.Web.Helpers;

public static class TimeZoneHelper
{
    public static string FormatGameTime(DateTime gameTime)
    {
        // Convert to Eastern Time (NFL uses ET)
        var easternZone = TimeZoneInfo.FindSystemTimeZoneById("America/New_York");
        var easternTime = TimeZoneInfo.ConvertTimeFromUtc(gameTime, easternZone);
        
        return easternTime.ToString("ddd MMM d, h:mm tt ET");
    }
}
