using System.Collections.Generic;

namespace WeSay.Data
{
	public interface IQuery<T> where T:class ,new()
	{
		//bool Matches(T item);
		IEnumerable<string[]> GetDisplayStrings(T item);
		ResultSet<T> RetrieveItems();
	}
}