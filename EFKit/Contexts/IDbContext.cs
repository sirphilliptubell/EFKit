using System;
using System.Data;
using System.Data.Entity;
using EFKit.Entity;

namespace EFKit.Contexts
{
    /// <summary>
    /// Represents a database context.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used when identifying an entity.</typeparam>
    public interface IDbContext<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <returns></returns>
        string GetDatabaseName();

        /// <summary>
        /// Gets the DbSet that contains entities.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        /// <returns>
        /// A DbSet that contains entities
        /// </returns>
        IDbSet<TEntity> Set<TEntity>()
            where TEntity : BaseEntity<TKey>;

        /// <summary>
        /// To get the entity state
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        EntityState GetEntityState(object entity);

        /// <summary>
        /// Attaches the entity to the context and sets it's state as Unchanged.
        /// Fails if the entity is already attached.
        /// </summary>
        /// <typeparam name="UEntity">The type of the entity.</typeparam>
        /// <typeparam name="UKey">The type of the identifier of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        Result AttachAsUnchanged<UEntity, UKey>(UEntity entity)
            where UEntity : BaseEntity<UKey>
            where UKey : struct, IEquatable<UKey>;

        /// <summary>
        /// Tries to find an entity with the same Id that's already attached.
        /// If an existing entity is found, the existing entity is returned with the specified entity's values copied to it.
        /// If an existing entity is not found, the specified entity is attached and returned.
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="attachAs">Indicates how to mark the attached entity, if it wasn't already attached.</param>
        /// <param name="alreadyAttached"><c>true</c> if there was already an attached entity with the specified Id.</param>
        TEntity AsAttached<TEntity>(TEntity entity, AttachAs attachAs, out bool alreadyAttached)
            where TEntity : BaseEntity<TKey>, new();

        /// <summary>
        /// Tries to find an entity with the same Id that's already attached.
        /// If an existing entity is found, the existing entity is returned with the specified entity's values copied to it.
        /// If an existing entity is not found, the specified entity is attached and returned.
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">Entity</param>
        /// <param name="attachAs">Indicates how to mark the attached entity, if it wasn't already attached.</param>
        TEntity AsAttached<TEntity>(TEntity entity, AttachAs attachAs)
            where TEntity : BaseEntity<TKey>, new();

        /// <summary>
        /// Save changes
        /// </summary>
        /// <returns></returns>
        int SaveChanges();

        /// <summary>
        /// Detach an entity
        /// </summary>
        /// <param name="entity">Entity</param>
        void Detach(object entity);

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled (used in EF)
        /// </summary>
        bool AutoDetectChangesEnabled { get; set; }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        IDbContextTransaction BeginTransaction();

        /// <summary>
        /// Begins a transaction using the specified Isolation Level.
        /// </summary>
        /// <param name="isolationLevel">The Database isolation level with which the underlying transaction will be created.</param>
        IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel);

        /// <summary>
        /// Sets the command timeout.
        /// Null sets to the default value.
        /// </summary>
        int? CommandTimeoutSeconds { set; }
    }
}