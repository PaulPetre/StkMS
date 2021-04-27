using System.Collections.Generic;
using System.Linq;

namespace StkMS.Services
{
    public static class Extensions
    {
        public static IEnumerable<T[]> GetBatches<T>(this IEnumerable<T> source, int count) => source
            .Select((value, index) => new { value, index })
            .GroupBy(it => it.index / count)
            .Select(g => g.Select(it => it.value).ToArray());
    }
}