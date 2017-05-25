using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Entity
{
    /// <summary>
    /// Represents an entity which is timestamped when deleted instead of actually being deleted.
    /// </summary>
    public interface IDeletedUtc
    {
        /// <summary>
        /// Gets or sets the UTC date/time the entity was deleted.
        /// </summary>
        /// <value>
        /// The UTC date/time the entity was deleted.
        /// </value>
        DateTime DeletedUtc { get; set; }
    }
}