using System;
using System.Collections.Generic;
using System.Linq;
using EFKit.Entity;

namespace EFKit.Services
{
    /// <summary>
    /// Represents the basic functionality of a service that reads entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    public interface IReadService<T, TKey>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Gets all entities with no predicates.
        /// </summary>
        /// <returns></returns>
        IQueryable<T> All();

        /// <summary>
        /// Gets a value indicating if an entity with the specified Id exists.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        bool Exists(TKey id);

        /// <summary>
        /// Gets the entity with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the entity.</param>
        /// <param name="tracked">if set to <c>true</c> the entity will be change-tracked.</param>
        /// <returns></returns>
        Maybe<T> GetById(TKey id, bool tracked = false);

        /// <summary>
        /// Gets the entity with the specified Ids.
        /// </summary>
        /// <param name="ids">The ids of the entities.</param>
        /// <param name="tracked">if set to <c>true</c> the entities will be change-tracked.</param>
        /// <returns></returns>
        IQueryable<T> GetByIds(IEnumerable<TKey> ids, bool tracked = false);

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <returns></returns>
        string GetDatabaseName();
    }
}