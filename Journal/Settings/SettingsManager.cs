using System.Text.Json;
using Codon.Codec.Json;
using Journal.Util;

namespace Journal.Settings;

public static class SettingsManager
{
    public static JournalSettings Config => fileLoaded ? currentConfig! : throw new InvalidOperationException("Config is not loaded yet!");

    public static readonly string CONFIG_PATH = Path.Combine(JournalApp.DATA_PATH, "settings.json");

    private static readonly JournalSettings default_settings = JournalSettings.DEFAULT;

    private static bool fileLoaded;
    private static JournalSettings? currentConfig;

    public static void Load()
    {
        if (!File.Exists(CONFIG_PATH))
        {
            Console.WriteLine("config does not exist");
            File.Create(CONFIG_PATH).Close();
            var encoded = JournalSettings.VERSIONED_CODEC.Encode(JsonTranscoder.INSTANCE, default_settings);
            File.WriteAllText(CONFIG_PATH, JsonUtil.Prettified(encoded));

            fileLoaded = true;
            currentConfig = default_settings;

            Console.WriteLine("Journal settings not found, creating new one!");
            return;
        }

        var json = File.ReadAllText(CONFIG_PATH);
        try
        {
            var decoded = JournalSettings.VERSIONED_CODEC.Decode(JsonTranscoder.INSTANCE, JsonDocument.Parse(json).RootElement);
            if (decoded.RequiresAuth && decoded.DiscordAuthSettings.IsMissing)
            {
                throw new InvalidOperationException("'discord_auth_settings' must be present if 'requires_auth' is enabled");
            }

            fileLoaded = true;
            currentConfig = decoded;
            Console.WriteLine("Journal settings loaded!");

            if (JournalSettings.DidMigrate)
            {
                var encoded = JournalSettings.VERSIONED_CODEC.Encode(JsonTranscoder.INSTANCE, default_settings);
                File.WriteAllText(CONFIG_PATH, JsonUtil.Prettified(encoded));
                Console.WriteLine("Journal settings re-saved because of schema migration!");
            }
        }
        catch (Exception e)
        {
            Console.WriteLine("Error while parsing 'settings.json': ");
            Console.WriteLine(e);
            throw;
        }
    }
}
