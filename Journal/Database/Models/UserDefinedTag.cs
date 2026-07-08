using Codon.Binary;
using Codon.Codec;

namespace Journal.Database.Models;

public record UserDefinedTag(string Id, string Title)
{
    public static readonly StructCodec<UserDefinedTag> CODEC = StructCodec.For<UserDefinedTag>()
        .Field("Id", Codecs.STRING, u => u.Id)
        .Field("Title", Codecs.STRING, u => u.Title)
        .Build((id, title) => new UserDefinedTag(id, title));

    public static readonly IBinaryCodec<UserDefinedTag> BINARY_CODEC = BinaryCodecs.For<UserDefinedTag>()
        .Field(BinaryCodecs.STRING, u => u.Id)
        .Field(BinaryCodecs.STRING, u => u.Title)
        .Build((id, title) => new UserDefinedTag(id, title));


}
