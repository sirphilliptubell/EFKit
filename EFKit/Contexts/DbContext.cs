using System;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics.Contracts;
using System.Linq;
using EFKit.Entity;
using EFKit.Exceptions;

namespace EFKit.Contexts
{
    /// <summary>
    /// Wrapper for a DbContext.
    /// </summary>
    public class DbContextWrapper<TKey> :
        IDbContext<TKey>
        where TKey : struct, IEquatable<TKey>
    {
        private readonly DbContext _dbContext;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContext" /> class.
        /// </summary>
        /// <param name="dbContext">The database context.</param>
        public DbContextWrapper(DbContext dbContext)
        {
            Contract.Requires<ArgumentNullException>(dbContext != null, nameof(dbContext));

            _dbContext = dbContext;
        }

        /// <summary>
        /// Gets the name of the database.
        /// </summary>
        /// <returns></returns>
        public string GetDatabaseName()
            => _dbContext.Database.Connection.Database;

        #region Methods

        /// <summary>
        /// Tries to find an entity with the same Id that's already attached.
        /// If an existing entity is found, the existing entity is returned with the specified entity's values copied to it.
        /// If an existing entity is not found, the specified entity is attached and returned with Modified EntityState.
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">The Entity to attach</param>
        /// <param name="attachAs">How to attach the entity.</param>
        /// <param name="alreadyAttached"><c>true</c> if there was already an attached entity with the specified Id.</param>
        /// <returns></returns>
        /// <exception cref="UpdateUntrackedException{TEntity}">Updating an entity that was retrieved by using TableNoTracking.</exception>
        /// <exception cref="NotImplementedException"></exception>
        /// <exception cref="System.NotImplementedException">AttachAs</exception>
        public virtual TEntity AsAttached<TEntity>(TEntity entity, AttachAs attachAs, out bool alreadyAttached)
            where TEntity : BaseEntity<TKey>, new()
        {
            //little hack here until Entity Framework really supports stored procedures
            //otherwise, navigation properties of loaded entities are not loaded until an entity is attached to the context
            var attached = GetByIdIfAttached<TEntity, TKey>(entity);

            if (attached.HasValue)
            {
                alreadyAttached = true;

                try
                {
                    //see if we can read the CurrentValues, if not, then it's likely a proxy object
                    var currentValues = _dbContext.Entry(entity).CurrentValues.PropertyNames;

                    //copy the values to the one that's already attached
                    //no exception above, not a proxy object
                    _dbContext
                        .Entry(attached.Value)
                        .CurrentValues
                        .SetValues(_dbContext.Entry(entity));
                }
                catch (InvalidOperationException ex) when (ex.Message.Contains("Member 'CurrentValues' cannot be called for the entity of type"))
                {
                    //Look for this exception:
                    //Member 'CurrentValues' cannot be called for the entity of type '___' because the entity does not exist in the context. To add an entity to the context call the Add or Attach method of DbSet<___>
                    //Programmer error, don't return Result.Fail()
                    throw new UpdateUntrackedException<TEntity>(entity, ex);
                }

                return attached.Value;
            }
            else //not attached
            {
                alreadyAttached = false;

                //attach
                _dbContext.Set<TEntity>().Add(entity);

                switch (attachAs)
                {
                    case AttachAs.Inserted:
                        _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Added;
                        break;

                    case AttachAs.Modified:
                        _dbContext.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                        break;

                    default:
                        throw new NotImplementedException(attachAs.ToString());
                }

                return entity;
            }
        }

        /// <summary>
        /// Tries to find an entity with the same Id that's already attached.
        /// If an existing entity is found, the existing entity is returned with the specified entity's values copied to it.
        /// If an existing entity is not found, the specified entity is attached and returned with Modified EntityState.
        /// </summary>
        /// <typeparam name="TEntity">TEntity</typeparam>
        /// <param name="entity">The Entity to attach</param>
        /// <param name="attachAs">How to attach the entity as.</param>
        /// <returns></returns>
        public virtual TEntity AsAttached<TEntity>(TEntity entity, AttachAs attachAs)
            where TEntity : BaseEntity<TKey>, new()
            => AsAttached(entity, attachAs, out bool _);

        /// <summary>
        /// Attaches the entity to the context and sets it's state as Unchanged.
        /// Fails if the entity is already attached.
        /// </summary>
        /// <typeparam name="UEntity">The type of the entity.</typeparam>
        /// <typeparam name="UKey">The type of the identifier of the entity.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public Result AttachAsUnchanged<UEntity, UKey>(UEntity entity)
            where UEntity : BaseEntity<UKey>
            where UKey : struct, IEquatable<UKey>
        {
            if (GetByIdIfAttached<UEntity, UKey>(entity).HasValue)
                return Result.Fail("The entity is already attached");
            else
            {
                _dbContext.Set<UEntity>().Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Unchanged;

                return Result.Ok();
            }
        }

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        public IDbContextTransaction BeginTransaction()
            => new DbContextTransactionWrap(_dbContext.Database.BeginTransaction());

        /// <summary>
        /// Begins a transaction using the specified Isolation Level.
        /// </summary>
        /// <param name="isolationLevel">The Database isolation level with which the underlying transaction will be created.</param>
        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
            => new DbContextTransactionWrap(_dbContext.Database.BeginTransaction(isolationLevel));

        /// <summary>
        /// Detaches an entity.
        /// </summary>
        /// <param name="entity">Required. The entity to detach</param>
        public void Detach(object entity)
        {
            Contract.Requires<ArgumentNullException>(entity != null, nameof(entity));

            ((IObjectContextAdapter)this).ObjectContext.Detach(entity);
        }

        /// <summary>
        /// Gets the state of an entity.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public EntityState GetEntityState(object entity)
            => _dbContext.Entry(entity).State;

        /// <summary>
        /// Save all changed made in this context to the underlying data store.
        /// </summary>
        /// <returns></returns>
        public int SaveChanges()
            => _dbContext.SaveChanges();

        /// <summary>
        /// Returns a <see cref="DbSet{Entity}" /> instance for access to entities of the given type in the context and underlying store.
        /// </summary>
        /// <typeparam name="TEntity">Entity type</typeparam>
        public IDbSet<TEntity> Set<TEntity>()
            where TEntity : BaseEntity<TKey>
            => _dbContext.Set<TEntity>();

        /// <summary>
        /// Attempts to get the specified entity by it's Id only if it is attached.
        /// </summary>
        /// <typeparam name="UEntity">The type of the entity.</typeparam>
        /// <typeparam name="UKey">The type of the key.</typeparam>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        private Maybe<UEntity> GetByIdIfAttached<UEntity, UKey>(UEntity entity)
            where UEntity : BaseEntity<UKey>
            where UKey : struct, IEquatable<UKey>
            => _dbContext.Set<UEntity>().Local.FirstOrDefault(x => x.Id.Equals(entity.Id));

        #endregion Methods

        #region Properties

        /// <summary>
        /// Gets or sets a value indicating whether auto detect changes setting is enabled
        /// </summary>
        public virtual bool AutoDetectChangesEnabled
        {
            get => _dbContext.Configuration.AutoDetectChangesEnabled;
            set => _dbContext.Configuration.AutoDetectChangesEnabled = value;
        }

        /// <summary>
        /// Sets the command timeout. In the constructor we have set it for 60 seconds
        /// Null sets to the default value.
        /// </summary>
        public int? CommandTimeoutSeconds
        { set { _dbContext.Database.CommandTimeout = (int?)value; } }

        #endregion Properties
    }
}