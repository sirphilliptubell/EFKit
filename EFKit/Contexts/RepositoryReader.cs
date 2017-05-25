using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using EFKit.Entity;

namespace EFKit.Contexts
{
    /// <summary>
    /// Reads a repository.
    /// </summary>
    public sealed class RepositoryReader<T, TKey> :
        IRepositoryReader<T, TKey>
        where TKey : struct, IEquatable<TKey>
        where T : BaseEntity<TKey>, new()
    {
        private readonly IDbContext<TKey> _context;
        private readonly IDbSet<T> _entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryReader{TKey, T}"/> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        public RepositoryReader(IDbContext<TKey> context)
        {
            Contract.Requires<ArgumentNullException>(context != null, nameof(context));

            this._context = context;
            this._entities = context.Set<T>();
        }

        /// <summary>
        /// Gets an entity by it's identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        public Maybe<T> GetById(TKey id)
            //Do not use _entities.Find(id)
            //See http://stackoverflow.com/questions/11686225/dbset-find-method-ridiculously-slow-compared-to-singleordefault-on-id/11688189#comment34876113_11688189
            => _entities.SingleOrDefault(x => x.Id.Equals(id));

        /// <summary>
        /// Sets the command timeout.
        /// Null sets to the default value.
        /// </summary>
        public int? CommandTimeoutSeconds
        { set { this._context.CommandTimeoutSeconds = value; } }

        /// <summary>
        /// Gets a table
        /// </summary>
        public IQueryable<T> Table
            => this._entities;

        /// <summary>
        /// Gets a table with "no tracking" enabled (EF feature) Use it only when you load record(s) only for read-only operations
        /// </summary>
        public IQueryable<T> TableNoTracking
            => this._entities.AsNoTracking();

        /// <summary>Gets a table as tracked or not tracked, depending on a value.</summary>
        /// <param name="tracked">if set to <c>true</c> the table is tracked.</param>
        /// <returns></returns>
        public IQueryable<T> TableWithTracking(bool tracked)
            => tracked
            ? Table
            : TableNoTracking;
    }
}