using System.Collections.Generic;
using System.Linq;
using StkMS.Library.Contracts;

namespace StkMS.SqlData.Services
{
    public class Storage<T> : IStorage<T>
    {
        /// <inheritdoc />
        public IEnumerable<T> GetAll() => dict.Values.AsEnumerable();

        /// <inheritdoc />
        public T? this[string key]
        {
            get => dict.ContainsKey(key) ? dict[key] : default;
            set
            {
                if (value is { })
                    dict[key] = value;
                else
                    dict.Remove(key);
            }
        }

        //

        private readonly Dictionary<string, T> dict = new();
    }
}