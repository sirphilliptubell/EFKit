using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics.Contracts;
using System.Linq;
using EFKit.Contexts;
using EFKit.Entity;

namespace EFKit.Providers
{
    /// <summary>
    /// Provides a variety of default operations to apply based on the interfaces the entities implement.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <seealso cref="EFKit.Providers.IPreOperationProvider{TKey, TUserId}" />
    internal class DefaultPreOperationProvider<TKey, TUserId> :
        IPreOperationProvider<TKey, TUserId>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
    {
        private readonly IReadOnlyCollection<PreCommitDelegate> _onChanges;

        public delegate Result PreCommitDelegate(BaseEntity<TKey> item, Operation operation, EntityState state, TUserId deleteBy, DateTime utc);

        public DefaultPreOperationProvider()
        {
            _onChanges = new List<PreCommitDelegate>()
            {
                UpdateCreatedBy,
                UpdateCreatedUtc,

                UpdateModifiedBy,
                UpdateModifiedUtc,

                UpdateDeletedBy,
                UpdateDeletedUtc,

                UpdateIsDeleted,
                UpdateIsHistoric,

                Custom
            };
        }

        public DefaultPreOperationProvider(IEnumerable<PreCommitDelegate> onChangeDelegates)
        {
            Contract.Requires<ArgumentNullException>(onChangeDelegates != null, nameof(onChangeDelegates));

            _onChanges = new List<PreCommitDelegate>(onChangeDelegates);
        }

        public Result BeforeOperation(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId byUserId, DateTime utc)
            => _onChanges
            .Select(x => x(entity, operation, state, byUserId, utc))
            .CombineAll();

        protected virtual Result Custom(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId byUserId, DateTime utc)
            => Result.Ok();

        protected virtual Result UpdateCreatedBy(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId createdBy, DateTime utc)
        {
            if (operation == Operation.Update && entity is ICreatedBy<TUserId> x)
                x.CreatedBy = createdBy;

            return Result.Ok();
        }

        protected virtual Result UpdateCreatedUtc(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId createdBy, DateTime utc)
        {
            if (operation == Operation.Insert && entity is ICreatedUtc x)
                x.CreatedUtc = utc;

            return Result.Ok();
        }

        protected virtual Result UpdateDeletedBy(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId deletedBy, DateTime utc)
        {
            if (operation == Operation.Update && entity is IDeletedBy<TUserId> x)
                x.DeletedBy = deletedBy;

            return Result.Ok();
        }

        protected virtual Result UpdateDeletedUtc(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId deleteBy, DateTime utc)
        {
            if (operation == Operation.Delete && entity is IDeletedUtc x)
                x.DeletedUtc = utc;

            return Result.Ok();
        }

        protected virtual Result UpdateIsDeleted(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId deleteBy, DateTime utc)
        {
            if (operation == Operation.Delete && entity is IIsDeleted x)
                x.IsDeleted = true;

            return Result.Ok();
        }

        protected virtual Result UpdateIsHistoric(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId TODOBy, DateTime utc)
        {
            throw new NotImplementedException("TODO, update the param names as well");
        }

        protected virtual Result UpdateModifiedBy(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId modifiedBy, DateTime utc)
        {
            if (operation == Operation.Update && entity is IModifiedBy<TUserId> x)
                x.ModifiedBy = modifiedBy;

            return Result.Ok();
        }

        protected virtual Result UpdateModifiedUtc(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId modifiedBy, DateTime utc)
        {
            if (operation == Operation.Update && entity is IModifiedUtc x)
                x.ModifiedUtc = utc;

            return Result.Ok();
        }
    }
}