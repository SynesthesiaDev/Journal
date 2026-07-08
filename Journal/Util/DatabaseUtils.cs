using System.Security.Claims;
using Journal.Authentication;
using Journal.Database.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Serilog;

namespace Journal.Util;

public static class DatabaseUtils
{
    public static long GetDiscordId(AuthenticationState authState)
    {
        var discordIdString = authState.User.FindFirstValue(AuthenticationExtensions.DISCORD_IDENTIFIER);
        return Convert.ToInt64(discordIdString);
    }

    public static List<JournalEntry> GetJournalEntries(AuthenticationState authState)
    {
        var discordId = GetDiscordId(authState);
        var user = JournalUser.DB_COLLECTION.Find(discordId);
        return user.ResolveJournalEntries();
    }

    public static JournalEntry? GetJournalEntryOrNull(Guid id, long discordOwnerId)
    {
        var entry = JournalEntry.DB_COLLECTION.FindOrNull(id);
        Log.Warning("Entry id: {entryId} != real id: {realId}", entry?.OwnerDiscordId, discordOwnerId);
        if (entry != null && entry.OwnerDiscordId != discordOwnerId) return null;
        return entry;
    }
}
