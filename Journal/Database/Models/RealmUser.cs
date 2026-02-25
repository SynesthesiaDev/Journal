using Codon.Codec;
using Realms;

namespace Journal.Database.Models;

public partial class RealmUser : IRealmObject
{
    [PrimaryKey] public long DiscordId { get; set; } = 0L;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string ProfileImageUrl { get; set; } = string.Empty;
    public bool IsAdmin { get; set; } = false;

    public RealmUser()
    {
    }

    public RealmUser(long discordId, string username, string displayName, string profileImageUrl, bool isAdmin)
    {
        DiscordId = discordId;
        Username = username;
        DisplayName = displayName;
        ProfileImageUrl = profileImageUrl;
        IsAdmin = isAdmin;
    }

    public static readonly StructCodec<RealmUser> CODEC = StructCodec.Of
    (
        "discord_id", Codecs.LONG, u => u.DiscordId,
        "username", Codecs.STRING, u => u.Username,
        "display_name", Codecs.STRING, u => u.DisplayName,
        "profile_image_url", Codecs.STRING, u => u.ProfileImageUrl,
        "is_admin", Codecs.BOOLEAN, u => u.IsAdmin,
        (discordId, username, displayName, profileImageUrl, isAdmin) => new RealmUser(discordId, username, displayName, profileImageUrl, isAdmin)
    );
}
