using System.Collections.Generic;
using System.Linq;

namespace ScrapBot.Extensions
{
    public static partial class Extensions
    {
        public static IEnumerable<IEnumerable<T>> Chunk<T>(this IEnumerable<T> enumerable, int chunkSize)
            => enumerable.Select((s, i) => new { Value = s, Index = i })
                         .GroupBy(x => x.Index / chunkSize)
                         .Select(grp => grp.Select(x => x.Value).ToArray());
    }
}
