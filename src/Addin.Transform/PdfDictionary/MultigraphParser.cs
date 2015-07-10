using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SIL.Code;

namespace Addin.Transform.PdfDictionary
{
	/// <summary>
	/// THis class simply identifies the first non-punctuation multigraph (1 to n letters) of a word.
	/// In the simplest case, this is just the first character. But it can also be a
	/// digraph or trigraph, and can be after skipping a punctionation.
	/// </summary>
	public class MultigraphParser
	{
		private readonly List<string>_multigraphsLongestToShortest;

		/// <summary>
		/// Contrstuctor
		/// </summary>
		/// <param name="multigraphs">can be either all the characters, along with digraphs and such, or just the digraphs</param>
		public MultigraphParser(IEnumerable<string> multigraphs)
		{
			_multigraphsLongestToShortest = new List<string>();
			foreach(var m in multigraphs.OrderByDescending(s=>s.Length))
			{
				var x = m.Trim();
				if (!string.IsNullOrEmpty(x))
				{
					_multigraphsLongestToShortest.Add(x.ToUpperInvariant());
				}
			}
		}

		/// <summary>
		/// Consults the list of multigraphs and returns the uppercase of the first 1..n chracters, the largest that is listed in the multigraphs.
		/// </summary>
		/// <returns></returns>
		public string GetFirstMultigraph(string word)
		{
			Guard.Against(string.IsNullOrEmpty(word),"Word cannot be empty");

			var trimmedUpper = word.Substring(GetOffsetOfFirstLetterOrDigit(word)).ToUpperInvariant();
			foreach (var multigraph in _multigraphsLongestToShortest)
			{
				if(trimmedUpper.StartsWith(multigraph))
					return multigraph;
			}
			//wasn't listed, that's ok, just return the upperchase of the first character (skipping hyphens and such)
			return trimmedUpper.ToCharArray()[0].ToString();
		}


		/// <summary>
		/// Skip over things like hyphens
		/// </summary>
		/// <param name="word"></param>
		/// <returns></returns>
		private static int GetOffsetOfFirstLetterOrDigit(string word)
		{
			int offset = 0;
			foreach (var c in word.ToCharArray())
			{
				if (char.IsLetterOrDigit(c))
				{
					return offset;
				 }
				//safe to assume it's a letter... someday the writing system could tell us
				if (System.Globalization.UnicodeCategory.PrivateUse == char.GetUnicodeCategory(c)) //see WS-1412
				{
					return offset;
				}
				++offset;
			}
			return 0; //wrong, but less harmful...
		}
	}
}
