using System;
using System.Collections.Generic;
using EFKit.Entity;

namespace EFKit.Services
{
    /// <summary>
    /// Represents the basic functionality of a service that writes entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    public interface IWriteService<T, TKey, TUserId>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
    {
        /// <summary>
        /// Deletes the entity with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier of the entity to delete.</param>
        /// <param name="byUserId">The Id of the user doing the delete.</param>
        /// <returns></returns>
        Result Delete(TKey id, TUserId byUserId);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">The entity to insert.</param>
        /// <param name="byUserId">The Id of the user doing the insert.</param>
        /// <returns></returns>
        Result Insert(T entity, TUserId byUserId);

        /// <summary>
        /// Updates the entities with the specified ids.
        /// </summary>
        /// <param name="ids">The ids of the entities to update.</param>
        /// <param name="alter">The action that alters the entity.</param>
        /// <param name="byUserId">The Id of the user doing the update.</param>
        /// <returns></returns>
        Result Update(IEnumerable<TKey> ids, Action<T> alter, TUserId byUserId);

        /// <summary>
        /// Updates the entities with the specified id.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="alter">The action that alters the entity.</param>
        /// <param name="byUserId">The Id of the user doing the update.</param>
        /// <returns></returns>
        Result<T> Update(TKey id, Action<T> alter, TUserId byUserId);

        /// <summary>
        /// Updates the entities with the specified id.
        /// </summary>
        /// <param name="id">The id of the entity to update.</param>
        /// <param name="alter">The action that alters the entity.</param>
        /// <param name="byUserId">The Id of the user doing the update.</param>
        /// <returns></returns>
        Result<T> Update(TKey id, Func<T, Result> alter, TUserId byUserId);
    }
}