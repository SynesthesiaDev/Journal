// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

namespace Journal.Util;

public static class MathUtils
{
    public static double CircularMeanHours(IEnumerable<double> hours)
    {
        double sinSum = 0, cosSum = 0;
        int count = 0;

        foreach (var h in hours)
        {
            double angle = h / 24.0 * 2 * Math.PI;
            sinSum += Math.Sin(angle);
            cosSum += Math.Cos(angle);
            count++;
        }

        if (count == 0) return 0;

        double avgAngle = Math.Atan2(sinSum / count, cosSum / count);
        double avgHours = avgAngle / (2 * Math.PI) * 24.0;

        return avgHours < 0 ? avgHours + 24 : avgHours;
    }

    public static int Average(IEnumerable<int> list)
    {
        var enumerable = list.ToList();
        var sum = enumerable.Sum();
        return (int)(enumerable.Count != 0 ? (double)sum / enumerable.Count : 0.0);
    }
}
