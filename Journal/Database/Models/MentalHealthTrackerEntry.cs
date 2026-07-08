using Codon.Binary;
using Codon.Codec;

namespace Journal.Database.Models;

public record MentalHealthTrackerEntry(
    MotivationTag MotivationTag,
    SocialTag SocialTag,
    StressTag StressTag,
    double SleepStart,
    double SleepEnd,
    double HoursSlept,
    List<UserDefinedTag> UserDefinedTags,
    int OverallDayRating,
    int MoodRating,
    int ProductivityRating
)
{
    public static readonly StructCodec<MentalHealthTrackerEntry> CODEC = StructCodec.For<MentalHealthTrackerEntry>()
        .Field("MotivationTag", Codecs.Enum<MotivationTag>(), e => e.MotivationTag)
        .Field("SocialTag", Codecs.Enum<SocialTag>(), e => e.SocialTag)
        .Field("StressTag", Codecs.Enum<StressTag>(), e => e.StressTag)
        .Field("SleepStart", Codecs.DOUBLE, e => e.SleepStart)
        .Field("SleepEnd", Codecs.DOUBLE, e => e.SleepEnd)
        .Field("HoursSlept", Codecs.DOUBLE, e => e.HoursSlept)
        .Field("UserDefinedTags", UserDefinedTag.CODEC.List(), e => e.UserDefinedTags)
        .Field("OverallDayRating", Codecs.INT, e => e.OverallDayRating)
        .Field("MoodRating", Codecs.INT, e => e.MoodRating)
        .Field("ProductivityRating", Codecs.INT, e => e.ProductivityRating)
        .Build((tag, socialTag, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => new MentalHealthTrackerEntry(tag, socialTag, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));


    public static readonly IBinaryCodec<MentalHealthTrackerEntry> BINARY_CODEC = BinaryCodecs.For<MentalHealthTrackerEntry>()
        .Field(BinaryCodecs.Enum<MotivationTag>(), e => e.MotivationTag)
        .Field(BinaryCodecs.Enum<SocialTag>(), e => e.SocialTag)
        .Field(BinaryCodecs.Enum<StressTag>(), e => e.StressTag)
        .Field(BinaryCodecs.DOUBLE, e => e.SleepStart)
        .Field(BinaryCodecs.DOUBLE, e => e.SleepEnd)
        .Field(BinaryCodecs.DOUBLE, e => e.HoursSlept)
        .Field(UserDefinedTag.BINARY_CODEC.List(), e => e.UserDefinedTags)
        .Field(BinaryCodecs.INT, e => e.OverallDayRating)
        .Field(BinaryCodecs.INT, e => e.MoodRating)
        .Field(BinaryCodecs.INT, e => e.ProductivityRating)
        .Build((tag, socialTag, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10) => new MentalHealthTrackerEntry(tag, socialTag, arg3, arg4, arg5, arg6, arg7, arg8, arg9, arg10));

}
