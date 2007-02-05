using System.Collections.Generic;

namespace WeSay.Data
{
	public interface IDb4oSortHelper<K, T> where T:class, new()
	{
		IComparer<K> KeyComparer{get;}

		List<KeyValuePair<K, long>> GetKeyIdPairs();

		IEnumerable<K> GetKeys(T item);

		string Name
		{
			get;
		}

	}
}
