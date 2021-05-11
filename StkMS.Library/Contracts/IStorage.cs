using System.Collections.Generic;

namespace StkMS.Library.Contracts
{
    public interface IStorage<T>
    {
        /// <summary>
        ///     Returns all items in storage.
        /// </summary>
        /// <returns>All items in storage.</returns>
        IEnumerable<T> GetAll();

        /// <summary>
        ///     Retrieves or sets the item associated with the given key.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <returns>The item associated with the given key.</returns>
        /// <remarks>If the item doesn't exist when retrieving, does not throw but returns <c>default</c>.</remarks>
        T? this[string key] { get; set; }
    }
}