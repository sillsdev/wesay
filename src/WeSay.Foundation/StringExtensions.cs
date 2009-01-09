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
}
