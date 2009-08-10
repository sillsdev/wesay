using System.Collections.Generic;
using Palaso.Data;

namespace WeSay.Data
{
	class QueryAdapter<T>: Query, IQuery<T> where T: class, new()
	{
		public QueryAdapter()
			: base(typeof (T))
		{
		}

		public IEnumerable<IDictionary<string, object>> GetResults(T item)
		{
			return base.GetResults((object) item);
		}
	}
}
