using Codon.Binary;
using Codon.Codec;
using Journal.Util;
using Nocturne.Database.API;
using Nocturne.Database.Migrations;

namespace Journal.Database.Models;

public record JournalUser(
    long DiscordId,
    string Username,
    string DisplayName,
    string ProfileImageUrl,
    bool IsAdmin,
    string? MobileAuthToken,
    string? MobileSyncCode,
    Dictionary<string, HealthSync> HealthSync,
    List<Guid> JournalEntries
)
{
    public static readonly Codec<JournalUser> CODEC = StructCodec.For<JournalUser>()
        .Field("DiscordId", Codecs.LONG, u => u.DiscordId)
        .Field("Username", Codecs.STRING, u => u.Username)
        .Field("DisplayName", Codecs.STRING, u => u.DisplayName)
        .Field("ProfileImageUrl", Codecs.STRING, u => u.ProfileImageUrl)
        .Field("IsAdmin", Codecs.BOOLEAN, u => u.IsAdmin)
        .Field("MobileAuthToken", Codecs.STRING.Optional(), u => ExtraCodecs.OfOrEmptyClass(u.MobileAuthToken))
        .Field("MobileSyncCode", Codecs.STRING.Optional(), u => ExtraCodecs.OfOrEmptyClass(u.MobileSyncCode))
        .Field("HealthSync", Codecs.STRING.MapTo(Models.HealthSync.CODEC), u => u.HealthSync)
        .Field("JournalEntries", ExtraCodecs.GUID_CODEC.List(), u => u.JournalEntries)
        .Build((l, s, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => new JournalUser(l, s, arg3, arg4, arg5, arg6.Value, arg7.Value, arg8, arg9));

    public static readonly IBinaryCodec<JournalUser> BINARY_CODEC = BinaryCodecs.For<JournalUser>()
        .Field(BinaryCodecs.LONG, u => u.DiscordId)
        .Field(BinaryCodecs.STRING, u => u.Username)
        .Field(BinaryCodecs.STRING, u => u.DisplayName)
        .Field(BinaryCodecs.STRING, u => u.ProfileImageUrl)
        .Field(BinaryCodecs.BOOLEAN, u => u.IsAdmin)
        .Field(BinaryCodecs.STRING.Optional(), u => ExtraCodecs.OfOrEmptyClass(u.MobileAuthToken))
        .Field(BinaryCodecs.STRING.Optional(), u => ExtraCodecs.OfOrEmptyClass(u.MobileSyncCode))
        .Field(BinaryCodecs.STRING.MapTo(Models.HealthSync.BINARY_CODEC), u => u.HealthSync)
        .Field(BinaryCodecs.GUID.List(), u => u.JournalEntries)
        .Build((l, s, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => new JournalUser(l, s, arg3, arg4, arg5, arg6.Value, arg7.Value, arg8, arg9));

    private static readonly INocturneSerializer<JournalUser> database_serializer = NocturneSerializer.FromCodec(BINARY_CODEC);

    public static readonly NocturneCollection<long, JournalUser> DB_COLLECTION = JournalApp.NOCTURNE_DATABASE.For
    (
        collectionKey: "users",
        schemaVersion: 0,
        keySerializer: KeySerializers.LONG,
        valueSerializer: database_serializer,
        migrationStrategy: IMigrationStrategy.Migrations().Build()
    );

    public List<JournalEntry> ResolveJournalEntries() => JournalEntry.DB_COLLECTION.FindAllWhere(e => JournalEntries.Contains(e.Id)).ToList();

    public override string ToString()
    {
        return $"JournalUser(Id={DiscordId}, Username={Username}, DisplayName={DisplayName}, HealthSync={HealthSync.Count}, IsAdmin={IsAdmin})";
    }
}
