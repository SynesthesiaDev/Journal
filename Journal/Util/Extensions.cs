using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Security.Cryptography;
using Codon.Optionals;

namespace Journal.Util;

public static class Extensions
{
    public static async Task<string> GetBodyString(this HttpRequest request)
    {
        var bodyStream = new StreamReader(request.Body);
        var bodyText = await bodyStream.ReadToEndAsync();
        return bodyText;
    }

    public static string GenerateSecureRandomToken(int size = 32)
    {
        var randomNumber = new byte[size];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomNumber);
        }
        return Convert.ToBase64String(randomNumber);
    }

    public static async Task WriteTextAsync(this HttpContext context, string text, int statusCode = 200)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "text/plain";

        // Get the memory from the BodyWriter
        var bytes = System.Text.Encoding.UTF8.GetBytes(text);
        await context.Response.BodyWriter.WriteAsync(bytes);
    }

    public static Optional<T> OfOrEmptyClass<T>(T? value) where T : class
    {
        return value == null ? Optional.Empty<T>() : Optional.Of(value);
    }

    public static Optional<T> OfOrEmptyStruct<T>(T? value) where T : struct
    {
        return value == null ? Optional.Empty<T>() : Optional.Of(value.Value);
    }

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
