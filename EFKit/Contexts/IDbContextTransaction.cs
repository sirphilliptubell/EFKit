using System;

namespace EFKit.Contexts
{
    /// <summary>
    /// Represents a DbContextTransaction.
    /// </summary>
    public interface IDbContextTransaction :
        IDisposable
    {
        /// <summary>
        /// Commits the underlying store transaction.
        /// </summary>
        void Commit();

        /// <summary>
        /// Rolls back the underlying store transaction.
        /// </summary>
        void Rollback();
    }
}