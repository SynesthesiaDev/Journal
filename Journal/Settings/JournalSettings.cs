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
    public static bool DidMigrate = false;

    public static readonly JournalSettings DEFAULT = new("0.0.0.0", 80, true, 0.0, 0.0, "weather api key here!!!", [new UserDefinedTag("test", "Test Tag"), new UserDefinedTag("tired", "Tired")], Optional.Of(Settings.DiscordAuthSettings.DEFAULT));

    private static readonly StructCodec<JournalSettings> codec = StructCodec.Of
    (
        "domain", Codecs.STRING, o => o.Domain,
        "port", Codecs.INT, o => o.Port,
        "requires_auth", Codecs.BOOLEAN, o => o.RequiresAuth,
        "latitude", Codecs.DOUBLE, o => o.Latitude,
        "longitude", Codecs.DOUBLE, o => o.Longitude,
        "visual_crossing_weather_api_key", Codecs.STRING, o => o.VisualCrossingWeatherApiKey,
        "user_defined_tags", UserDefinedTag.CODEC.List(), o => o.UserDefinedTags,
        "discord_auth_settings", Settings.DiscordAuthSettings.CODEC.Optional(), o => o.DiscordAuthSettings,
        (domain, port, requiresAuth, latitude, longitude, weatherKey, tags, discordAuthSettings) => new JournalSettings(domain, port, requiresAuth, latitude, longitude, weatherKey, tags, discordAuthSettings)
    );

    public static readonly VersionedStructCodec<JournalSettings> VERSIONED_CODEC = new()
    {
        CurrentSchemaVersion = 2,
        InnerCodec = codec,
        SchemaMigrationRegistry = SchemaMigrationRegistry.Builder()
            .For<JsonElement>(migrations =>
            {
                migrations.Add(1, (transcoder, input, output) =>
                {
                    DidMigrate = true;
                    output.Put("latitude", transcoder.EncodeDouble(DEFAULT.Latitude));
                    output.Put("longitude", transcoder.EncodeDouble(DEFAULT.Longitude));
                });

                migrations.Add(2, (transcoder, input, output) =>
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

    public static readonly StructCodec<DiscordAuthSettings> CODEC = StructCodec.Of
    (
        "app_id", Codecs.LONG, o => o.AppId,
        "client_id", Codecs.LONG, o => o.ClientId,
        "secret", Codecs.STRING, o => o.Secret,
        (appId, clientId, secret) => new DiscordAuthSettings(appId, clientId, secret)
    );
}
