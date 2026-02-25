using Journal.Database.Models;

namespace Journal.Util;

public static class Routes
{
    public const string HOME = "/";
    public const string JOURNAL = "/journal";
    public const string JOURNAL_NEW_ENTRY = "/journal/new";
    public const string MENTAL_HEALTH_TRACKER = "/tracker";
    public const string EXPORT_DATA = "/export";
    public const string SETTINGS = "/settings";

    public static string JournalEntry(JournalEntry entry)
    {
        return $"/journal/entry/{entry.Id.ToString()}";
    }
}