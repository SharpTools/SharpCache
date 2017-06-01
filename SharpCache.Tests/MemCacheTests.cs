using System;
using System.Threading.Tasks;
using Xunit;

namespace SharpCache.Tests {
    public class MemCacheTests {

        private MemCache<int, string> _cache;
       
        public MemCacheTests() {
            _cache = new MemCache<int, string>(3, TimeSpan.FromMilliseconds(1000));
        }

        [Fact]
        public void Should_add_simple_item() {
            _cache.Add(1, "1");
            Assert.Equal("1", _cache.Get(1));
        }

        [Fact]
        public void Should_remove_oldest_after_limit() {
            _cache.Add(1, "a");
            _cache.Add(2, "b");
            _cache.Add(3, "c");
            _cache.Add(4, "d");
            Assert.Equal("b", _cache.Get(2));
            Assert.Equal("c", _cache.Get(3));
            Assert.Equal("d", _cache.Get(4));
            Assert.Equal(null, _cache.Get(1));
        }

        [Fact]
        public async Task Should_not_return_expired() {
            _cache = new MemCache<int, string>(3, TimeSpan.FromMilliseconds(49));
            _cache.Add(1, "a");
            await Task.Delay(50);
            Assert.Equal(null, _cache.Get(1));
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task Should_renew_age_when_returned__when_configured(bool renewAge) {
            _cache = new MemCache<int, string>(3, TimeSpan.FromMilliseconds(49));
            _cache.AutoRenew = renewAge;
            _cache.Add(1, "a");
            await Task.Delay(25);
            _cache.Get(1);
            await Task.Delay(25);
            Assert.Equal(renewAge ? "a" : null, _cache.Get(1));
        }
    }
}
