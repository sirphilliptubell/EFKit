using System;
using EFKit.Contexts;

namespace EFKit.Plugins
{
    /// <summary>
    /// Represnts an object which validates entities before they are stored to the underlying provider.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Gets any errors detected for the specified operation.
        /// Returns Maybe.NoValue if there are no errors.
        /// Returns Maybe.Value if there were errors found.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <param name="operation">The operation to validate for.</param>
        /// <returns></returns>
        Maybe<ValidationResult<T>> GetErrors(T item, Operation operation);
    }
}