using Journal.Database.Models;

namespace Journal.Util;

public static class TimeWeatherUtils
{
    public static Weather MapToWeatherType(string iconId)
    {
        if (string.IsNullOrWhiteSpace(iconId)) return Weather.Unknown;
        var key = iconId.Replace("-day", "").Replace("-night", "").ToLower();

        return key switch
        {
            "clear" => Weather.Sunny,
            "partly-cloudy" => Weather.PartlyCloudy,
            "cloudy" => Weather.Cloudy,
            "rain" or "showers" => Weather.Rainy,
            "snow" or "snow-showers" => Weather.Snowy,
            "rain-snow" or "rain-snow-showers" or "sleet" => Weather.Sleet,
            "hail" => Weather.Hail,
            "thunder" or "thunder-rain" or "thunder-showers" => Weather.Stormy,
            "wind" => Weather.Windy,
            "fog" => Weather.Foggy,
            _ => Weather.Unknown
        };
    }

    public static int GetAwakeHours(double sleepEndPreviousDay, double sleepStartToday)
    {
        if (sleepStartToday < sleepEndPreviousDay)
        {
            sleepStartToday += 24;
        }

        double awakeDuration = sleepStartToday - sleepEndPreviousDay;

        return (int)awakeDuration;
    }

    public static double CalculateSunlightHours(int sunriseSec, int sunsetSec, double sleepStartTonight, double sleepEndThisMorning)
    {
        double sunrise = sunriseSec / 3600.0;
        double sunset = sunsetSec / 3600.0;

        double awakeStart = sleepEndThisMorning;
        double awakeEnd = sleepStartTonight;

        if (awakeEnd <= awakeStart)
        {
            awakeEnd += 24;
        }

        double exposureStart = Math.Max(awakeStart, sunrise);
        double exposureEnd = Math.Min(awakeEnd, sunset);

        return Math.Max(0, exposureEnd - exposureStart);
    }

    public static string ToProperlyFormatted(this DateTimeOffset dateTimeOffset, bool includeTime = false)
    {
        return GetProperFormattedDate(dateTimeOffset, includeTime);
    }

    public static string GetProperFormattedDate(DateTimeOffset dateTimeOffset, bool includeTime = false)
    {
        var day = dateTimeOffset.Day;

        var time = includeTime ? $" {dateTimeOffset:h:mm tt}" : "";
        return $"{day}{GetDaySuffix(day)} of {dateTimeOffset:MMMM yyyy}{time}";
    }

    public static string GetDaySuffix(int day)
    {
        if (day is >= 11 and <= 13) return "th";
        return (day % 10) switch
        {
            1 => "st",
            2 => "nd",
            3 => "rd",
            _ => "th"
        };
    }

    public static double Truncate(this double value, int precision)
    {
        double step = Math.Pow(10, precision);
        return Math.Truncate(step * value) / step;
    }
}
