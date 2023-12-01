namespace CraigMiller.Map.Core.Layers.Tiling
{
    public class AccessOrderedCache<TKey, TVal> where TKey : notnull
    {
        readonly IDictionary<TKey, CachedItem> _dict;
        readonly Action<TKey, TVal> _itemEvicted;

        public AccessOrderedCache(Action<TKey, TVal> itemEvicted)
        {
            _dict = new Dictionary<TKey, CachedItem>();
            _itemEvicted = itemEvicted;
        }

        public int MaxSize { get; set; } = 256;

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

                if (_dict.Count > MaxSize)
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
