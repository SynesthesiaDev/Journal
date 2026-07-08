// Copyright (c) 2026 SynesthesiaDev <synesthesiadev@proton.me>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using Journal.Database.Models;

namespace Journal.Extensions;

public static class ListExtensions
{
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    public static List<JournalEntry> SortByDate(this List<JournalEntry> list, SortDirection direction)
    {
        if (list.Count <= 1)
            return list;

        if (direction == SortDirection.Ascending)
            list.Sort((x, y) => x.Time.CompareTo(y.Time));
        else
            list.Sort((x, y) => y.Time.CompareTo(x.Time));

        return list;
    }
}
