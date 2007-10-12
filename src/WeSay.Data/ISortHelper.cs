using System.Collections.Generic;

namespace WeSay.Data
{
	/// <summary>
	/// ISortHelper objects should also implement GetHashCode
	/// </summary>
	/// <typeparam name="Key"></typeparam>
	/// <typeparam name="Value"></typeparam>
	public interface ISortHelper<Key, Value> where Value : class, new()
	{
		IComparer<Key> KeyComparer{get;}

		List<KeyValuePair<Key, long>> GetKeyIdPairs();

		IEnumerable<Key> GetKeys(Value item);

		/// <summary>
		/// this is a name suitable for display to a human (or saving as a file)
		/// </summary>
		string Name
		{
			get;
		}
	}
}
