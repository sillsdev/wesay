using System.Collections.Generic;
using WeSay.Data;

namespace WeSay.LexicalModel
{
	public interface IQuery<T>
	{
		//bool Matches(T item);
		IEnumerable<string> GetDisplayStrings(T item);
		List<RecordToken> RetrieveItems();
	}
}