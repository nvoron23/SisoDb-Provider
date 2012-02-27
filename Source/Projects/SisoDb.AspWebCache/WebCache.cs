using System;
using System.Collections.Generic;
using System.Web.Caching;
using EnsureThat;
using PineCone.Structures;

namespace SisoDb.AspWebCache
{
    public class WebCache : ICache
	{
        protected readonly Cache InternalCache;
        protected readonly WebCacheConfig CacheConfig;

	    public WebCache(Cache cache, WebCacheConfig cacheConfig)
		{
            Ensure.That(cache, "cache").IsNotNull();
            Ensure.That(cacheConfig, "cacheConfig").IsNotNull();

            InternalCache = cache;
	        CacheConfig = cacheConfig;
		}

		public virtual Type StructureType
		{
			get { return CacheConfig.StructureType; }
		}

		public virtual void Clear()
		{
		    var cacheEnumerator = InternalCache.GetEnumerator();
		    while (cacheEnumerator.MoveNext())
		    {
                if(cacheEnumerator.Key.ToString().StartsWith(CacheConfig.CacheEntryKeyPrefix))
		            InternalCache.Remove(cacheEnumerator.Key.ToString());
		    }
		}

	    public virtual bool Exists(IStructureId id)
	    {
            return InternalCache.Get(GenerateCacheKey(id)) != null;
	    }

	    public virtual T GetById<T>(IStructureId id) where T : class
	    {
            return InternalCache.Get(GenerateCacheKey(id)) as T;
		}

		public virtual IDictionary<IStructureId, T> GetByIds<T>(IStructureId[] ids) where T : class
		{
			var result = new Dictionary<IStructureId, T>(ids.Length);

			foreach (var id in ids)
			{
				var structure = GetById<T>(id);
				if (structure != null)
					result.Add(id, structure);
			}

			return result;
		}

		public virtual T Put<T>(IStructureId id, T structure) where T : class
		{
            InternalCache.Insert(
                GenerateCacheKey(id), 
                structure, 
                null, 
                CacheConfig.AbsoluteExpiration,
                CacheConfig.SlidingExpiration);

			return structure;
		}

		public virtual IEnumerable<T> Put<T>(IEnumerable<KeyValuePair<IStructureId, T>> items) where T : class
		{
			foreach (var kv in items)
			{
				Put(kv.Key, kv.Value);
				yield return kv.Value;
			}
		}

		public virtual void Remove(IStructureId id)
		{
            InternalCache.Remove(GenerateCacheKey(id));
		}

		public virtual void Remove(IEnumerable<IStructureId> ids)
		{
			foreach (var structureId in ids)
				Remove(structureId);
		}

        protected virtual string GenerateCacheKey(IStructureId id)
        {
            return CacheConfig.CacheEntryKeyGenerator.Invoke(id);
        }
	}
}