using System.Collections.Generic;
using WeSay.Data;

namespace WeSay.Data
{
	public interface IQuery<T>
	{
		//bool Matches(T item);
		IEnumerable<string> GetDisplayStrings(T item);
		List<RecordToken<T>> RetrieveItems();
	}
}