using System.Collections.Generic;
using System.Linq;

namespace StkMS.Library.Services
{
    public static class Extensions
    {
        public static IEnumerable<T[]> GetBatches<T>(this IEnumerable<T> source, int count) => source
            .Select((value, index) => new { value, index })
            .GroupBy(it => it.index / count)
            .Select(g => g.Select(it => it.value).ToArray());

        public static decimal ParseDecimal(this string? s) =>
            decimal.TryParse(s, out var result) ? result : default;

        public static string InQuotes(this string? s) =>
            Q + s?.Replace(Q, Q + Q) + Q;

        //

        private const string Q = "\"";
    }
}