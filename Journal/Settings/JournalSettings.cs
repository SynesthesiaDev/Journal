using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Versioned;
using Codon.Optionals;
using Journal.Database.Models;

namespace Journal.Settings;

public record JournalSettings(
    string Domain,
    int Port,
    bool RequiresAuth,
    double Latitude,
    double Longitude,
    string VisualCrossingWeatherApiKey,
    List<UserDefinedTag> UserDefinedTags,
    Optional<DiscordAuthSettings> DiscordAuthSettings
)
{
    public static bool DidMigrate;

    public static readonly JournalSettings DEFAULT = new("0.0.0.0", 80, true, 0.0, 0.0, "weather api key here!!!", [new UserDefinedTag("test", "Test Tag"), new UserDefinedTag("tired", "Tired")], Optional.Of(Settings.DiscordAuthSettings.DEFAULT));

    private static readonly StructCodec<JournalSettings> codec = StructCodec.For<JournalSettings>()
        .Field("domain", Codecs.STRING, o => o.Domain)
        .Field("port", Codecs.INT, o => o.Port)
        .Field("requires_auth", Codecs.BOOLEAN, o => o.RequiresAuth)
        .Field("latitude", Codecs.DOUBLE, o => o.Latitude)
        .Field("longitude", Codecs.DOUBLE, o => o.Longitude)
        .Field("visual_crossing_weather_api_key", Codecs.STRING, o => o.VisualCrossingWeatherApiKey)
        .Field("user_defined_tags", UserDefinedTag.CODEC.List(), o => o.UserDefinedTags)
        .Field("discord_auth_settings", Settings.DiscordAuthSettings.CODEC.Optional(), o => o.DiscordAuthSettings)
        .Build((domain, port, requiresAuth, latitude, longitude, weatherKey, tags, discordAuthSettings) => new JournalSettings(domain, port, requiresAuth, latitude, longitude, weatherKey, tags, discordAuthSettings));

    public static readonly VersionedStructCodec<JournalSettings> VERSIONED_CODEC = new()
    {
        CurrentSchemaVersion = 2,
        InnerCodec = codec,
        SchemaMigrationRegistry = SchemaMigrationRegistry.Builder()
            .For<JsonElement>(migrations =>
            {
                migrations.Add(1, (transcoder, _, output) =>
                {
                    DidMigrate = true;
                    output.Put("latitude", transcoder.EncodeDouble(DEFAULT.Latitude));
                    output.Put("longitude", transcoder.EncodeDouble(DEFAULT.Longitude));
                });

                migrations.Add(2, (transcoder, _, output) =>
                {
                    DidMigrate = true;
                    output.Put("visual_crossing_weather_api_key", transcoder.EncodeString(DEFAULT.VisualCrossingWeatherApiKey));
                });
            })
    };
}

public record DiscordAuthSettings(
    long AppId,
    long ClientId,
    string Secret
)
{
    public static readonly DiscordAuthSettings DEFAULT = new
    (
        1234567891011121314,
        1234567891011121314,
        "zVXCrTbIxx9Gw-UIUsj4eobk6RjZ9Sx9"
    );

    public static readonly StructCodec<DiscordAuthSettings> CODEC = StructCodec.For<DiscordAuthSettings>()
        .Field("app_id", Codecs.LONG, o => o.AppId)
        .Field("client_id", Codecs.LONG, o => o.ClientId)
        .Field("secret", Codecs.STRING, o => o.Secret)
        .Build((appId, clientId, secret) => new DiscordAuthSettings(appId, clientId, secret));
}
