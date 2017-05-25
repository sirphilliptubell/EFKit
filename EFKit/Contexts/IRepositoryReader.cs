using System;
using System.Linq;
using EFKit.Entity;

namespace EFKit.Contexts
{
    /// <summary>
    /// Represents an object which reads a repository.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier used to identify entities.</typeparam>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IRepositoryReader<T, TKey>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Gets a table with it's entities tracked.
        /// </summary>
        IQueryable<T> Table { get; }

        /// <summary>
        /// Gets a table without tracking enabled.
        /// </summary>
        IQueryable<T> TableNoTracking { get; }

        /// <summary>Gets a table as tracked or not tracked, depending on a value.</summary>
        /// <param name="tracked">if set to <c>true</c> the table is tracked.</param>
        /// <returns></returns>
        IQueryable<T> TableWithTracking(bool tracked);

        /// <summary>
        /// Sets the command timeout.
        /// Null sets to the default value.
        /// </summary>
        int? CommandTimeoutSeconds { set; }
    }
}