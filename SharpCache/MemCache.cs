using System;
using System.Collections.Generic;

namespace SharpCache {
    public class MemCache<TK, TV> where TV : class {
        private readonly int _maxItems;
        private readonly TimeSpan _expirationSpan;
        private object _syncRoot = new object();
        private Dictionary<TK, CachedItem> _dic = new Dictionary<TK, CachedItem>();

        public bool AutoRenew { get; set; } = true;

        public MemCache(int maxItems, TimeSpan expirationSpan) {
            _maxItems = maxItems;
            _expirationSpan = expirationSpan;
        }

        public void Add(TK key, TV value) {
            lock (_syncRoot) {
                if (!_dic.ContainsKey(key) && _dic.Count >= _maxItems) {
                    RemoveOldest();
                }
                _dic[key] = new CachedItem(value, _expirationSpan);
            }
        }
        private void RemoveOldest() {
            var oldest = DateTime.MaxValue;
            var key = default(TK);
            foreach (var keyValue in _dic) {
                if (keyValue.Value.LastUsed < oldest) {
                    oldest = keyValue.Value.LastUsed;
                    key = keyValue.Key;
                }
            }
            if (key != null) {
                _dic.Remove(key);
            }
        }

        public TV Get(TK key) {
            lock (_syncRoot) {
                if (!_dic.ContainsKey(key)) {
                    return null;
                }
                var cachedItem = _dic[key];
                if (cachedItem.IsValid()) {
                    if (AutoRenew) {
                        cachedItem.Touch();
                    }
                    return cachedItem.Item;
                }
                _dic.Remove(key);
                return null;
            }
        }

        public void Invalidate(TK key) {
            lock(_syncRoot) {
                _dic.Remove(key);
            }
        }
        
        private class CachedItem {
            private readonly TimeSpan _expiration;
            public DateTime LastUsed { get; private set; }
            public TV Item { get; }

            public CachedItem(TV item, TimeSpan expiration) {
                _expiration = expiration;
                Item = item;
                Touch();
            }

            public bool IsValid() {
                return (DateTime.Now - LastUsed) < _expiration;
            }

            public void Touch() {
                LastUsed = DateTime.Now;
            }
        }
    }
}
