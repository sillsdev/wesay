using System.Collections.Generic;

namespace WeSay.Data
{
	public interface ISortHelper<Key, Value> where Value : class, new()
	{
		IComparer<Key> KeyComparer{get;}

		List<KeyValuePair<Key, long>> GetKeyIdPairs();

		IEnumerable<Key> GetKeys(Value item);

		string Name
		{
			get;
		}

	}
}
