using System;
using System.Collections.Generic;
using System.Linq;

namespace MyTeam
{
    public static class EnumerableExtensions
    {

        public static bool ContainsAny(this IEnumerable<string> collection, params string[] values)
        {
            return collection.Any(value => values.Any(v => v == value));
        }

        public static bool ContainsAny(this IEnumerable<Guid> collection, IEnumerable<Guid> values)
        {
            return collection.Any(value => values.Any(v => v == value));
        }
    }
}