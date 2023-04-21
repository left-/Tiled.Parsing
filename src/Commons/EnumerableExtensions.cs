using System.Collections.Generic;
using System.Linq;

namespace Tiled.Parsing.Commons
{
    public static class EnumerableExtensions
    {
        public static int[] AsIntArray(this IEnumerable<string> src)
            => src.Select(x => int.Parse(x.Length == 0 ? "-1" : x)).ToArray();
    }
}
