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
	}
}
