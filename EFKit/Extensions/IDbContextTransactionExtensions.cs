using System.Diagnostics.Contracts;
using EFKit;
using EFKit.Contexts;

namespace System
{
    /// <summary>
    /// Extension methods for IDbContextTransaction.
    /// </summary>
    public static class IDbContextTransactionExtensions
    {
        /// <summary>
        /// Performs an action.
        /// Commits the transaction if there are no exceptions thrown and the action's Result is successful.
        /// Rollsback otherwise.
        /// </summary>
        /// <param name="tran">The transaction.</param>
        /// <param name="action">The action.</param>
        /// <returns></returns>
        public static Result CommitOrRollback(
            this IDbContextTransaction tran,
            Func<Result> action)
            => CommitOrRollback(tran, action, null);

        /// <summary>
        /// Performs an action.
        /// Commits the transaction if there are no exceptions thrown and the action's Result is successful.
        /// Rollsback otherwise.
        /// </summary>
        /// <param name="tran">The transaction.</param>
        /// <param name="action">The action.</param>
        /// <param name="transformException">Optional. Transforms any exception that is caught.</param>
        /// <returns></returns>
        public static Result CommitOrRollback(
            this IDbContextTransaction tran,
            Func<Result> action,
            Maybe<Func<Exception, Exception>> transformException)
        {
            Contract.Requires<ArgumentNullException>(tran != null, nameof(tran));
            Contract.Requires<ArgumentNullException>(action != null, nameof(action));

            using (tran)
            {
                try
                {
                    var result = action();
                    if (result.IsSuccess)
                        tran.Commit();
                    else
                        tran.Rollback();
                    return result;
                }
                catch (Exception ex)
                {
                    tran.Rollback();

                    var newEx = transformException.Unwrap()?.Invoke(ex) ?? ex;
                    return Result.Fail(newEx);
                }
            }
        }
    }
}