using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using EFKit.Contexts;
using EFKit.Entity;
using EFKit.Plugins;

namespace EFKit.Services
{
    /// <summary>
    /// The default writer used for changing entity states.
    /// </summary>
    /// <typeparam name="T">The type of the entity</typeparam>
    /// <typeparam name="TKey">The type of the identifier for the entity.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <seealso cref="EFKit.Services.IWriteService{T, TKey, TUserId}" />
    public class WriteService<T, TKey, TUserId>
        : IWriteService<T, TKey, TUserId>
        where T : BaseEntity<TKey>, new()
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
    {
        /// <summary>
        /// Gets the repository that handles the CRUD operations.
        /// </summary>
        protected IRepositoryWriter<T, TKey, TUserId> Repository { get; }

        /// <summary>
        /// Gets the entity validator.
        /// </summary>
        protected Maybe<IValidator<T>> Validator { get; }

        /// <summary>
        /// Gets the entity cleaner.
        /// </summary>
        protected Maybe<ICleaner<T>> Cleaner { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WriteService{T, TKey, TUserId}"/> class.
        /// </summary>
        /// <param name="repository">The repository.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        public WriteService(
            IRepositoryWriter<T, TKey, TUserId> repository,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner)
        {
            Contract.Requires<ArgumentNullException>(repository != null, nameof(repository));

            this.Repository = repository;
            this.Validator = validator;
            this.Cleaner = cleaner;
        }

        /// <summary>
        /// Updates entities with the specified Ids.
        /// </summary>
        /// <param name="ids">Required. The ids of the entities to change.</param>
        /// <param name="alter">Required. The action to take on each entity.</param>
        /// <param name="byUserId">The Id of the user doing the update.</param>
        /// <returns></returns>
        public Result Update(IEnumerable<TKey> ids, Action<T> alter, TUserId byUserId)
        {
            Contract.Requires<ArgumentNullException>(ids != null, nameof(ids));
            Contract.Requires<ArgumentNullException>(alter != null, nameof(alter));

            //in case the id's are coming from a dbcontext, lets detach them
            ids = ids.ToList();

            return !ids.Any()
                ? Result.Ok()
                : this.Repository
                    .BeginTransaction()
                    .CommitOrRollback(() =>
                    {
                        var entities = GetByIds(ids)
                            .ToList(); //detach

                        foreach (var entity in entities)
                            alter(entity);

                        //perform all updates with a single SaveChanges()
                        return this.Repository.Update(entities, Validator, Cleaner, byUserId);
                    });
        }

        /// <summary>
        /// Updates the entity with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the entity to alter.</param>
        /// <param name="alter">Required. The action to take on the entity.</param>
        /// <param name="byUserId">The Id of the user doing the update.</param>
        /// <returns>
        /// The Result of the change, along with the changed entity (not tracked).
        /// </returns>
        public virtual Result<T> Update(TKey id, Action<T> alter, TUserId byUserId)
            => this.Update(id, item =>
            {
                alter(item);
                return Result.Ok<T>(item);
            }, byUserId);

        /// <summary>
        /// Updates the entity with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the entity to alter.</param>
        /// <param name="alter">Required. The action to take on the entity.</param>
        /// <param name="byUserId">The Id of the user doing the update.</param>
        /// <returns>
        /// The Result of the change, along with the changed entity (not tracked).
        /// </returns>
        public virtual Result<T> Update(TKey id, Func<T, Result> alter, TUserId byUserId)
        {
            Contract.Requires<ArgumentNullException>(alter != null, nameof(alter));

            var alteredEntry = this.GetById(id);

            if (alteredEntry.HasNoValue)
                return Result.Fail<T>($"Could not find {typeof(T).Name} with id {id} to update.");
            else
            {
                var alterResult = alter(alteredEntry.Value);
                if (alterResult.IsFailure)
                    return alterResult.ToTypedResult<T>();
                else
                    return this.Repository.Update(alteredEntry.Value, this.Validator, this.Cleaner, byUserId);
            }
        }

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="item">Required. The entity to insert.</param>
        /// <param name="byUserId">The by user identifier.</param>
        /// <returns>
        /// The result of the Insert operation.
        /// </returns>
        public virtual Result Insert(T item, TUserId byUserId)
            => this.Repository.Insert(item, this.Validator, this.Cleaner, byUserId);

        /// <summary>
        /// Deletes the entity with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the entity to delete.</param>
        /// <param name="byUserId">The Id of the user doing the delete.</param>
        /// <returns>
        /// The Result of the Delete operation.
        /// </returns>
        public virtual Result Delete(TKey id, TUserId byUserId)
        {
            var existing = this.GetById(id);

            if (existing.HasNoValue)
                return Result.Fail($"Could not delete {typeof(T).Name} because #{id} doesn't exist.");
            else
                return this.Repository.Delete(existing.Value, this.Validator, this.Cleaner, byUserId);
        }

        /// <summary>
        /// Tries to get the entity with the specified Id.
        /// Returned entity is not tracked.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns></returns>
        private Maybe<T> GetById(TKey id)
            => this.Repository
            .TableWithTracking
            .Where(x => x.Id.Equals(id))
            .SingleOrDefault();

        /// <summary>
        /// Tries to get the entity with the specified Id.
        /// Returned entity is not tracked.
        /// </summary>
        /// <param name="ids">The Ids.</param>
        /// <returns></returns>
        private IQueryable<T> GetByIds(IEnumerable<TKey> ids)
            => this.Repository
            .TableWithTracking
            .Where(x => ids.Contains(x.Id));

        /// <summary>
        /// Removes all the children in a relationship and replaces with the specified entries.
        /// </summary>
        /// <typeparam name="U">The type of the child entity.</typeparam>
        /// <typeparam name="UKey">The type of the identifier used on the child.</typeparam>
        /// <param name="id">The Id of the parent Entity.</param>
        /// <param name="selectChildren">Required. The method that selects the children collection of the parent.</param>
        /// <param name="childIds">Required. The new child ids the collection will have.</param>
        /// <param name="byUserId">The by user identifier.</param>
        /// <returns>
        /// The result of the operation.
        /// </returns>
        protected Result ReplaceChildren<U, UKey>(
            TKey id,
            Func<T, ICollection<U>> selectChildren,
            IEnumerable<UKey> childIds,
            TUserId byUserId)
            where U : BaseEntity<UKey>, new()
            where UKey : struct, IEquatable<UKey>
        {
            Contract.Requires<ArgumentNullException>(selectChildren != null, nameof(selectChildren));
            Contract.Requires<ArgumentNullException>(childIds != null, nameof(childIds));

            return this.Update(id, parent =>
            {
                var collectionToModify = selectChildren(parent);

                var oldSet = collectionToModify.ToHashSet();

                var newSet =
                      childIds
                      .Select(x => new U() { Id = x })
                      .ToHashSet();

                //remove children that should be removed
                foreach (var oldItem in oldSet.ToList())
                    if (!newSet.Contains(oldItem))
                    {
                        oldSet.Remove(oldItem);
                        collectionToModify.Remove(oldItem);
                    }

                var needsAdding = new List<U>();
                //find the children that should be added
                foreach (var newItem in newSet.ToList())
                    if (!oldSet.Contains(newItem))
                        needsAdding.Add(newItem);

                //attach the new children but keep them as "unchanged" so we don't insert them
                var attachResult =
                      Result.CombineSequential(
                          needsAdding
                          .Select(x => this.Repository.AttachAsUnchanged<U, UKey>(x))
                          );

                if (attachResult.IsFailure)
                    return attachResult;
                else
                {
                    //update the children collection
                    foreach (var newItem in needsAdding)
                        collectionToModify.Add(newItem);
                    return Result.Ok();
                }
            }, byUserId);
        }
    }
}