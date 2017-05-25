using System;

namespace EFKit.Providers
{
    /// <summary>
    /// The Default DateTimeProvider.
    /// </summary>
    /// <seealso cref="EFKit.Providers.IDateTimeProvider" />
    public class DefaultDateTimeProvider :
        IDateTimeProvider
    {
        /// <summary>
        /// Always returns DateTime.UtcNow.
        /// </summary>
        public DateTime Utc
            => DateTime.UtcNow;
    }
}