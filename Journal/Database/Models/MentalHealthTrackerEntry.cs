using Codon.Codec;
using Realms;
using SynesthesiaUtil.Extensions;

namespace Journal.Database.Models;

public partial class MentalHealthTrackerEntry : EmbeddedObject
{
    public int MotivationTagId { get; set; } = 0;
    public int SocialTagId { get; set; } = 0;
    public int StressTagId { get; set; } = 0;
    public double SleepStart { get; set; } = 0.0;
    public double SleepEnd { get; set; } = 0.0;
    public double HoursSlept { get; set; } = 0.0;
    public IList<UserDefinedTag> UserDefinedTags { get; }
    public int OverallDayRating { get; set; } = 0;

    public int MoodRating { get; set; } = 0;

    public int ProductivityRating { get; set; } = 0;

    [Ignored]
    public MotivationTag MotivationTag
    {
        get => (MotivationTag)MotivationTagId;
        init => MotivationTagId = value.Ordinal();
    }

    [Ignored]
    public SocialTag SocialTag
    {
        get => (SocialTag)SocialTagId;
        init => SocialTagId = value.Ordinal();
    }

    [Ignored]
    public StressTag StressTag
    {
        get => (StressTag)StressTagId;
        init => StressTagId = value.Ordinal();
    }

    [Ignored]
    private List<UserDefinedTag> listTransform => (List<UserDefinedTag>)UserDefinedTags;

    public MentalHealthTrackerEntry(MotivationTag motivationTag, SocialTag socialTag, StressTag stressTag, double sleepStart, double sleepEnd, double hoursSlept, List<UserDefinedTag> userDefinedTags, int overallDayRating, int moodRating, int productivityRating)
    {
        MotivationTag = motivationTag;
        SocialTag = socialTag;
        StressTag = stressTag;
        UserDefinedTags = userDefinedTags;
        OverallDayRating = overallDayRating;
        SleepStart = sleepStart;
        SleepEnd = sleepEnd;
        HoursSlept = hoursSlept;
        MoodRating = moodRating;
        ProductivityRating = productivityRating;
    }

    // realm needs
    public MentalHealthTrackerEntry()
    {
    }

    public static readonly StructCodec<MentalHealthTrackerEntry> CODEC = StructCodec.Of
    (
        "motivation_tag", Codecs.Enum<MotivationTag>(), e => e.MotivationTag,
        "social_tag", Codecs.Enum<SocialTag>(), e => e.SocialTag,
        "stress_tag", Codecs.Enum<StressTag>(), e => e.StressTag,
        "sleep_start", Codecs.DOUBLE, e => e.SleepStart,
        "sleep_end", Codecs.DOUBLE, e => e.SleepEnd,
        "hours_slept", Codecs.DOUBLE, e => e.HoursSlept,
        "user_defined_tags", UserDefinedTag.CODEC.List(), e => e.listTransform,
        "overall_day_rating", Codecs.INT, e => e.OverallDayRating,
        "mood_rating", Codecs.INT, e => e.MoodRating,
        "productivity_rating", Codecs.INT, e => e.ProductivityRating,
        (motivation, social, stress, sleepStart, sleepEnd, hoursSlept, userTags, rating, mood, productivity) => new MentalHealthTrackerEntry(motivation, social, stress, sleepStart, sleepEnd, hoursSlept, userTags, rating, mood, productivity)
    );
}
