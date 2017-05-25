using EFKit.Contexts;

namespace EFKit.Plugins
{
    /// <summary>
    /// Represents an object which may perform a cleaning operation on entities before committing them.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICleaner<T>
    {
        /// <summary>
        /// Cleans the specified entity.
        /// eg: trims relevant strings, limits string lengths, etc..
        /// </summary>
        /// <param name="item">The item.</param>
        /// <param name="operation">The operation that will be performed.</param>
        void Clean(T item, Operation operation);
    }
}