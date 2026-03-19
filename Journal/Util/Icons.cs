using Journal.Database.Models;

namespace Journal.Util;

public static class Icons
{
    public const string UNKNOWN = "question_mark";

    public const string HOME = "home";
    public const string BEDTIME = "bedtime";
    public const string SUNRISE = "backlight_high";
    public const string WEATHER = "cloud";
    public const string NAV_BAR_CLOSE = "menu_open";
    public const string NAV_BAR_OPEN = "menu";
    public const string ADD = "add";
    public const string PERSON = "person";
    public const string LOGIN = "login";
    public const string JOURNAL = "collections_bookmark";
    public const string GRAPH = "bar_chart";
    public const string EXPORT = "upload";
    public const string SETTINGS = "settings";
    public const string DELETE = "delete";
    public const string EDIT = "edit";
    public const string CLOSE = "close";
    public const string RATING = "kid_star";
    public const string PRODUCTIVITY_RATING = "checklist";
    public const string MOOD_RATING = "psychiatry";
    public const string HEALTH_STEPS = "steps";
    public const string HEALTH_HEART_PTS_GOOD = "heart_check";
    public const string HEALTH_HEART_PTS_BAD = "heart_minus";
    public const string HEALTH_MAP = "map";
    public const string HEALTH_CALORIES = "mode_heat";


    public static string GetHeartPtsIcon(double heartPts)
    {
        return heartPts >= 20.0 ? HEALTH_HEART_PTS_GOOD : HEALTH_HEART_PTS_BAD;
    }

    public static string GetForWeatherType(Weather weather)
    {
        return weather switch
        {
            Weather.Cloudy => "cloud",
            Weather.Unknown => UNKNOWN,
            Weather.Sunny => "sunny",
            Weather.PartlyCloudy => "partly_cloudy_day",
            Weather.Rainy => "rainy",
            Weather.Snowy => "weather_snowy",
            Weather.Sleet or Weather.Hail => "weather_hail",
            Weather.Stormy => "thunderstorm",
            Weather.Windy => "air",
            Weather.Foggy => "foggy",
            _ => UNKNOWN
        };
    }
}

