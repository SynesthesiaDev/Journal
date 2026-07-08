// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;
using Codon.Codec.Json;
using Codon.Optionals;
using Journal.Contracts.GoogleHealth;
using Journal.Database.Models;
using Journal.Util;

namespace Journal.Endpoints;

public static class HealthConnectEndpoints
{
    public static void MapGoogleHealthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/mobileAuth", async context =>
        {
            var body = await context.Request.GetBodyString();
            var decoded = AccountSyncRequest.CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(body).RootElement);

            var user = JournalUser.DB_COLLECTION.FindFirstOrNull(u => u.MobileSyncCode == decoded.Code);

            AccountSyncResponse response;
            if (user == null)
            {
                response = new AccountSyncResponse(false, "User with that code that doesn't have account already linked does not exist", Optional.Empty<long>(), Optional.Empty<string>(), Optional.Empty<string>());
            }
            else
            {
                var token = ExtraCodecs.GenerateSecureRandomToken();

                var editedUser = user with
                {
                    MobileAuthToken = token,
                    MobileSyncCode = null
                };

                JournalUser.DB_COLLECTION.Insert(editedUser.DiscordId, editedUser);
                response = new AccountSyncResponse(true, "Yippee", Optional.Of(user.DiscordId), Optional.Of(user.DisplayName), Optional.Of(token));
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(AccountSyncResponse.CODEC.Encode(JsonTranscoder.INSTANCE, response).GetRawText());
        });

        app.MapPost("/api/syncHealthData", async context =>
        {
            var authToken = context.Request.Headers.Authorization.FirstOrDefault();
            if (authToken == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var user = JournalUser.DB_COLLECTION.FindFirstOrNull(u => u.MobileAuthToken == authToken);

            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var body = await context.Request.GetBodyString();
            var decoded = HealthSync.CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(body).RootElement);

            user.HealthSync[decoded.Date.ToString()] = decoded;
            JournalUser.DB_COLLECTION.Insert(user.DiscordId, user);
        });
    }
}
