using System;
using System.Collections.Generic;

namespace WeSay.Data
{
	//todo: write tests and hashcode
	internal class DictionaryEqualityComparer<Key, Value>: EqualityComparer<Dictionary<Key, Value>>
	{
		public override bool Equals(Dictionary<Key, Value> x, Dictionary<Key, Value> y)
		{
			if (x.Count != y.Count)
			{
				return false;
			}
			foreach (Key key in x.Keys)
			{
				if (!Equals(x[key], y[key]))
				{
					return false;
				}
			}
			return true;
		}

		public override int GetHashCode(Dictionary<Key, Value> obj)
		{
			throw new NotImplementedException();
		}
	}
}