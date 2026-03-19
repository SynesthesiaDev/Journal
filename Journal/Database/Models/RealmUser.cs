using Codon.Codec;
using Codon.Optionals;
using Journal.Util;
using Realms;

namespace Journal.Database.Models;

public class RealmUser : RealmObject
{
    [PrimaryKey]
    public long DiscordId { get; set; }

    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public bool IsAdmin { get; set; }
    public string? MobileAuthToken { get; set; }
    public string? MobileSyncCode { get; set; }

    public IDictionary<string, HealthSync> HealthSync { get; }

    // realm needs an empty constructor
    public RealmUser()
    {
    }

    public RealmUser(long discordId, string username, string displayName, string profileImageUrl, bool isAdmin, Optional<string> mobileAuthToken, Optional<string> mobileAuthSyncCode, Dictionary<string, HealthSync> healthSync)
    {
        DiscordId = discordId;
        Username = username;
        DisplayName = displayName;
        ProfileImageUrl = profileImageUrl;
        IsAdmin = isAdmin;
        MobileAuthToken = mobileAuthToken.Value;
        MobileSyncCode = mobileAuthSyncCode.Value;
        foreach (var (key, value) in healthSync)
        {
            HealthSync[key] = value;
        }
    }

    public static readonly StructCodec<RealmUser> CODEC = StructCodec.Of
    (
        "discord_id", Codecs.LONG, u => u.DiscordId,
        "username", Codecs.STRING, u => u.Username,
        "display_name", Codecs.STRING, u => u.DisplayName,
        "profile_image_url", Codecs.STRING, u => u.ProfileImageUrl,
        "is_admin", Codecs.BOOLEAN, u => u.IsAdmin,
        "mobile_auth_token", Codecs.STRING.Optional(), u => Extensions.OfOrEmptyClass(u.MobileAuthToken),
        "mobile_sync_code", Codecs.STRING.Optional(), u => Extensions.OfOrEmptyClass(u.MobileSyncCode),
        "health_sync", Codecs.STRING.MapTo(Models.HealthSync.CODEC), u => u.HealthSync.ToDictionary(),
        (discordId, username, displayName, profileImageUrl, isAdmin, authToken, syncCode, health) => new RealmUser(discordId, username, displayName, profileImageUrl, isAdmin, authToken, syncCode, health)
    );

    public override string ToString()
    {
        return $"RealmUser(Id={DiscordId}, Username={Username}, DisplayName={DisplayName}, HealthSync={HealthSync.Count})";
    }
}
