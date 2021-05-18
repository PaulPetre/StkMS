using System.Collections.Generic;
using System.Linq;
using StkMS.Library.Contracts;

namespace StkMS.SqlData.Services
{
    public class Storage<T> : IStorage<T>
    {
        /// <inheritdoc />
        public IEnumerable<T> GetAll()
        {
            lock (GATE)
            {
                return dict.Values.AsEnumerable();
            }
        }

        /// <inheritdoc />
        public T? this[string key]
        {
            get
            {
                lock (GATE)
                {
                    return dict.ContainsKey(key) ? dict[key] : default;
                }
            }
            set
            {
                lock (GATE)
                {
                    if (value is { })
                        dict[key] = value;
                    else
                        dict.Remove(key);
                }
            }
        }

        //

        // ReSharper disable once StaticMemberInGenericType
        private static readonly object GATE = new();

        private readonly Dictionary<string, T> dict = new();
    }
}