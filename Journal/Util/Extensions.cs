using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Journal.Util;

public static class Extensions
{
    public static DateTimeOffset ToDateTime(this long time)
    {
        return DateTimeOffset.FromUnixTimeMilliseconds(time);
    }

    public static string GetDisplayName(this Enum enumValue)
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<DisplayAttribute>()
            ?.Name ?? enumValue.ToString();
    }

    public static List<T> Reversed<T>(this List<T> list)
    {
        var inner = list.ToList();
        inner.Reverse();
        return inner;
    }

    public static IEnumerable<(T Value, string Name)> GetValuesWithDisplayNames<T>(Type enumType) where T : Enum
    {
        return Enum.GetValues(enumType)
            .Cast<T>()
            .Select(e => (Value: e, Name: e.GetDisplayName()));
    }

    public static IEnumerable<(T Value, string Name)> GetValuesWithDisplayNames<T>() where T : Enum
    {
        return GetValuesWithDisplayNames<T>(typeof(T));
    }
}