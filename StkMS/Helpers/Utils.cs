using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace StkMS.Helpers
{
    public static class Utils
    {
        public static DateTime? ParseDateTimeWithCulture(this string s)
        {
            s ??= "";

            var cultures = new[]
            {
                new CultureInfo("ro-RO"),
            };

            return cultures
                .Select(c => DateTime.TryParse(s, c, DateTimeStyles.None, out var result) ? (DateTime?)result : null)
                .FirstOrDefault(it => it != null);
        }
    }
}
