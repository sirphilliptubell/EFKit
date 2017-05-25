using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.Contracts;
using System.Linq;
using EFKit.Contexts;
using EFKit.Entity;

namespace EFKit.Services
{
    /// <summary>
    /// A basic entity reader.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    public abstract class ReadService<T, TKey>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReadService{T, TKey}"/> class.
        /// </summary>
        /// <param name="dbContext">Required. The database context.</param>
        /// <param name="repository">Required. The repository.</param>
        public ReadService(
            IDbContext<TKey> dbContext,
            IRepositoryReader<T, TKey> repository)
        {
            Contract.Requires<ArgumentNullException>(dbContext != null, nameof(dbContext));
            Contract.Requires<ArgumentNullException>(repository != null, nameof(repository));

            this.DbContext = dbContext;
            this.Repository = repository;
        }

        /// <summary>
        /// Gets the database context.
        /// </summary>
        protected IDbContext<TKey> DbContext { get; }

        /// <summary>
        /// Gets the repository.
        /// </summary>
        protected IRepositoryReader<T, TKey> Repository { get; }

        /// <summary>
        /// Get all the Entities in this Repository.
        /// None of the Entities are change-tracked.
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> All()
            => Repository.TableNoTracking;

        /// <summary>
        /// Gets a value indicating if an entity with the specified Id exists in the Repository.
        /// </summary>
        /// <param name="id">The Id.</param>
        /// <returns></returns>
        public virtual bool Exists(TKey id)
            => GetById(id).HasValue;

        /// <summary>
        /// Gets the entity with the specified Id.
        /// </summary>
        /// <param name="id">The Id of the entity.</param>
        /// <param name="tracked">if set to <c>true</c> the entity will be change-tracked.</param>
        /// <returns></returns>
        public virtual Maybe<T> GetById(TKey id, bool tracked = false)
            => Repository
            .TableWithTracking(tracked)
            .Where(x => x.Id.Equals(id))
            .SingleOrDefault();

        /// <summary>
        /// Gets the entities with the specified Ids.
        /// </summary>
        /// <param name="ids">The Ids of the entities.</param>
        /// <param name="tracked">if set to <c>true</c> the entities will be change-tracked.</param>
        /// <returns></returns>
        public virtual IQueryable<T> GetByIds(IEnumerable<TKey> ids, bool tracked = false)
        {
            Contract.Requires<ArgumentNullException>(ids != null, nameof(ids));

            var result =
                Repository
                .TableWithTracking(tracked)
                .Where(x => ids.Contains(x.Id));

            return result;
        }

        /// <summary>
        /// Gets the name of the database used by the Repository.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
            => this.DbContext.GetDatabaseName();
    }
}