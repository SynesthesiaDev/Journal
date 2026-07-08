using Codon.Binary;
using Codon.Codec;
using Journal.Util;
using Nocturne.Database.API;
using Nocturne.Database.Migrations;

namespace Journal.Database.Models;

public record JournalEntry(
    Guid Id,
    long OwnerDiscordId,
    long Time,
    string Title,
    string Text,
    int Sunrise,
    int Sunset,
    Weather Weather,
    MentalHealthTrackerEntry MentalHealthTrackerEntry
)
{
    public static readonly Codec<JournalEntry> CODEC = StructCodec.For<JournalEntry>()
        .Field("Id", ExtraCodecs.GUID_CODEC, e => e.Id)
        .Field("OwnerDiscordId", Codecs.LONG, e => e.OwnerDiscordId)
        .Field("Time", Codecs.LONG, e => e.Time)
        .Field("Title", Codecs.STRING, e => e.Title)
        .Field("Text", Codecs.STRING, e => e.Text)
        .Field("Sunrise", Codecs.INT, e => e.Sunrise)
        .Field("Sunset", Codecs.INT, e => e.Sunset)
        .Field("Weather", Codecs.Enum<Weather>(), e => e.Weather)
        .Field("MentalHealthTrackerEntry", MentalHealthTrackerEntry.CODEC, e => e.MentalHealthTrackerEntry)
        .Build((guid, l, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => new JournalEntry(guid, l, arg3, arg4, arg5, arg6, arg7, arg8, arg9));

    public static readonly IBinaryCodec<JournalEntry> BINARY_CODEC = BinaryCodecs.For<JournalEntry>()
        .Field(BinaryCodecs.GUID, e => e.Id)
        .Field(BinaryCodecs.LONG, e => e.OwnerDiscordId)
        .Field(BinaryCodecs.LONG, e => e.Time)
        .Field(BinaryCodecs.STRING, e => e.Title)
        .Field(BinaryCodecs.STRING, e => e.Text)
        .Field(BinaryCodecs.INT, e => e.Sunrise)
        .Field(BinaryCodecs.INT, e => e.Sunset)
        .Field(BinaryCodecs.Enum<Weather>(), e => e.Weather)
        .Field(MentalHealthTrackerEntry.BINARY_CODEC, e => e.MentalHealthTrackerEntry)
        .Build((guid, l, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => new JournalEntry(guid, l, arg3, arg4, arg5, arg6, arg7, arg8, arg9));

    public static readonly NocturneCollection<Guid, JournalEntry> DB_COLLECTION = JournalApp.NOCTURNE_DATABASE.For
    (
        "journal_entries",
        0,
        KeySerializers.GUID,
        NocturneSerializer.FromCodec(BINARY_CODEC),
        IMigrationStrategy.Migrations().Build()
    );

    public double SunlightHours => TimeWeatherUtils.CalculateSunlightHours(Sunrise, Sunset, MentalHealthTrackerEntry.SleepStart, MentalHealthTrackerEntry.SleepEnd).Truncate(1);
}
