using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using EFKit.Entity;
using EFKit.Plugins;
using EFKit.Providers;

namespace EFKit.Contexts
{
    /// <summary>
    /// Writes to a repository
    /// </summary>
    /// <remarks>
    /// Order of operation:
    /// 1. User calls Insert/Update/Delete on entities.
    /// 2. ExecuteRequest()
    /// 3. -> IValidator.Clean()                        //Clean the properties of the entity, eg: change a null string to empty if necessary
    /// 4. -> IPreOperationProvider.BeforeOperation()   //Update the properties of the entity, eg: set ModifiedBy, DeletedUtc, etc...
    /// 5. -> IValidator.Validate()
    /// 6. -> Operate()                                 //eg: change the state of the entity to "modified" if doing an Update
    /// </remarks>
    public sealed class RepositoryWriter<T, TKey, TUserId> :
        IRepositoryWriter<T, TKey, TUserId>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
        where T : BaseEntity<TKey>, new()
    {
        private readonly IDbContext<TKey> _context;
        private readonly IDbSet<T> _entities;
        private readonly IPreOperationProvider<TKey, TUserId> _preOpProvider;
        private readonly IDateTimeProvider _timeProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="RepositoryWriter{TKey, TUserId, T}" /> class.
        /// </summary>
        /// <param name="context">The db context.</param>
        /// <param name="timeProvider">
        /// The time provider.
        /// If null a <see cref="DefaultDateTimeProvider"/> is used.
        /// </param>
        /// <param name="preOperationUpdater">
        /// An object which may perform actions just before committing entities.
        /// If null a <see cref="DefaultPreOperationProvider{TKey, TUserId}"/> is used.
        /// </param>
        public RepositoryWriter(
            IDbContext<TKey> context,
            IDateTimeProvider timeProvider = null,
            IPreOperationProvider<TKey, TUserId> preOperationUpdater = null)
        {
            Contract.Requires<ArgumentNullException>(context != null, nameof(context));

            _context = context;
            _entities = context.Set<T>();
            _timeProvider = timeProvider ?? new DefaultDateTimeProvider();
            _preOpProvider = preOperationUpdater ?? new DefaultPreOperationProvider<TKey, TUserId>();
        }

        #region Public Methods

        /// <summary>
        /// Gets the table with change tracking enabled.
        /// </summary>
        public IQueryable<T> TableWithTracking
            => this._entities;

        /// <summary>
        /// Begins a transaction.
        /// </summary>
        /// <returns></returns>
        public IDbContextTransaction BeginTransaction()
            => this._context.BeginTransaction();

        /// <summary>
        /// Begins a transaction using the specified Isolation Level.
        /// </summary>
        /// <param name="isolationLevel">The Database isolation level with which the underlying transaction will be created.</param>
        public IDbContextTransaction BeginTransaction(IsolationLevel isolationLevel)
            => this._context.BeginTransaction(isolationLevel);

        /// <summary>
        /// Deletes the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity to delete.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="deletedBy">The User Id of the person making the change.</param>
        /// <param name="deletedUTC">The UTC time the entity is deleted.</param>
        /// <returns></returns>
        public Result Delete(T entity, Maybe<IValidator<T>> validator, Maybe<ICleaner<T>> cleaner, TUserId deletedBy, DateTime? deletedUTC = null)
            => ExecuteRequest(entity, validator, cleaner, Operation.Delete, OperateDelete, deletedBy, deletedUTC);

        /// <summary>
        /// Deletes the specified entities.
        /// </summary>
        /// <param name="entities">Required. The entities to delete.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="deletedBy">The User Id of the person making the change.</param>
        /// <param name="deletedUTC">The UTC time the entities are deleted.</param>
        /// <returns></returns>
        public Result Delete(IEnumerable<T> entities, Maybe<IValidator<T>> validator, Maybe<ICleaner<T>> cleaner, TUserId deletedBy, DateTime? deletedUTC = null)
            => ExecuteRequest(entities.ToList(), validator, cleaner, Operation.Delete, OperateDelete, deletedBy, deletedUTC);

        /// <summary>
        /// Inserts the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity to insert. Can't be null.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="insertedBy">The User Id of the person making the change.</param>
        /// <param name="insertUTC">The UTC time the entity is inserted.</param>
        /// <returns></returns>
        public Result Insert(T entity, Maybe<IValidator<T>> validator, Maybe<ICleaner<T>> cleaner, TUserId insertedBy, DateTime? insertUTC = null)
            => ExecuteRequest(entity, validator, cleaner, Operation.Insert, OperateInsert, insertedBy, insertUTC);

        /// <summary>
        /// Inserts entities
        /// </summary>
        /// <param name="entities">Required. The entities to insert.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="insertedBy">The User Id of the person making the change.</param>
        /// <param name="insertUTC">The UTC time the entity is inserted.</param>
        /// <returns></returns>
        public Result Insert(IEnumerable<T> entities, Maybe<IValidator<T>> validator, Maybe<ICleaner<T>> cleaner, TUserId insertedBy, DateTime? insertUTC = null)
            => ExecuteRequest(entities.ToList(), validator, cleaner, Operation.Insert, OperateInsert, insertedBy, insertUTC);

        /// <summary>
        /// Updates the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity to update.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="updatedBy">The User Id of the person making the change.</param>
        /// <param name="updatedUTC">The UTC time the entity is updated.</param>
        /// <returns></returns>
        public Result<T> Update(T entity, Maybe<IValidator<T>> validator, Maybe<ICleaner<T>> cleaner, TUserId updatedBy, DateTime? updatedUTC = null)
            => ExecuteRequest(entity, validator, cleaner, Operation.Update, OperateUpdate, updatedBy, updatedUTC)
            .ToTypedResult(entity);

        /// <summary>
        /// Updates entities.
        /// </summary>
        /// <param name="entities">Required. The entities to update.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="updatedBy">The User Id of the person making the change.</param>
        /// <param name="updatedUTC">The UTC time the entities are updated.</param>
        /// <returns></returns>
        public Result Update(IEnumerable<T> entities, Maybe<IValidator<T>> validator, Maybe<ICleaner<T>> cleaner, TUserId updatedBy, DateTime? updatedUTC = null)
            => ExecuteRequest(entities.ToList(), validator, cleaner, Operation.Update, OperateUpdate, updatedBy, updatedUTC);

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Attaches the entity to the context and sets it's state as Unchanged.
        /// Fails if the entity is already attached.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns></returns>
        public Result AttachAsUnchanged<U, UKey>(U entity)
            where U : BaseEntity<UKey>
            where UKey : struct, IEquatable<UKey>
            => this._context.AttachAsUnchanged<U, UKey>(entity);

        /// <summary>
        /// Performs an operation of the specified entity.
        /// </summary>
        /// <param name="entity">Required. The entity.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="operationType">The type of the operation being performed.</param>
        /// <param name="operate">Required. The action that does the operation.</param>
        /// <param name="by">The User Id of the person making the change.</param>
        /// <param name="utc">The UTC time the operation is happening.</param>
        /// <returns></returns>
        private Result ExecuteRequest(
            T entity,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            Operation operationType,
            Func<T, Result<T>> operate,
            TUserId by,
            DateTime? utc = null)
            => ExecuteRequest(new T[] { entity }, validator, cleaner, operationType, operate, by, utc);

        /// <summary>
        /// Performs an operation on the specified entities.
        /// </summary>
        /// <param name="entitiesToOperateOn">Required. The entities to operate on.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="cleaner">The entity cleaner.</param>
        /// <param name="operation">The type of the operation being performed.</param>
        /// <param name="operate">Required. The action that does the operation.</param>
        /// <param name="by">The User Id of the person making the change.</param>
        /// <param name="utc">The UTC time the operation is happening.</param>
        /// <returns></returns>
        private Result ExecuteRequest(
            IEnumerable<T> entitiesToOperateOn,
            Maybe<IValidator<T>> validator,
            Maybe<ICleaner<T>> cleaner,
            Operation operation,
            Func<T, Result<T>> operate,
            TUserId by,
            DateTime? utc = null)
        {
            Contract.Requires<ArgumentNullException>(entitiesToOperateOn != null, nameof(entitiesToOperateOn));
            Contract.Requires<ArgumentNullException>(operate != null, nameof(operate));

            //since 'entities' is IEnumerable, the entities could be coming from a yielding method
            //convert to a concrete list so we can modify the entities without loosing the changes.
            var concreteList =
                entitiesToOperateOn
                .ToList();

            //use the timeprovider to get the time if none was specified
            utc = utc ?? _timeProvider.Utc;

            //modify the entities where needed
            foreach (var entity in concreteList)
            {
                //perform any necessary cleanup
                cleaner.IfValue(c => c.Clean(entity, operation));

                //do any final operations necessary, eg: set ModifiedBy, CreatedUTC, etc...
                _preOpProvider.BeforeOperation(entity, operation, _context.GetEntityState(entity), by, utc.Value);
            }

            return
                //validate the entities
                Validate(concreteList, validator, operation)
                .OnSuccess(entities =>
                    entities
                    //Do the Operation work
                    //this may alter the entities
                    .Select(operate)
                    //combine the results into a single result to detect any errors
                    .CombineAll()
                )
                //Commit the changes
                .OnSuccess(_ =>
                {
                    try
                    {
                        this._context.SaveChanges();
                        return Result.Ok();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        var newEx = TryConvertDbValidationException(dbEx);
                        return Result.Fail(newEx);
                    }
                });
        }

        /// <summary>
        /// Prepares the entity for deletion.
        /// Removes the entity from the DbContext.
        /// </summary>
        /// <param name="entity">The entity being deleted.</param>
        /// <returns></returns>
        private Result<T> OperateDelete(T entity)
            => Result.Ok(this._entities.Remove(entity));

        /// <summary>
        /// Prepares the entity for insertion.
        /// Attached to the DbContext.
        /// </summary>
        /// <param name="entity">The entity being inserted.</param>
        /// <returns></returns>
        private Result<T> OperateInsert(T entity)
            => Result.Ok(this._context.AsAttached(entity, AttachAs.Inserted));

        /// <summary>
        /// Prepares the entity for updating.
        /// Attached to the DbContext.
        /// </summary>
        /// <param name="entity">The entity being updated.</param>
        /// <returns></returns>
        private Result<T> OperateUpdate(T entity)
            => Result.Ok(this._context.AsAttached(entity, AttachAs.Modified));

        /// <summary>
        /// Tries the convert a database validation exception into an exception where
        /// the properties and errors are easy to understand.
        /// </summary>
        /// <param name="ex">The ex.</param>
        /// <returns></returns>
        private Exception TryConvertDbValidationException(Exception ex)
        {
            //If there was a validation issue, provide a summary of the errors in the message.
            if (ex is DbEntityValidationException dbex)
            {
                var sb = new StringBuilder("The following properties did not validate:");
                foreach (var validationErrors in dbex.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        sb.AppendLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");

                return new DbEntityValidationException(sb.ToString(), dbex.EntityValidationErrors, dbex);
            }
            else
                return ex;
        }

        /// <summary>
        /// Returns the Result of the validations of the entities.
        /// Note: If more than one entity has errors, the Result.Fail's error message
        /// is only the error message for the first error-entity.
        /// </summary>
        /// <param name="entities">Required. The entities to clean and validate.</param>
        /// <param name="validator">The entity validator.</param>
        /// <param name="operationType">Type of the operation.</param>
        /// <returns>
        /// The result of the validations of the entities.
        /// </returns>
        private Result<IEnumerable<T>> Validate(
            IEnumerable<T> entities,
            Maybe<IValidator<T>> validator,
            Operation operationType)
        {
            Contract.Requires<ArgumentNullException>(entities != null, nameof(entities));

            if (validator.HasNoValue)
                return Result.Ok(entities);

            var firstError = entities
                //validate all of them
                .Select(x => validator.Unwrap().GetErrors(x, operationType))
                .OnlyValues()
                .FirstOrDefault();

            if (firstError != null)
                return Result.Fail<IEnumerable<T>>(firstError);
            else
                return Result.Ok(entities);
        }

        #endregion Private Methods
    }
}