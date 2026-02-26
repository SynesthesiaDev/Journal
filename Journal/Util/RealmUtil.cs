using System.ComponentModel;
using System.Security.Claims;
using Journal.Database.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Realms;

namespace Journal.Util;

public static class RealmUtil
{
    public static long GetDiscordId(AuthenticationState authState)
    {
        var discordIdString = authState.User.FindFirstValue("urn:discord:id");
        return Convert.ToInt64(discordIdString);
    }

    public static RealmUser GetRealmUser(AuthenticationState authState, Realm realm)
    {
        return realm.Find<RealmUser>(GetDiscordId(authState)) ?? throw new InvalidAsynchronousStateException("Realm user is null");
    }

    public static List<JournalEntry> GetJournalEntries(AuthenticationState authState, Realm realm)
    {
        var discordId = GetDiscordId(authState);
        return realm.All<JournalEntry>().Where(e => e.OwnerDiscordId == discordId).ToList();
    }

    public static JournalEntry? GetJournalEntryOrNull(Guid id, long discordOwnerId, Realm realm)
    {
        var entry = realm.Find<JournalEntry>(id);
        if (entry != null && entry.OwnerDiscordId != discordOwnerId) return null;
        return entry;
    }

    public static List<JournalEntry> GetJournalEntries(long discordId, Realm realm)
    {
        return realm.All<JournalEntry>().Where(e => e.OwnerDiscordId == discordId).ToList();
    }
}
