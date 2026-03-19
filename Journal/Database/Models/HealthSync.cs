// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Codon.Codec;
using Journal.Util;
using Realms;

namespace Journal.Database.Models;

public class HealthSync : EmbeddedObject
{
    public long Steps { get; set; }
    public double DistanceKm { get; set; }
    public double Calories { get; set; }
    public double HeartPoints { get; set; }
    public double? Height { get; set; }
    public double? Weight { get; set; }
    public int MoveMinutes { get; set; }
    public long Date { get; set; }

    // Realm needs empty constructor
    public HealthSync()
    {

    }

    public HealthSync(long steps, double distanceKm, double calories, double heartPoints, double? height, double? weight, int moveMinutes, long date)
    {
        Steps = steps;
        DistanceKm = distanceKm;
        Calories = calories;
        HeartPoints = heartPoints;
        Height = height;
        Weight = weight;
        MoveMinutes = moveMinutes;
        Date = date;
    }

    public static readonly Codec<HealthSync> CODEC = StructCodec.Of
    (
        "steps", Codecs.LONG, c => c.Steps,
        "distanceKm", Codecs.DOUBLE, c => c.DistanceKm,
        "calories", Codecs.DOUBLE, c => c.Calories,
        "heartPoints", Codecs.DOUBLE, c => c.HeartPoints,
        "height", Codecs.DOUBLE.Optional(), c => Extensions.OfOrEmptyStruct(c.Height),
        "weight", Codecs.DOUBLE.Optional(), c => Extensions.OfOrEmptyStruct(c.Weight),
        "moveMinutes", Codecs.INT, c => c.MoveMinutes,
        "date", Codecs.LONG, c => c.Date,
        (steps, distance, calories, heartPoints, height, weight, moveMinutes, date) =>
            new HealthSync(steps, distance, calories, heartPoints, height.Value, weight.Value, moveMinutes, date)
    );

    public override string ToString()
    {
        return $"HealthSync({Steps}, {DistanceKm}, {Calories}, {HeartPoints}, {Height}, {Weight}, {MoveMinutes}, {Date}";
    }
}
