using NUnit.Framework;
using System.Diagnostics;
using System.Collections.Generic;
using WeSay.Language;

namespace Language.Tests
{
	[TestFixture]
	public class Test1
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void NullConditions()
		{
			MultiText text = new MultiText();
			Assert.AreSame("", text["foo"], "never before heard of alternative should give back an empty string");
			Assert.AreSame("", text["foo"], "second time");
			Assert.AreSame("", text.GetAlternative("fox"));
			text.SetAlternative("zox", "");
			Assert.AreSame("", text["zox"]);
			text.SetAlternative("zox", null);
			Assert.AreSame("", text["zox"], "should still be empty string after setting to null");
			text.SetAlternative("zox", "something");
			text.SetAlternative("zox", null);
			Assert.AreSame("", text["zox"], "should still be empty string after setting something and then back to null");
		}

		[Test]
		public void BasicStuff()
		{
			MultiText text = new MultiText();
			text["foo"] = "alpha";
			Assert.AreSame("alpha", text["foo"]);
			text["foo"] = "beta";
			Assert.AreSame("beta", text["foo"]);
			text["foo"] = "gamma";
			Assert.AreSame("gamma", text["foo"]);
			text["bee"] = "beeeee";
			Assert.AreSame("gamma", text["foo"], "setting a different alternative should not affect this one");
			text["foo"] = null;
			Assert.AreSame("", text["foo"]);
		}
	}
}
