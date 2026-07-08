using Discord;
using Discord.Rest;
using Journal.Database.Models;
using Serilog;

namespace Journal.Integrations;

public static class DiscordIntegration
{
    private static readonly DiscordRestClient discord_client = new DiscordRestClient();

    public static async Task<JournalUser?> GetData(string accessToken)
    {
        try
        {
            await discord_client.LoginAsync(TokenType.Bearer, accessToken);
            var user = await discord_client.GetCurrentUserAsync();

            var newJournalUser = new JournalUser
            (
                (long)user.Id,
                user.Username,
                user.GlobalName,
                user.GetAvatarUrl(),
                false,
                null,
                null,
                [],
                []
            );

            return newJournalUser;
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Failed to fetch Discord data");
            return null;
        }
    }
}
