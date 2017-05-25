using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit
{
    /// <summary>
    /// Extension methods for IEnumerable.
    /// </summary>
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Converts the current collection to a HashSet.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="items">The items.</param>
        /// <returns></returns>
        internal static HashSet<T> ToHashSet<T>(this IEnumerable<T> items)
            => new HashSet<T>(items);
    }
}