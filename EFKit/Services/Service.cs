using System;
using EFKit.Entity;

namespace EFKit.Services
{
    /// <summary>
    /// A container for the read/write services for an object type.
    /// This version most likely uses the DefaultWriter service.
    /// </summary>
    /// <typeparam name="T">The type of the entity for the service.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <typeparam name="R">The Type of the Reader service.</typeparam>
    /// <seealso cref="EFKit.Services.IService{T, TKey, TUserId, R}" />
    public class Service<T, TKey, TUserId, R> : IService<T, TKey, TUserId, R>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
        where R : IReadService<T, TKey>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Service{T, TKey, TUserId, R}"/> class.
        /// </summary>
        /// <param name="reader">The reader service.</param>
        /// <param name="writer">The writer service.</param>
        public Service(R reader, IWriteService<T, TKey, TUserId> writer)
        {
            this.Reader = reader;
            this.Writer = writer;
        }

        /// <summary>
        /// Gets the read service.
        /// </summary>
        public R Reader { get; }

        /// <summary>
        /// Gets the write service.
        /// </summary>
        public IWriteService<T, TKey, TUserId> Writer { get; }
    }

    /// <summary>
    /// A container for the read/write services for an object type.
    /// </summary>
    /// <typeparam name="T">The type of the entity for the service.</typeparam>
    /// <typeparam name="TKey">The type of the entity's identifier.</typeparam>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    /// <typeparam name="R">The Type of the Reader service.</typeparam>
    /// <typeparam name="W">The Type of the Writer Service.</typeparam>
    /// <seealso cref="EFKit.Services.IService{T, TKey, TUserId, R}" />
    public class Service<T, TKey, TUserId, R, W> : IService<T, TKey, TUserId, R, W>
        where T : BaseEntity<TKey>
        where TKey : struct, IEquatable<TKey>
        where TUserId : struct
        where R : IReadService<T, TKey>
        where W : IWriteService<T, TKey, TUserId>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Service{T, TKey, TUserId, R, W}"/> class.
        /// </summary>
        /// <param name="reader">The reader service.</param>
        /// <param name="writer">The writer service.</param>
        public Service(R reader, W writer)
        {
            this.Reader = reader;
            this.Writer = writer;
        }

        /// <summary>
        /// Gets the read service.
        /// </summary>
        public R Reader { get; }

        /// <summary>
        /// Gets the write service.
        /// </summary>
        public W Writer { get; }
    }
}