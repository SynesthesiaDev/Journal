// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec;

namespace Journal.Contracts.GoogleHealth;

public record AccountSyncRequest(string Code, string Device)
{
    public static readonly Codec<AccountSyncRequest> CODEC = StructCodec.For<AccountSyncRequest>()
        .Field("code", Codecs.STRING, c => c.Code)
        .Field("device", Codecs.STRING, c => c.Device)
        .Build((code, device) => new AccountSyncRequest(code, device));
}
