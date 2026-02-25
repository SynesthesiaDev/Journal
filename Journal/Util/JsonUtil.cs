using System.Text.Json;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Journal.Util;

public static class JsonUtil
{
    private static JsonSerializerSettings jsonOptions = new JsonSerializerSettings()
    {
        Formatting = Formatting.Indented,
    };


    public static string Prettified(JsonElement jsonElement)
    {
        var parsed = JToken.Parse(jsonElement.GetRawText());
        var stringWriter = new StringWriter();
        var jsonTextWriter = new JsonTextWriter(stringWriter)
        {
            Formatting = Formatting.Indented,
            Indentation = 4
        };
        parsed.WriteTo(jsonTextWriter);
        stringWriter.Close();
        return stringWriter.ToString();
    }
}