using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Exceptions
{
    /// <summary>
    /// Represents an error which occurred because an entity was retrieved as untracked, and then the entity was changed and SaveChanges() was called.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.Exception" />
    public class UpdateUntrackedException<T> :
        Exception
    {
        private readonly T _entity;

        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateUntrackedException{T}" /> class.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <param name="innerEx">The inner exception.</param>
        public UpdateUntrackedException(T entity, Exception innerEx)
            : base($"You tried to update an entity ({entity.GetType().Name}) which was retrieved via .TableNoTracking.", innerEx)
        {
            Contract.Requires<ArgumentNullException>(entity != null, nameof(entity));

            _entity = entity;
        }

        /// <summary>
        /// Gets the entity that was retrieved via the TableNoTracking property.
        /// </summary>
        public T Entity
            => _entity;
    }
}