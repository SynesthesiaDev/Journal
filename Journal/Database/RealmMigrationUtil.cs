// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Text.Json;
using Codon.Codec;
using Codon.Codec.Json;
using Journal.Database.Models;
using Journal.Util;
using Serilog;
using Synesthesia.Utils.Extensions;

namespace Journal.Database;

public static class RealmMigrationUtil
{
    public record RealmExport(List<RealmJournalEntry> JournalEntries, List<RealmJournalUser> Users, List<UserDefinedTag> UserDefinedTags)
    {
        public void RemoveDuplicateTags()
        {
            var middle = UserDefinedTags.DistinctBy(p => p.Id).ToList();
            UserDefinedTags.Clear();
            UserDefinedTags.AddRange(middle);
        }

        public static readonly Codec<RealmExport> CODEC = StructCodec.For<RealmExport>()
            .Field("JournalEntry", RealmJournalEntry.CODEC.List(), e => e.JournalEntries)
            .Field("RealmUser", RealmJournalUser.CODEC.List(), e => e.Users)
            .Field("UserDefinedTag", UserDefinedTag.CODEC.List(), e => e.UserDefinedTags)
            .Build((list, users, arg3) => new RealmExport(list, users, arg3));
    }

    public static void TryToParse()
    {
        var path = JournalApp.REALM_EXPORTS_PATH;
        var text = File.ReadAllText(path);
        var json = JsonDocument.Parse(text).RootElement;
        var decoded = RealmExport.CODEC.Decode(JsonTranscoder.INSTANCE, json);

        decoded.RemoveDuplicateTags();

        Log.Warning("Entries: {e}, Users: {u}, Tags: {t}", decoded.JournalEntries.Count, decoded.Users.Count, decoded.UserDefinedTags.Count);

        foreach (var entry in decoded.JournalEntries)
        {
            Log.Information("Importing entry {entryDate} ({id})", entry.Title, entry.Id);
            JournalEntry.DB_COLLECTION.Insert(entry.Id, entry.ToNew());
        }

        foreach (var user in decoded.Users)
        {
            Log.Information("Importing user {user} ({id})", user.DisplayName, user.DiscordId);
            var entries = decoded.JournalEntries.Filter(e => e.OwnerDiscordId == user.DiscordId).Select(e => e.Id).ToList();
            Log.Information("User {id} matched {count} entries", user.DiscordId, entries.Count);

            JournalUser.DB_COLLECTION.Insert(user.DiscordId, user.ToNew(entries));
        }
    }

    public record RealmJournalEntry(
        Guid Id,
        long OwnerDiscordId,
        long Time,
        string Title,
        string Text,
        int Sunrise,
        int Sunset,
        int WeatherId,
        RealmMentalHealthTrackerEntry RealmMentalHealthTrackerEntry
    )
    {
        public JournalEntry ToNew()
        {
            return new JournalEntry
            (
                Id,
                OwnerDiscordId,
                Time,
                Title,
                Text,
                Sunrise,
                Sunset,
                (Weather)WeatherId,
                RealmMentalHealthTrackerEntry.ToNew()
            );
        }

        public static readonly Codec<RealmJournalEntry> CODEC = StructCodec.For<RealmJournalEntry>()
            .Field("Id", ExtraCodecs.GUID_CODEC, e => e.Id)
            .Field("OwnerDiscordId", Codecs.LONG, e => e.OwnerDiscordId)
            .Field("Time", Codecs.LONG, e => e.Time)
            .Field("Title", Codecs.STRING, e => e.Title)
            .Field("Text", Codecs.STRING, e => e.Text)
            .Field("Sunrise", Codecs.INT, e => e.Sunrise)
            .Field("Sunset", Codecs.INT, e => e.Sunset)
            .Field("WeatherId", Codecs.INT, e => e.WeatherId)
            .Field("MentalHealthTrackerEntry", RealmMentalHealthTrackerEntry.CODEC, e => e.RealmMentalHealthTrackerEntry)
            .Build((guid, l, arg3, arg4, arg5, arg6, arg7, arg8, arg9) => new RealmJournalEntry(guid, l, arg3, arg4, arg5, arg6, arg7, arg8, arg9));
    }

    public record RealmMentalHealthTrackerEntry(
        int MotivationTag,
        int SocialTag,
        int StressTag,
        double SleepStart,
        double SleepEnd,
        double HoursSlept,
        List<UserDefinedTag> UserDefinedTags,
        double OverallDayRating,
        double MoodRating,
        double ProductivityRating
    )
    {
        public static readonly StructCodec<RealmMentalHealthTrackerEntry> CODEC = StructCodec.For<RealmMentalHealthTrackerEntry>()
            .Field("MotivationTagId", Codecs.INT, e => e.MotivationTag)
            .Field("SocialTagId", Codecs.INT, e => e.SocialTag)
            .Field("StressTagId", Codecs.INT, e => e.StressTag)
            .Field("SleepStart", Codecs.DOUBLE, e => e.SleepStart)
            .Field("SleepEnd", Codecs.DOUBLE, e => e.SleepEnd)
            .Field("HoursSlept", Codecs.DOUBLE, e => e.HoursSlept)
            .Field("UserDefinedTags", UserDefinedTag.CODEC.List(), e => e.UserDefinedTags)
            .Field("OverallDayRating", Codecs.DOUBLE, e => e.OverallDayRating)
            .Field("MoodRating", Codecs.DOUBLE, e => e.MoodRating)
            .Field("ProductivityRating", Codecs.DOUBLE, e => e.ProductivityRating)
            .Build((tag, socialTag, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => new RealmMentalHealthTrackerEntry(tag, socialTag, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));

        public MentalHealthTrackerEntry ToNew()
        {
            return new MentalHealthTrackerEntry
            (
                (MotivationTag)MotivationTag,
                (SocialTag)SocialTag,
                (StressTag)StressTag,
                SleepStart,
                SleepEnd,
                HoursSlept,
                UserDefinedTags,
                (int)OverallDayRating,
                (int)MoodRating,
                (int)ProductivityRating
            );
        }
    }

    public record RealmJournalUser(
        long DiscordId,
        string Username,
        string DisplayName,
        string ProfileImageUrl,
        bool IsAdmin,
        string? MobileAuthToken,
        string? MobileSyncCode,
        Dictionary<string, HealthSync> HealthSync
    )
    {
        public static readonly Codec<RealmJournalUser> CODEC = StructCodec.For<RealmJournalUser>()
            .Field("DiscordId", Codecs.LONG, u => u.DiscordId)
            .Field("Username", Codecs.STRING, u => u.Username)
            .Field("DisplayName", Codecs.STRING, u => u.DisplayName)
            .Field("ProfileImageUrl", Codecs.STRING, u => u.ProfileImageUrl)
            .Field("IsAdmin", Codecs.BOOLEAN, u => u.IsAdmin)
            .Field("MobileAuthToken", Codecs.STRING.Optional(), u => ExtraCodecs.OfOrEmptyClass(u.MobileAuthToken))
            .Field("MobileSyncCode", Codecs.STRING.Optional(), u => ExtraCodecs.OfOrEmptyClass(u.MobileSyncCode))
            .Field("HealthSync", Codecs.STRING.MapTo(Models.HealthSync.CODEC), u => u.HealthSync)
            .Build((l, s, arg3, arg4, arg5, arg6, arg7, arg8) => new RealmJournalUser(l, s, arg3, arg4, arg5, arg6.Value, arg7.Value, arg8));

        public JournalUser ToNew(List<Guid> entries)
        {
            return new JournalUser(DiscordId, Username, DisplayName, ProfileImageUrl, IsAdmin, MobileAuthToken, MobileSyncCode, HealthSync, entries);
        }
    }
}
