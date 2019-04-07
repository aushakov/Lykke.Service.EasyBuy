using System;
using System.Collections.Generic;
using System.Linq;

namespace Lykke.Service.EasyBuy.DomainServices.Cache
{
    internal sealed class InMemoryCache<T> where T : class
    {
        private readonly Func<T, string> _getKeyFunc;
        private readonly object _sync = new object();

        private readonly Dictionary<string, T> _cache = new Dictionary<string, T>();

        public bool Initialized { get; private set; }

        public InMemoryCache(Func<T, string> getKeyFunc, bool initialized)
        {
            _getKeyFunc = getKeyFunc;
            Initialized = initialized;
        }

        public IReadOnlyList<T> GetAll()
        {
            lock (_sync)
            {
                return Initialized ? _cache.Values.ToArray() : null;
            }
        }

        public T Get(string key)
        {
            lock (_sync)
            {
                if (_cache.ContainsKey(key))
                    return _cache[key];
            }

            return null;
        }

        public void Set(T item)
        {
            if (!Initialized)
                return;

            lock (_sync)
            {
                _cache[_getKeyFunc(item)] = item;
            }
        }

        public void Set(IReadOnlyList<T> items)
        {
            if (!Initialized)
                return;

            lock (_sync)
            {
                foreach (T item in items)
                    _cache[_getKeyFunc(item)] = item;
            }
        }

        public void Initialize(IReadOnlyList<T> items)
        {
            if (Initialized)
                return;

            lock (_sync)
            {
                if (Initialized)
                    return;

                foreach (T item in items)
                    _cache[_getKeyFunc(item)] = item;

                Initialized = true;
            }
        }

        public void Remove(string key)
        {
            lock (_sync)
            {
                if (_cache.ContainsKey(key))
                    _cache.Remove(key);
            }
        }

        public void Remove(string key, Func<T, bool> func)
        {
            lock (_sync)
            {
                if (_cache.ContainsKey(key) && func(_cache[key]))
                    _cache.Remove(key);
            }
        }

        public void Clear()
        {
            lock (_sync)
            {
                _cache.Clear();
            }
        }
    }
}
