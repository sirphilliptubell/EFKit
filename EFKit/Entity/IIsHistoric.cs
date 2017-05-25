using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Entity
{
    /// <summary>
    /// Represents an entity which when it is modified, it is marked as historical and a new entity takes it's place
    /// which is not historical.
    /// </summary>
    public interface IIsHistoric
    {
        /// <summary>
        /// Gets or sets a value indicating whether this instance represents an earlier version of some data.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is historic; otherwise, <c>false</c>.
        /// </value>
        bool IsHistoric { get; set; }
    }
}