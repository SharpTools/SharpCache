using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;

namespace SharpCache.Tests {
    public class MemCacheTests {

        private MemCache<int, string> _cache;
        
        [SetUp]
        public void SetUp() {
            _cache = new MemCache<int, string>(3, TimeSpan.FromMilliseconds(1000));
        }

        [Test]
        public void Should_add_simple_item() {
            _cache.Add(1, "1");
            Assert.AreEqual("1", _cache.Get(1));
        }

        [Test]
        public void Should_remove_oldest_after_limit() {
            _cache.Add(1, "a");
            _cache.Add(2, "b");
            _cache.Add(3, "c");
            _cache.Add(4, "d");
            Assert.AreEqual("b", _cache.Get(2));
            Assert.AreEqual("c", _cache.Get(3));
            Assert.AreEqual("d", _cache.Get(4));
            Assert.AreEqual(null, _cache.Get(1));
        }

        [Test]
        public async Task Should_not_return_expired() {
            _cache = new MemCache<int, string>(3, TimeSpan.FromMilliseconds(100));
            _cache.Add(1, "a");
            await Task.Delay(100);
            Assert.AreEqual(null, _cache.Get(1));
        }
    }
}
