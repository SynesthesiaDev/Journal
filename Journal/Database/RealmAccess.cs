using Realms;

namespace Journal.Database;

public static class RealmAccess
{
    public static void Migrate(Migration migration, ulong oldSchemaVersion)
    {
        if (oldSchemaVersion == 1)
        {
            var newJournalEntries = migration.NewRealm.DynamicApi.All("JournalEntry");

            foreach (var entry in newJournalEntries)
            {
                var tracker = entry.DynamicApi.Get<dynamic>("MentalHealthTrackerEntry");

                if (tracker != null)
                {
                    tracker.SleepStart = 0.0;
                    tracker.SleepEnd = 0.0;
                }
            }
        }

        if (oldSchemaVersion == 2)
        {
            var newJournalEntries = migration.NewRealm.DynamicApi.All("JournalEntry");

            foreach (var entry in newJournalEntries)
            {
                var tracker = entry.DynamicApi.Get<dynamic>("MentalHealthTrackerEntry");

                if (tracker != null)
                {
                    tracker.MoodRating = 0;
                    tracker.ProductivityRating = 0;
                }
            }
        }

        if (oldSchemaVersion == 3)
        {
            var newJournalEntries = migration.NewRealm.DynamicApi.All("JournalEntry");

            foreach (var entry in newJournalEntries)
            {
                entry.DynamicApi.Set("Sunrise", 25200);
                entry.DynamicApi.Set("Sunset", 73800);
                entry.DynamicApi.Set("Weather", "Clear Sky");
            }
        }

        if (oldSchemaVersion == 4)
        {
            var newJournalEntries = migration.NewRealm.DynamicApi.All("JournalEntry");

            foreach (var entry in newJournalEntries)
            {
                entry.DynamicApi.Set("WeatherId", 0);
            }
        }
    }
}
