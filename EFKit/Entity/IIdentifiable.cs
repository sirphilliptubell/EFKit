using System;

namespace EFKit.Entity
{
    /// <summary>
    /// Represents an entity which can be uniquely identified by a value.
    /// </summary>
    /// <typeparam name="T">The type that Identifies the entity.</typeparam>
    public interface IIdentifiable<T>
        where T : struct, IEquatable<T>
    {
        /// <summary>
        /// Gets or sets the identifier of the entity.
        /// </summary>
        /// <value>
        /// The identifier of the entity.
        /// </value>
        T Id { get; set; }
    }
}