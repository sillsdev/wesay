using System;
using System.Collections.Generic;
using System.Text;

namespace WeSay.Foundation
{
	public static class StringExtensions
	{
		public static List<string> SplitTrimmed(this string s, char seperator)
		{
			var x = s.Split(seperator);

			var r = new List<string>();

			foreach (var part in x)
			{
				r.Add(part.Trim());
			}
			return r;
		}


	}

	public static class DictionaryExtensions
	{
		public static B GetOrCreate<A, B>(this System.Collections.Generic.Dictionary<A, B> dictionary, A key)
			where B: new()
		{
			B target;
			if(!dictionary.TryGetValue(key, out target))
			{
				target = new B();
				dictionary.Add(key, target);
			}
			return target;
		}
	}

	public static class IEnumberableExtensions
	{
		public static string Concat<T>(this IEnumerable<T> list, string seperator)
		{
			string s = string.Empty;

			foreach (var part in list)
			{
				s += part.ToString()+seperator;
			}
			if (s.Length > 0)
			{
				return s.Substring(0, s.Length - seperator.Length);
			}
			return string.Empty;
		}
	}
}
