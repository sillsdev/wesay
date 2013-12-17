using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.Data.Tests
{
	[TestFixture]
	public class DictionaryEqualityComparerTests
	{
		private DictionaryEqualityComparer<string, string> _comparer;
		private Dictionary<string, string> _x;
		private Dictionary<string, string> _y;

		[SetUp]
		public void SetUp()
		{
			_comparer = new DictionaryEqualityComparer<string, string>();
			_x = new Dictionary<string, string>();
			_y = new Dictionary<string, string>();
		}

		[Test]
		public void Equals_xNullyNotNull_false()
		{
			_x = null;
			_y.Add("0", "Zero");
			Assert.IsFalse(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xNotNullyNull_false()
		{
			_x.Add("0", "Zero");
			_y = null;
			Assert.IsFalse(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xMoreEntriesThany_false()
		{
			_x.Add("0", "Zero");
			Assert.IsFalse(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xFewerEntriesThany_false()
		{
			_y.Add("0", "Zero");
			Assert.IsFalse(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xKeyIsDifferentThany_false()
		{
			_x.Add("0", "Zero");
			_y.Add("1", "Zero");
			Assert.IsFalse(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xValueIsDifferentThany_false()
		{
			_x.Add("0", "Zero");
			_y.Add("0", "One");
			Assert.IsFalse(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xHasSameLengthKeysAndValuesAsy_true()
		{
			_x.Add("0", "Zero");
			_y.Add("0", "Zero");
			Assert.IsTrue(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xAndyAreBothEmpty_true()
		{
			Assert.IsTrue(_comparer.Equals(_x, _y));
		}

		[Test]
		public void Equals_xIsNullyIsNull_true()
		{
			_x = null;
			_y = null;
			Assert.IsTrue(_comparer.Equals(_x, _y));

		}

		[Test]
		[ExpectedException(typeof(ArgumentNullException))]
		public void GetHashCode_Null_Throws()
		{
			DictionaryEqualityComparer<string, string> comparer = new DictionaryEqualityComparer<string, string>();
			comparer.GetHashCode(null);
		}

		[Test]
		public void GetHashCode_TwoDictionariesAreEqual_ReturnSameHashCodes()
		{
			Dictionary<string, string> reference = new Dictionary<string, string>();
			reference.Add("key1", "value1");
			reference.Add("key2", "value2");
			Dictionary<string, string> other = new Dictionary<string, string>();
			other.Add("key1", "value1");
			other.Add("key2", "value2");
			DictionaryEqualityComparer<string, string> comparer = new DictionaryEqualityComparer<string, string>();
			Assert.AreEqual(comparer.GetHashCode(reference), comparer.GetHashCode(other));
		}

		[Test]
		public void GetHashCode_TwoDictionariesHaveDifferentLength_ReturnDifferentHashCodes()
		{
			Dictionary<string, string> reference = new Dictionary<string, string>();
			reference.Add("key1", "value1");
			reference.Add("key2", "value2");
			Dictionary<string, string> other = new Dictionary<string, string>();
			other.Add("key1", "value1");
			DictionaryEqualityComparer<string, string> comparer = new DictionaryEqualityComparer<string, string>();
			Assert.AreNotEqual(comparer.GetHashCode(reference), comparer.GetHashCode(other));
		}

		[Test]
		public void GetHashCode_TwoDictionariesHaveDifferentKey_ReturnDifferentHashCodes()
		{
			Dictionary<string, string> reference = new Dictionary<string, string>();
			reference.Add("key1", "value1");
			reference.Add("key2", "value2");
			Dictionary<string, string> other = new Dictionary<string, string>();
			other.Add("key1", "value1");
			other.Add("key3", "value2");
			DictionaryEqualityComparer<string, string> comparer = new DictionaryEqualityComparer<string, string>();
			Assert.AreNotEqual(comparer.GetHashCode(reference), comparer.GetHashCode(other));
		}

		[Test]
		public void GetHashCode_TwoDictionariesHaveDifferentValue_ReturnDifferentHashCodes()
		{
			Dictionary<string, string> reference = new Dictionary<string, string>();
			reference.Add("key1", "value1");
			reference.Add("key2", "value2");
			Dictionary<string, string> other = new Dictionary<string, string>();
			other.Add("key1", "value1");
			other.Add("key2", "value3");
			DictionaryEqualityComparer<string, string> comparer = new DictionaryEqualityComparer<string, string>();
			Assert.AreNotEqual(comparer.GetHashCode(reference), comparer.GetHashCode(other));
		}
	}
}
