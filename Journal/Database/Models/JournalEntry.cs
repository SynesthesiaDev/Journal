using Codon.Codec;
using Journal.Util;
using Realms;
using SynesthesiaUtil.Extensions;

namespace Journal.Database.Models;

public partial class JournalEntry : RealmObject
{
    [PrimaryKey]
    public Guid Id { get; set; } = Guid.Empty;

    public long OwnerDiscordId { get; set; } = 0L;
    public long Time { get; set; } = 0L;
    public string Title { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;

    public int Sunrise { get; set; } = 25200; // 7AM default

    public int Sunset { get; set; } = 73800; // 8:30PM default

    public int WeatherId { get; set; } = 0;

    [Ignored]
    public Weather Weather
    {
        get => (Weather)WeatherId;
        init => value.Ordinal();
    }

    public MentalHealthTrackerEntry MentalHealthTrackerEntry { get; set; } = new();

    [Ignored]
    public double SunlightHours => TimeWeatherUtils.CalculateSunlightHours(Sunrise, Sunset, MentalHealthTrackerEntry.SleepStart, MentalHealthTrackerEntry.SleepEnd).Truncate(1);

    // realm needs
    public JournalEntry()
    {
    }

    public JournalEntry(Guid id, long ownerDiscordId, long time, string title, string text, int sunrise, int sunset, Weather weather, MentalHealthTrackerEntry mentalHealthTrackerEntry)
    {
        Id = id;
        OwnerDiscordId = ownerDiscordId;
        Time = time;
        Title = title;
        Text = text;
        MentalHealthTrackerEntry = mentalHealthTrackerEntry;
        Sunrise = sunrise;
        Sunset = sunset;
        Weather = weather;
    }

    public static readonly StructCodec<JournalEntry> CODEC = StructCodec.Of
    (
        "id", Codecs.STRING.Transform(Guid.Parse, g => g.ToString()), e => e.Id,
        "owner_discord_id", Codecs.LONG, e => e.OwnerDiscordId,
        "time", Codecs.LONG, e => e.Time,
        "title", Codecs.STRING, e => e.Title,
        "text", Codecs.STRING, e => e.Text,
        "sunrise", Codecs.INT, e => e.Sunrise,
        "sunset", Codecs.INT, e => e.Sunset,
        "weather", Codecs.Enum<Weather>(), e => e.Weather,
        "mental_health_tracker_entry", MentalHealthTrackerEntry.CODEC, e => e.MentalHealthTrackerEntry,
        (id, owner, time, title, text, sunrise, sunset, weather, mental) => new JournalEntry(id, owner, time, title, text, sunrise, sunset, weather, mental)
    );
}
