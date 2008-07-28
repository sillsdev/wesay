using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	internal class DictionaryContentAsserter<K, V>: IAsserter
	{
		private readonly Dictionary<K, V>[] _expectedResult;
		private readonly Dictionary<K, V>[] _actualResult;

		public DictionaryContentAsserter(Dictionary<K, V>[] expectedResult,
										 IEnumerable<Dictionary<K, V>> actualResult)
		{
			_expectedResult = expectedResult;
			_actualResult = ToArray(actualResult);
		}

		public DictionaryContentAsserter(IEnumerable<Dictionary<K, V>> expectedResult,
										 IEnumerable<Dictionary<K, V>> actualResult)
		{
			_expectedResult = ToArray(expectedResult);
			_actualResult = ToArray(actualResult);
		}

		private static T[] ToArray<T>(IEnumerable<T> result)
		{
			return new List<T>(result).ToArray();
		}

		public DictionaryContentAsserter(Dictionary<K, V>[] expectedResult,
										 Dictionary<K, V>[] actualResult)
		{
			_expectedResult = expectedResult;
			_actualResult = actualResult;
		}

		public bool Test()
		{
			if (_expectedResult.Length != _actualResult.Length)
			{
				return false;
			}
			DictionaryEqualityComparer<K, V> comparer = new DictionaryEqualityComparer<K, V>();
			for (int i = 0;i != _expectedResult.Length;++i)
			{
				if (!comparer.Equals(_expectedResult[i], _actualResult[i]))
				{
					return false;
				}
			}
			return true;
		}

		public string Message
		{
			get
			{
				return "Jagged arrays differ.\n" + "Expected:\n" + Write(_expectedResult) + '\n' +
					   "Actual:\n" + Write(_actualResult);
			}
		}

		private static string Write(IEnumerable<Dictionary<K, V>> dicts)
		{
			StringBuilder sb = new StringBuilder();

			foreach (Dictionary<K, V> dict in dicts)
			{
				foreach (KeyValuePair<K, V> pair in dict)
				{
					sb.Append(pair.Key);
					sb.Append(':');
					sb.Append(pair.Value);
					sb.Append(' ');
				}
				sb.Append('\n');
			}
			return sb.ToString();
		}
	}
}