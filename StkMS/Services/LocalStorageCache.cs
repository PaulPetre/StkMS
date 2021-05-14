using Blazored.LocalStorage;
using StkMS.Library.Contracts;

namespace StkMS.Services
{
    public class LocalStorageCache : ICache
    {
        public string? this[string? key]
        {
            get => localStorage.GetItemAsString(key);
            set => localStorage.SetItem(key, value);
        }

        public LocalStorageCache(ISyncLocalStorageService localStorage)
        {
            this.localStorage = localStorage;
        }

        //

        private readonly ISyncLocalStorageService localStorage;
    }
}