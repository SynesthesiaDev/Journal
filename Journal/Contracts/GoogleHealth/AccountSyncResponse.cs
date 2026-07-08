// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec;
using Codon.Optionals;

namespace Journal.Contracts.GoogleHealth;

public record AccountSyncResponse(bool Success, string Message, Optional<long> UserId, Optional<string> Username, Optional<string> AuthenticationToken)
{
    public static readonly Codec<AccountSyncResponse> CODEC = StructCodec.For<AccountSyncResponse>()
        .Field("success", Codecs.BOOLEAN, c => c.Success)
        .Field("message", Codecs.STRING, c => c.Message)
        .Field("userId", Codecs.LONG.Optional(), c => c.UserId)
        .Field("username", Codecs.STRING.Optional(), c => c.Username)
        .Field("authenticationToken", Codecs.STRING.Optional(), c => c.AuthenticationToken)
        .Build((success, message, userid, username, token) => new AccountSyncResponse(success, message, userid, username, token));
}
