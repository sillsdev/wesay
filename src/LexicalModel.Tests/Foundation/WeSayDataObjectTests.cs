using System;
using NUnit.Framework;
using WeSay.Foundation;

namespace WeSay.LexicalModel.Tests.Foundation
{
	internal class TestClass: WeSayDataObject
	{
		public TestClass(WeSayDataObject parent): base(parent) {}

		public override bool IsEmpty
		{
			get { throw new NotImplementedException(); }
		}
	}

	[TestFixture]
	public class WeSayDataObjectTests
	{
		[Test]
		public void NoProperties_NoFlag()
		{
			TestClass t = new TestClass(null);
			Assert.IsFalse(t.GetHasFlag("foo"));
		}

		[Test]
		public void LackingProperty_NoFlag()
		{
			TestClass t = new TestClass(null);
			t.SetFlag("notfoo");
			Assert.IsFalse(t.GetHasFlag("foo"));
		}

		[Test]
		public void AfterSettingReportsTrue()
		{
			TestClass t = new TestClass(null);
			t.SetFlag("foo");
			Assert.IsTrue(t.GetHasFlag("foo"));
		}

		[Test]
		public void SetPropertiesToTrue()
		{
			TestClass t = new TestClass(null);
			t.SetFlag("foo");
			Assert.IsTrue(t.GetHasFlag("foo"));
		}
	}
}