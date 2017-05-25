using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EFKit.Providers
{
    /// <summary>
    /// Provides a Utc DateTime to apply on Create/Update/Delete operations.
    /// </summary>
    public interface IDateTimeProvider
    {
        /// <summary>
        /// Gets the UTC DateTime to apply to entities.
        /// </summary>
        DateTime Utc { get; }
    }
}