using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	//todo: write tests and hashcode
	internal class DictionaryEqualityComparer<Key, Value> : EqualityComparer<IDictionary<Key, Value>>
	{
		public override bool Equals(IDictionary<Key, Value> x, IDictionary<Key, Value> y)
		{
			if (x == null && y == null)
			{
				return true;
			}

			if (x == null || y == null)
			{
				return false;
			}

			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (Key key in x.Keys)
			{
				Value v;
				if(!y.TryGetValue(key, out v)
				   || !Equals(x[key], v))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode(IDictionary<Key, Value> obj)
		{
			throw new NotImplementedException();
		}
	}
}
