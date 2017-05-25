using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using EFKit.Entity;
using EFKit.Plugins;

namespace EFKit.Contexts
{
    /// <summary>
    /// Represents an object which writes to a repository.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier used to identify entities.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <typeparam name="T">The type of the entity.</typeparam>
    public interface IRepositoryWriter<T, TKey, TUserId>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
        where T : BaseEntity<TKey>, new()
    {
        /// <summary>
        /// Gets the table with change-tracking enabled.
        /// </summary>
        IQueryable<T> TableWithTracking { get; }

        /// <summary>
        /// Attaches the entity to the context and sets it's state as Unchanged.
        /// Fails if the entity is already attached.
        /// </summary>
        /// <param name="entity">The entity to attach.</param>
        /// <returns></returns>
        Result AttachAsUnchanged<U, UKey>(U entity)
            where U : BaseEntity<UKey>
            where UKey : struct, IEquatable<UKey>;

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns></returns>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Begins a transaction using the specified Isolation Level.
        /// </summary>
        /// <param name="isolationLevel">The Database isolation level with which the underlying transaction will be created.</param>
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity to delete.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="deletedBy">The User Id of the person making the change.</param>
        /// <param name="deletedUTC">The UTC time the entity is deleted.</param>
        /// <returns></returns>
        Result Delete(
            T entity,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            TUserId deletedBy,
            DateTime? deletedUTC = null);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">Required. The entities to delete.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="deletedBy">The User Id of the person making the change.</param>
        /// <param name="deletedUTC">The UTC time the entities are deleted.</param>
        /// <returns></returns>
        Result Delete(
            IEnumerable<T> entities,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            TUserId deletedBy,
            DateTime? deletedUTC = null);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity to insert. Can't be null.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="insertedBy">The User Id of the person making the change.</param>
        /// <param name="insertUTC">The UTC time the entity is inserted.</param>
        /// <returns></returns>
        Result Insert(
            T entity,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            TUserId insertedBy,
            DateTime? insertUTC = null);

        /// <summary>
        /// Inserts entities
        /// </summary>
        /// <param name="entities">Required. The entities to insert.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="insertedBy">The User Id of the person making the change.</param>
        /// <param name="insertUTC">The UTC time the entity is inserted.</param>
        /// <returns></returns>
        Result Insert(
            IEnumerable<T> entities,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            TUserId insertedBy,
            DateTime? insertUTC = null);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity to update.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="updatedBy">The User Id of the person making the change.</param>
        /// <param name="updatedUTC">The UTC time the entity is updated.</param>
        /// <returns></returns>
        Result<T> Update(
            T entity,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            TUserId updatedBy,
            DateTime? updatedUTC = null);

        /// <summary>
        /// Updates entities.
        /// </summary>
        /// <param name="entities">Required. The entities to update.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="updatedBy">The User Id of the person making the change.</param>
        /// <param name="updatedUTC">The UTC time the entities are updated.</param>
        /// <returns></returns>
        Result Update(
            IEnumerable<T> entities,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            TUserId updatedBy,
            DateTime? updatedUTC = null);
    }
}