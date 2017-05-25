using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Entity
{
    /// <summary>
    /// Represents an entity which has the date/time for which it was last modified.
    /// </summary>
    public interface IModifiedUtc
    {
        /// <summary>
        /// Gets or sets the UTC date/time the entity was modified.
        /// </summary>
        /// <value>
        /// The UTC date/time the entity was modified.
        /// </value>
        DateTime? ModifiedUtc { get; set; }
    }
}