// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;

namespace Journal.Integrations;

public static class VisualCrossingWeatherApi
{
    private static readonly HttpClient client = new();
    private const string api_endpoint = "https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline";

    public static async Task<WeatherReportData.Day?> GetWeatherData(double latitude, double longitude, long date, string apiKey)
    {
        DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeMilliseconds(date);
        string formattedDate = dateTimeOffset.ToString("yyyy-MM-dd");

        var uri = $"{api_endpoint}/{latitude},{longitude}?unitGroup=metric&include=days&contentType=json&key={apiKey}";

        var message = new HttpRequestMessage(HttpMethod.Get, uri);

        var response = await client.SendAsync(message);
        var body = await response.Content.ReadAsStringAsync();
        var json = JsonDocument.Parse(body).RootElement;
        var decoded = WeatherReportData.CODEC.Decode(JsonTranscoder.INSTANCE, json);


        var today = decoded.Days.FirstOrDefault(d => d.DateTime == formattedDate);

        return today;
    }

    public record WeatherReportData(
        int QueryCost,
        string TimeZone,
        List<WeatherReportData.Day> Days
    )
    {
        public static readonly StructCodec<WeatherReportData> CODEC = StructCodec.For<WeatherReportData>()
            .Field("queryCost", Codecs.INT, d => d.QueryCost)
            .Field("timezone", Codecs.STRING, d => d.TimeZone)
            .Field("days", Day.CODEC.List(), d => d.Days)
            .Build((queryCost, timezone, days) => new WeatherReportData(queryCost, timezone, days));

        public record Day(
            string DateTime,
            long DateTimeEpoch,
            double TempMax,
            double TempMin,
            double TempAvg,
            string Conditions,
            string Description,
            string Icon
        )
        {
            public static readonly StructCodec<Day> CODEC = StructCodec.For<Day>()
                .Field("datetime", Codecs.STRING, d => d.DateTime)
                .Field("datetimeEpoch", Codecs.LONG, d => d.DateTimeEpoch)
                .Field("tempmax", Codecs.DOUBLE, d => d.TempMax)
                .Field("tempmin", Codecs.DOUBLE, d => d.TempMin)
                .Field("temp", Codecs.DOUBLE, d => d.TempAvg)
                .Field("conditions", Codecs.STRING, d => d.Conditions)
                .Field("description", Codecs.STRING, d => d.Description)
                .Field("icon", Codecs.STRING, d => d.Icon)
                .Build((datetime, epoch, tempmax, tempmin, tempavg, conditions, desc, icon) => new Day(datetime, epoch, tempmax, tempmin, tempavg, conditions, desc, icon));
        }
    };
}
