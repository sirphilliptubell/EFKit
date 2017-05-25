using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Entity
{
    /// <summary>
    /// Represents an entity which has information about who created it.
    /// </summary>
    /// <typeparam name="TUserId">The type of the user identifier.</typeparam>
    public interface ICreatedBy<TUserId>
        where TUserId : struct
    {
        /// <summary>
        /// Gets or sets the Id of the User this entity was created by.
        /// </summary>
        /// <value>
        /// The Id of the User this entity was created by.
        /// </value>
        TUserId? CreatedBy { get; set; }
    }
}