using System;
using System.Data.Entity;
using EFKit.Contexts;
using EFKit.Entity;

namespace EFKit.Providers
{
    /// <summary>
    /// Handles updating entities before an operation is committed.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    public interface IPreOperationProvider<TKey, TUserId>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
    {
        /// <summary>
        /// Called before the entities are committed.
        /// </summary>
        /// <param name="entity">The entity the operation is going to happen on.</param>
        /// <param name="operation">The type of operation.</param>
        /// <param name="state">The entity's state.</param>
        /// <param name="byUserId">The identifier of the user making the change.</param>
        /// <param name="utc">The UTC time of the operation.</param>
        /// <returns></returns>
        Result BeforeOperation(BaseEntity<TKey> entity, Operation operation, EntityState state, TUserId byUserId, DateTime utc);
    }
}