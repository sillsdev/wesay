using System.Collections.Generic;

namespace WeSay.Data
{
	/// <summary>
	/// ISortHelper objects should also implement GetHashCode
	/// </summary>
	/// <typeparam name="Value"></typeparam>
	public interface ISortHelper<Value> where Value : class, new()
	{
		IComparer<string> KeyComparer{get;}

		List<RecordToken> GetKeyIdPairs();

		IEnumerable<string> GetKeys(Value item);

		/// <summary>
		/// this is a name suitable for display to a human (or saving as a file)
		/// </summary>
		string Name
		{
			get;
		}
	}
}
