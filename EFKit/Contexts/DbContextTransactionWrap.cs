using System;
using System.Data.Entity;
using System.Diagnostics.Contracts;

namespace EFKit.Contexts
{
    /// <summary>
    /// This wraps a DbContextTransaction.
    /// </summary>
    public class DbContextTransactionWrap : IDbContextTransaction
    {
        private readonly DbContextTransaction _transaction;

        /// <summary>
        /// Initializes a new instance of the <see cref="DbContextTransactionWrap"/> class.
        /// </summary>
        /// <param name="transaction">The transaction.</param>
        public DbContextTransactionWrap(DbContextTransaction transaction)
        {
            Contract.Requires<ArgumentNullException>(transaction != null, nameof(transaction));

            this._transaction = transaction;
        }

        #region IDbContextTransaction Members

        /// <summary>
        /// Commits the underlying store transaction.
        /// </summary>
        public void Commit()
            => _transaction.Commit();

        /// <summary>
        /// Rolls back the underlying store transaction.
        /// </summary>
        public void Rollback()
            => _transaction.Rollback();

        #endregion IDbContextTransaction Members

        #region IDisposable Members

        /// <summary>
        /// Cleans up this transaction object and ensures the Entity Framework is no longer using the transaction.
        /// </summary>
        public void Dispose()
            => this._transaction.Dispose();

        #endregion IDisposable Members
    }
}