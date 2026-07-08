// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Security.Claims;
using AspNet.Security.OAuth.Discord;
using Journal.Database.Models;
using Journal.Integrations;
using Journal.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OAuth;

namespace Journal.Authentication;

public static class AuthenticationExtensions
{
    public const string DISCORD_IDENTIFIER = "urn:discord:id";

    public static IServiceCollection AddDiscordAuthentication(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = DiscordAuthenticationDefaults.AuthenticationScheme;
            })
            .AddCookie(options => { options.LoginPath = "/login"; })
            .AddDiscord(options =>
            {
                options.ClientId = SettingsManager.Config.DiscordAuthSettings.Value!.ClientId.ToString();
                options.ClientSecret = SettingsManager.Config.DiscordAuthSettings.Value!.Secret;

                options.SaveTokens = true;
                options.Scope.Add("identify");
                options.ClaimActions.MapJsonKey(DISCORD_IDENTIFIER, "id");

                options.CorrelationCookie.SameSite = SameSiteMode.Lax;
                options.CorrelationCookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;

                options.Events.OnCreatingTicket = OnDiscordTicketCreated;
            });
        return serviceCollection;
    }

    private static async Task OnDiscordTicketCreated(OAuthCreatingTicketContext context)
    {
        var data = await DiscordIntegration.GetData(context.AccessToken!);
        if (data == null) throw new InvalidDataException("data is null");

        if (context.Principal?.Identity is not ClaimsIdentity identity) return;

        var user = JournalUser.DB_COLLECTION.FindOrAdd(data.DiscordId, _ => data);
        JournalUser.DB_COLLECTION.Insert(data.DiscordId, user with
        {
            DisplayName = data.DisplayName,
            ProfileImageUrl = data.ProfileImageUrl,
            Username = data.Username
        });

        identity.AddClaim(new Claim(DISCORD_IDENTIFIER, data.DiscordId.ToString()));
    }
}
