using Discord;
using Discord.Rest;
using Journal.Database.Models;

namespace Journal.API;

public static class DiscordApi
{
    public static async Task<RealmUser?> GetData(string accessToken)
    {
        try
        {
            await using var discordClient = new DiscordRestClient();
            await discordClient.LoginAsync(TokenType.Bearer, accessToken);

            var user = await discordClient.GetCurrentUserAsync();

            var newJournalUser = new RealmUser
            (
                (long)user.Id,
                user.Username,
                user.GlobalName,
                user.GetAvatarUrl(),
                false
            );

            return newJournalUser;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw;
        }
    }
}
