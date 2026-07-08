// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Binary;
using Codon.Codec;
using Journal.Util;

namespace Journal.Database.Models;

public record HealthSync(long Steps, double DistanceKm, double Calories, double HeartPoints, double? Height, double? Weight, int MoveMinutes, long Date)
{
    public static readonly Codec<HealthSync> CODEC = StructCodec.For<HealthSync>()
        .Field("Steps", Codecs.LONG, c => c.Steps)
        .Field("DistanceKm", Codecs.DOUBLE, c => c.DistanceKm)
        .Field("Calories", Codecs.DOUBLE, c => c.Calories)
        .Field("HeartPoints", Codecs.DOUBLE, c => c.HeartPoints)
        .Field("Height", Codecs.DOUBLE.Optional(), c => ExtraCodecs.OfOrEmptyStruct(c.Height))
        .Field("Weight", Codecs.DOUBLE.Optional(), c => ExtraCodecs.OfOrEmptyStruct(c.Weight))
        .Field("MoveMinutes", Codecs.INT, c => c.MoveMinutes)
        .Field("Date", Codecs.LONG, c => c.Date)
        .Build((steps, distance, calories, heartPoints, height, weight, moveMinutes, date) =>
            new HealthSync(steps, distance, calories, heartPoints, height.Value, weight.Value, moveMinutes, date));

    public static readonly IBinaryCodec<HealthSync> BINARY_CODEC = BinaryCodecs.For<HealthSync>()
        .Field(BinaryCodecs.LONG, c => c.Steps)
        .Field(BinaryCodecs.DOUBLE, c => c.DistanceKm)
        .Field(BinaryCodecs.DOUBLE, c => c.Calories)
        .Field(BinaryCodecs.DOUBLE, c => c.HeartPoints)
        .Field(BinaryCodecs.DOUBLE.Optional(), c => ExtraCodecs.OfOrEmptyStruct(c.Height))
        .Field(BinaryCodecs.DOUBLE.Optional(), c => ExtraCodecs.OfOrEmptyStruct(c.Weight))
        .Field(BinaryCodecs.INT, c => c.MoveMinutes)
        .Field(BinaryCodecs.LONG, c => c.Date)
        .Build((steps, distance, calories, heartPoints, height, weight, moveMinutes, date) =>
            new HealthSync(steps, distance, calories, heartPoints, height.Value, weight.Value, moveMinutes, date));

    public override string ToString()
    {
        return $"HealthSync({Steps}, {DistanceKm}, {Calories}, {HeartPoints}, {Height}, {Weight}, {MoveMinutes}, {Date}";
    }
}
