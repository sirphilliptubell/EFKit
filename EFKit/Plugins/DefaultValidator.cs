using System;
using System.Collections.Generic;
using System.Linq;
using EFKit.Contexts;

namespace EFKit.Plugins
{
    /// <summary>
    /// Basic Validation function for entities.
    /// </summary>
    /// <typeparam name="T">The type of the entity being validated.</typeparam>
    /// <typeparam name="TReader">A source for reading existing entities in case validation of this entity requires you to validate against existing data.</typeparam>
    /// <seealso cref="EFKit.Plugins.IValidator{T}" />
    public abstract class BaseValidator<T, TReader>
        : IValidator<T>
        where TReader : class
    {
        /// <summary>
        /// Gets an object for reading existing data in case the entity must be compared to existing information.
        /// </summary>
        protected Maybe<TReader> Reader { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseValidator{T, TReader}"/> class.
        /// </summary>
        /// <param name="reader">An object used to read existing information.</param>
        public BaseValidator(TReader reader)
        {
            this.Reader = reader;
        }

        /// <summary>
        /// Gets any errors detected for the specified operation.
        /// Returns Maybe.NoValue if there are no errors.
        /// Returns Maybe.Value if there were errors found.
        /// </summary>
        /// <param name="entity">The item to validate.</param>
        /// <param name="operation">The operation to validate for.</param>
        /// <returns></returns>
        /// <exception cref="System.NotImplementedException"></exception>
        public Maybe<ValidationResult<T>> GetErrors(T entity, Operation operation)
        {
            IEnumerable<string> errors;

            switch (operation)
            {
                case Operation.Insert:
                    errors = GetInsertErrors(entity);
                    break;

                case Operation.Update:
                    errors = GetUpdateErrors(entity);
                    break;

                case Operation.Delete:
                    errors = GetDeleteErrors(entity);
                    break;

                default:
                    throw new NotImplementedException(operation.ToString());
            }

            //create a concrete list in the case the validator yielded results
            var result = errors.ToList();

            return
                result.Count > 0
                ? new ValidationResult<T>(entity, operation, result)
                : null;
        }

        /// <summary>
        /// Gets the errors detected for an Insert operation.
        /// Unless overridden, this is the same as the GetUpdateErrors() method.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetInsertErrors(T item)
            => GetUpdateErrors(item);

        /// <summary>
        /// Gets the errors detected for an Update operation.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <returns>An empty collection.</returns>
        protected virtual IEnumerable<string> GetUpdateErrors(T item)
        { yield break; }

        /// <summary>
        /// Gets the errors detected for an Delete operation.
        /// </summary>
        /// <param name="item">The item to validate.</param>
        /// <returns>An empty collection.</returns>
        protected virtual IEnumerable<string> GetDeleteErrors(T item)
        { yield break; }

        /// <summary>
        /// Cleans the specified entity.
        /// </summary>
        /// <param name="entity">The entity to clean.</param>
        public virtual void Clean(T entity)
        { }

        /// <summary>
        /// Trims whitespace from the string.
        /// </summary>
        /// <param name="s">The string to clean.</param>
        /// <param name="convertNullToWhitespace">if set to <c>true</c>, nulls are converted to whitespace.
        /// are converted to ann empty string.</param>
        /// <returns></returns>
        protected string CleanStringForDb(string s, bool convertNullToWhitespace)
        {
            if (s == null && convertNullToWhitespace)
                s = string.Empty;
            else
                s = s?.Trim();

            return s;
        }

        /// <summary>
        /// Returns true if the string is null, or it's length is longer than specified.
        /// </summary>
        /// <param name="s">The string to validate.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        protected bool LengthValidationFails(Maybe<string> s, int maxLength)
            => s.HasNoValue
            ? false
            : s.Value.Length > maxLength;

        /// <summary>
        /// Gets the standard error message for when the length of a string is too long.
        /// </summary>
        /// <param name="name">The name of the property.</param>
        /// <param name="maxLength">The maximum length.</param>
        /// <returns></returns>
        protected string LengthErrorMsg(string name, int maxLength)
            => $"{name} must be {maxLength} characters or less.";
    }
}