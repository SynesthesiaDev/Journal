// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;
using Codon.Optionals;
using Journal.Database.Models;
using Journal.Util;
using Realms;

namespace Journal.API;

public class MobileConnectAuthApi(WebApplication webApplication, RealmConfiguration realmConfiguration)
{
    public void Initialize()
    {
        webApplication.MapPost("/api/mobileAuth", async context =>
        {
            var body = await context.Request.GetBodyString();
            var decoded = AccountSyncRequest.CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(body).RootElement);

            var realm = await Realm.GetInstanceAsync(realmConfiguration);
            var user = realm.All<RealmUser>().FirstOrDefault(u => u.MobileSyncCode == decoded.Code);

            AccountSyncResponse response;
            if (user == null)
            {
                response = new AccountSyncResponse(false, "User with that code that doesn't have account already linked does not exist", Optional.Empty<long>(), Optional.Empty<string>(), Optional.Empty<string>());
            }
            else
            {
                var token = Extensions.GenerateSecureRandomToken();
                await realm.WriteAsync(() =>
                {
                    user.MobileAuthToken = token;
                    user.MobileSyncCode = null;
                });
                response = new AccountSyncResponse(true, "Yippee", Optional.Of(user.DiscordId), Optional.Of(user.DisplayName), Optional.Of(token));
            }

            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(AccountSyncResponse.CODEC.Encode(JsonTranscoder.INSTANCE, response).GetRawText());
        });

        webApplication.MapPost("/api/syncHealthData", async context =>
        {
            var authToken = context.Request.Headers.Authorization.FirstOrDefault();
            if (authToken == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }


            var realm = await Realm.GetInstanceAsync(realmConfiguration);
            var user = realm.All<RealmUser>().FirstOrDefault(u => u.MobileAuthToken == authToken);

            if (user == null)
            {
                context.Response.StatusCode = StatusCodes.Status404NotFound;
                return;
            }

            var body = await context.Request.GetBodyString();
            var decoded = HealthSync.CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(body).RootElement);

            await realm.WriteAsync(() =>
            {
                user.HealthSync[decoded.Date.ToString()] = decoded;
            });
        });
    }

    public record AccountSyncRequest(string Code, string Device)
    {
        public static readonly Codec<AccountSyncRequest> CODEC = StructCodec.Of
        (
            "code", Codecs.STRING, c => c.Code,
            "device", Codecs.STRING, c => c.Device,
            (code, device) => new AccountSyncRequest(code, device)
        );
    }

    public record AccountSyncResponse(bool Success, string Message, Optional<long> UserId, Optional<string> Username, Optional<string> AuthenticationToken)
    {
        public static readonly Codec<AccountSyncResponse> CODEC = StructCodec.Of
        (
            "success", Codecs.BOOLEAN, c => c.Success,
            "message", Codecs.STRING, c => c.Message,
            "userId", Codecs.LONG.Optional(), c => c.UserId,
            "username", Codecs.STRING.Optional(), c => c.Username,
            "authenticationToken", Codecs.STRING.Optional(), c => c.AuthenticationToken,
            (success, message, userid, username, token) => new AccountSyncResponse(success, message, userid, username, token)
        );
    }
}
