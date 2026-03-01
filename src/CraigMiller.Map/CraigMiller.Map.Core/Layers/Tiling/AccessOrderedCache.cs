namespace CraigMiller.Map.Core.Layers.Tiling
{
    public class AccessOrderedCache<TKey, TVal> where TKey : notnull
    {
        readonly IDictionary<TKey, CachedItem> _dict;
        readonly Action<TKey, TVal> _itemEvicted;

        public AccessOrderedCache(Action<TKey, TVal> itemEvicted, int capacity)
        {
            _dict = new Dictionary<TKey, CachedItem>();
            _itemEvicted = itemEvicted;
            Capacity = capacity;
        }

        /// <summary>
        /// Gets or sets the maximum number of items before the oldest is disposed of
        /// </summary>
        public int Capacity { get; set; }

        public bool TryGetValue(TKey key, out TVal? val)
        {
            lock (_dict)
            {
                if (_dict.TryGetValue(key, out CachedItem? cachedItem))
                {
                    val = cachedItem.Value;

                    return true;
                }
            }

            val = default;
            return false;
        }

        public void Add(TKey key, TVal val)
        {
            lock (_dict)
            {
                _dict.Add(key, new CachedItem(val));

                if (_dict.Count > Capacity)
                {
                    KeyValuePair<TKey, CachedItem> toRemove = _dict.MinBy(kvp => kvp.Value.LastAccessed);
                    _dict.Remove(toRemove.Key);

                    _itemEvicted(toRemove.Key, toRemove.Value.Value);
                }
            }
        }

        class CachedItem
        {
            readonly TVal _value;

            public CachedItem(TVal val)
            {
                _value = val;
                LastAccessed = DateTime.UtcNow;
            }

            public TVal Value
            {
                get
                {
                    LastAccessed = DateTime.UtcNow;
                    return _value;
                }
            }

            public DateTime LastAccessed { get; private set; }
        }
    }
}
