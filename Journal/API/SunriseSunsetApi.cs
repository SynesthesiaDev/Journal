// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;
using Microsoft.AspNetCore.WebUtilities;

namespace Journal.API;

public static class SunriseSunsetApi
{
    private const string api_endpoint = "https://api.sunrise-sunset.org/json";

    public static async Task<Tuple<int, int>> GetSunriseAndSunset(double latitude, double longitude, long date)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(date);
        string formattedDate = dateTimeOffset.ToString("yyyy-MM-dd");

        var query = new Dictionary<string, string?>
        {
            ["lat"] = $"{latitude}",
            ["lng"] = $"{longitude}",
            ["date"] = formattedDate
        };

        var client = new HttpClient();
        var uri = QueryHelpers.AddQueryString(api_endpoint, query);
        var message = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await client.SendAsync(message);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body).RootElement;
        var decoded = SunriseSunsetData.CODEC.Decode(JsonTranscoder.INSTANCE, json);

        Console.WriteLine(decoded.ToString());
        return new Tuple<int, int>(getSecondsSinceMidnight(decoded.Results.Sunrise), getSecondsSinceMidnight(decoded.Results.Sunset));
    }

    private static int getSecondsSinceMidnight(string time)
    {
        DateTime sunriseParsed = DateTime.Parse(time);
        TimeSpan timeSinceMidnight = sunriseParsed.TimeOfDay;
        var totalSeconds = timeSinceMidnight.TotalSeconds;
        return (int)totalSeconds;
    }

    public record SunriseSunsetData(
        SunriseSunsetData.TimeData Results,
        string Status,
        string TimezoneId
    )
    {
        public static readonly StructCodec<SunriseSunsetData> CODEC = StructCodec.Of
        (
            "results", TimeData.CODEC, s => s.Results,
            "status", Codecs.STRING, s => s.Status,
            "tzid", Codecs.STRING, s => s.TimezoneId,
            (results, status, tzid) => new SunriseSunsetData(results, status, tzid)
        );

        public record TimeData(
            string Sunrise,
            string Sunset,
            string SolarNoon,
            string DayLength,
            string CivilTwilightBegin,
            string CivilTwilightEnd,
            string NauticalTwilightBegin,
            string NauticalTwilightEnd,
            string AstronomicalTwilightBegin,
            string AstronomicalTwilightEnd
        )
        {
            public static readonly StructCodec<TimeData> CODEC = StructCodec.Of
            (
                "sunrise", Codecs.STRING, t => t.Sunrise,
                "sunset", Codecs.STRING, t => t.Sunset,
                "solar_noon", Codecs.STRING, t => t.SolarNoon,
                "day_length", Codecs.STRING, t => t.DayLength,
                "civil_twilight_begin", Codecs.STRING, t => t.CivilTwilightBegin,
                "civil_twilight_end", Codecs.STRING, t => t.CivilTwilightEnd,
                "nautical_twilight_begin", Codecs.STRING, t => t.NauticalTwilightBegin,
                "nautical_twilight_end", Codecs.STRING, t => t.NauticalTwilightEnd,
                "astronomical_twilight_begin", Codecs.STRING, t => t.AstronomicalTwilightBegin,
                "astronomical_twilight_end", Codecs.STRING, t => t.AstronomicalTwilightEnd,
                (sunrise, sunset, noon, len, ctb, cte, ntb, nte, atb, ate) => new TimeData(sunrise, sunset, noon, len, ctb, cte, ntb, nte, atb, ate)
            );
        }
    }
}
