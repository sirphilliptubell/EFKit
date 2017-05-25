using System;
using System.Data.Entity.Core.Objects;
using EFKit.Entity;

namespace EFKit.Entity
{
    /// <summary>
    /// A standard database Entity.
    /// </summary>
    /// <typeparam name="TKey">The type of the identifier used to identify the entity.</typeparam>
    /// <seealso cref="EFKit.Entity.IIdentifiable{TKey}" />
    /// <seealso cref="System.IEquatable{Entity.BaseEntity{TKey}}" />
    public abstract partial class BaseEntity<TKey> :
        IIdentifiable<TKey>,
        IEquatable<BaseEntity<TKey>>
        where TKey : struct, IEquatable<TKey>
    {
        /// <summary>
        /// Gets or sets the entity's identifier
        /// </summary>
        public TKey Id { get; set; }

        /// <summary>
        /// Determines whether the specified entity is newly created (Id not specified).
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is transient; otherwise, <c>false</c>.
        /// </value>
        private bool IsTransient
            => this.Id.Equals(default(TKey));

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(BaseEntity<TKey> left, BaseEntity<TKey> right)
            => !(left == right);

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(BaseEntity<TKey> left, BaseEntity<TKey> right)
            => Equals(left, right);

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
            => Equals(obj as BaseEntity<TKey>);

        /// <summary>
        /// Gets a value indicating whether the two entities are equivalent.
        /// Returns true if the references are the same, OR if both unproxied types are
        /// the same and their Id's are the same; otherwise returns false.
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public virtual bool Equals(BaseEntity<TKey> other)
        {
            if (other == null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            bool typesMatch = this.GetUnproxiedType().Equals(other.GetUnproxiedType());

            bool idsMatch = !this.IsTransient && this.Id.Equals(other.Id);

            return typesMatch && idsMatch;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
            => IsTransient
            ? base.GetHashCode()
            : Id.GetHashCode();

        /// <summary>
        /// Gets the actual entity type in case the type is an EF proxy object.
        /// </summary>
        /// <returns></returns>
        private Type GetUnproxiedType()
            => ObjectContext.GetObjectType(GetType());
    }
}