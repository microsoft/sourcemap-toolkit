using System;
using System.Collections.Generic;

namespace SourcemapToolkit.CallstackDeminifier
{
	internal class KeyValueCache<TKey, TValue> where TValue : class
	{
		private readonly Dictionary<TKey, TValue> _cache;
		public KeyValueCache()
		{
			_cache = new Dictionary<TKey, TValue>();
		}

		/// <summary>
		/// Attempts to obtain the value associated with this key from the cache.
		/// If it is not found in the cache, it gets it from the valueGetter function provided
		/// and stores it in the cache for future calls.
		/// </summary>
		public TValue GetValue(TKey key, Func<TValue> valueGetter)
		{
			TValue value = null;
			_cache.TryGetValue(key, out value);

			if (value == null)
			{
				value = valueGetter();
				_cache[key] = value;
			}

			return value;
		}
	}
}
