using System.Collections.Generic;
using System.Linq;

namespace Tiled.Parsing.Commons
{
    public static class EnumerableExtensions
    {
        /// <summary>
        /// Converts a string array whose values are actually all numbers to an int array
        /// </summary>
        /// <param name="src">The string array</param>
        /// <returns>The parsed int array</returns>
        public static int[] AsIntArray(this IEnumerable<string> src)
            => src.Select(x => int.Parse(x.Length == 0 ? "-1" : x)).ToArray();
    }
}
