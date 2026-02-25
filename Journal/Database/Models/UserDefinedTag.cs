using Codon.Codec;
using Realms;

namespace Journal.Database.Models;

public partial class UserDefinedTag : RealmObject
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;

    public UserDefinedTag(string id, string title)
    {
        Id = id;
        Title = title;
    }

    // realm needs
    public UserDefinedTag()
    {
    }

    public static readonly StructCodec<UserDefinedTag> CODEC = StructCodec.Of
    (
        "id", Codecs.STRING, u => u.Id,
        "title", Codecs.STRING, u => u.Title,
        (id, title) => new UserDefinedTag(id, title)
    );
}
