using System;
using EFKit.Entity;

namespace EFKit.Services
{
    /// <summary>
    /// A container for the read/write services for an object type.
    /// This version most likely uses some default write service.
    /// </summary>
    /// <typeparam name="T">The Type the services are for.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <typeparam name="R">The Type of the Reader service.</typeparam>
    public interface IService<T, TKey, TUserId, R>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
        where R : IReadService<T, TKey>
    {
        /// <summary>
        /// Gets the read service.
        /// </summary>
        R Reader { get; }

        /// <summary>
        /// Gets the write service.
        /// </summary>
        IWriteService<T, TKey, TUserId> Writer { get; }
    }

    /// <summary>
    /// A container for the read/write services for an object type.
    /// This version most likely uses some default write service.
    /// </summary>
    /// <typeparam name="T">The Type the services are for.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <typeparam name="R">The Type of the Reader service.</typeparam>
    /// <typeparam name="W">The Type of the Writer service.</typeparam>
    public interface IService<T, TKey, TUserId, R, W>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
        where R : IReadService<T, TKey>
        where W : IWriteService<T, TKey, TUserId>
    {
        /// <summary>
        /// Gets the reader service.
        /// </summary>
        R Reader { get; }

        /// <summary>
        /// Gets the writer service.
        /// </summary>
        W Writer { get; }
    }
}