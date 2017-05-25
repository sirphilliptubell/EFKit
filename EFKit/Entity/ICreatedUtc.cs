using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Entity
{
    /// <summary>
    /// Represents an entity which has the date/time for which it was created.
    /// </summary>
    public interface ICreatedUtc
    {
        /// <summary>
        /// Gets or sets the UTC date/time the entity was created.
        /// </summary>
        /// <value>
        /// The UTC date/time the entity was created.
        /// </value>
        DateTime CreatedUtc { get; set; }
    }
}