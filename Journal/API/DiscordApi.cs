using Discord;
using Discord.Rest;
using Journal.Database.Models;
using Optional = Codon.Optionals.Optional;

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
                false,
                Optional.Empty<string>(),
                Optional.Empty<string>(),
                []
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
