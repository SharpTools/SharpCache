# SharpCache
The simplest in-memory threadsafe cache you will find

## Usage:

```cs
  //create cache with maximum 300 items and with expiration of 30 seconds
  var cache = new MemCache<int, string>(3, TimeSpan.FromSeconds(30));
  
  //add to cache
  _cache.Add(1, "foo");
  
  //get from cache
  var cachedItem = _cache.Get(1);
```

## Item Expiration

Item expiration date is renewed when it is retrieved from cache.
Ex:
```cs
  var cache = new MemCache<int, string>(300, TimeSpan.FromSeconds(30));
  _cache.Add(1, "foo"); //30s to expire
  // 5s later
  _cache.Get(1); //expire date set to 30s again
```

You can change that setting AutoRenew to false.
```cs
  _cache.AutoRenew = renewAge;
  _cache.Add(1, "foo"); //30s to expire
  // 5s later
  _cache.Get(1); //25s to expire
```

## Maximum number of items

When the cache is full, the oldest item is removed.
Ex:
```cs
  var cache = new MemCache<int, string>(3, TimeSpan.FromSeconds(30));
  _cache.Add(1, "foo1");
  _cache.Add(2, "foo2");
  _cache.Add(3, "foo3");
  _cache.Get(1); //expire date set to 30s again
  _cache.Add(4, "foo4");
  //foo2 is no longer available. The item foo1 was not removed because its expiration date was renewed
```
Enjoy!
