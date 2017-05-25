using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using EFKit.Contexts;

namespace EFKit.Plugins
{
    /// <summary>
    /// Represents the result of validating an object before an Insert/Update/Delete operation.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <seealso cref="System.IErrorContainer" />
    [DebuggerDisplay("{DebuggerDisplay,nq")]
    public sealed class ValidationResult<T> :
        IErrorContainer
    {
        /// <summary>
        /// Gets the object that was validated.
        /// </summary>
        public T Object { get; }

        /// <summary>
        /// Gets any error messages for the object.
        /// </summary>
        public IEnumerable<string> Errors { get; }

        /// <summary>
        /// Gets operation that was being processed when the error(s) may have occurred.
        /// </summary>
        public Operation Operation { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationResult{T}"/> class.
        /// </summary>
        /// <param name="obj">The object the validation occurred on. Can't be null.</param>
        /// <param name="operation">The operation that was about to be attempted on the entity.</param>
        /// <param name="errors">The errors for the object.</param>
        public ValidationResult(T obj, Operation operation, IEnumerable<string> errors)
        {
            Contract.Requires<ArgumentNullException>(obj != null, nameof(obj));
            Contract.Requires<ArgumentNullException>(errors != null, nameof(errors));

            this.Object = obj;
            this.Operation = operation;
            this.Errors = errors;
        }

        /// <summary>
        /// Gets a value indicating whether this instance has any errors.
        /// </summary>
        internal bool HasErrors
            => this.Errors.Any();

        /// <summary>
        /// Combines the error messages.
        /// </summary>
        public string Error
            => this.HasErrors
            ? $"The following errors occurred when trying to {Operation} '{typeof(T).Name}':"
            + Environment.NewLine
            + this.Errors.Join(Environment.NewLine)
            : "No errors.";

        /// <summary>
        /// Returns the value of the .ErrorText property.
        /// </summary>
        public override string ToString()
            => Error;

        /// <summary>
        /// Returns a Result.OK() if there are no errors.
        /// Returns a Result.Fail() if there are errors.
        /// </summary>
        /// <returns></returns>
        public Result<T> ToResult()
            => this.HasErrors
            ? Result.Fail<T>(this.Error)
            : Result.Ok(this.Object);

        private string DebuggerDisplay
            => this.ToResult().ToString();
    }
}