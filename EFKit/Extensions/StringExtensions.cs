using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit
{
    /// <summary>
    /// String Extension methods.
    /// </summary>
    internal static class StringExtensions
    {
        /// <summary>
        /// Appends the specified text.
        /// However this will first append the specified separator if the StringBuilder already has anything.
        /// </summary>
        /// <param name="sb">The sb.</param>
        /// <param name="text">The text.</param>
        /// <param name="separator">The separator.</param>
        /// <returns></returns>
        private static StringBuilder Append(this StringBuilder sb, string text, string separator)
            => sb.Length == 0
            ? sb.Append(text)
            : sb.Append(separator).Append(text);

        internal static string Join(this IEnumerable<string> strings, string separator)
            => strings
            .Aggregate(
                seed: new StringBuilder(),
                func: (sb, s) => sb.Append(s, separator),
                resultSelector: sb => sb.ToString());
    }
}