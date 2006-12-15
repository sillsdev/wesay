using System;
using NUnit.Framework;
using System.ComponentModel;
using WeSay.Language;
using System.Collections;

namespace Language.Tests
{
	[TestFixture]
	public class MultiTextTests
	{
		private bool _gotHandlerNotice;

	   [SetUp]
		public void Setup()
		{
		}

		[Test]
		public void Notification()
		{
			_gotHandlerNotice = false;
			MultiText text = new MultiText();
			text.PropertyChanged += new PropertyChangedEventHandler(propertyChangedHandler);
			text.SetAlternative("zox", "");
			Assert.IsTrue(_gotHandlerNotice);
		}

		void propertyChangedHandler(object sender, PropertyChangedEventArgs e)
		{
			_gotHandlerNotice = true;
		}

		[Test]
		public void NullConditions()
		{
			MultiText text = new MultiText();
			Assert.AreSame(string.Empty, text["foo"], "never before heard of alternative should give back an empty string");
			Assert.AreSame(string.Empty, text["foo"], "second time");
			Assert.AreSame(string.Empty, text.GetAlternative("fox"));
			text.SetAlternative("zox", "");
			Assert.AreSame(string.Empty, text["zox"]);
			text.SetAlternative("zox", null);
			Assert.AreSame(string.Empty, text["zox"], "should still be empty string after setting to null");
			text.SetAlternative("zox", "something");
			text.SetAlternative("zox", null);
			Assert.AreSame(string.Empty, text["zox"], "should still be empty string after setting something and then back to null");
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
			Assert.AreSame(string.Empty, text["foo"]);
		}

		[Test]
		public void ImplementsIEnumerable()
		{
			MultiText text = new MultiText();
			IEnumerable ienumerable = text;
			Assert.IsNotNull(ienumerable);
		}

		[Test]
		public void Count()
		{
			MultiText text = new MultiText();
			Assert.AreEqual(0, text.Count);
			text["a"] = "alpha";
			text["b"] = "beta";
			text["g"] = "gamma";
			Assert.AreEqual(3, text.Count);
		}

		[Test]
		public void IterateWithForEach()
		{
			MultiText text = new MultiText();
			text["a"] = "alpha";
			text["b"] = "beta";
			text["g"] = "gamma";
			int i = 0;
			foreach (LanguageForm l in text)
			{
				switch(i){
					case 0:
						Assert.AreEqual("a", l.WritingSystemId);
						Assert.AreEqual("alpha", l.Form);
						break;
					case 1:
						Assert.AreEqual("b", l.WritingSystemId);
						Assert.AreEqual("beta", l.Form);
						break;
					case 2:
						Assert.AreEqual("g", l.WritingSystemId);
						Assert.AreEqual("gamma", l.Form);
						break;
				}
				i++;
			}
		}
		[Test]
		public void GetEnumerator()
		{
			MultiText text = new MultiText();
			IEnumerator ienumerator = text.GetEnumerator();
			Assert.IsNotNull(ienumerator);
		}


		[Test]
		public void MergedGuyHasCorrectParentsOnForms()
		{
			MultiText x = new MultiText();
			x["a"] = "alpha";
			MultiText y = new MultiText();
			y["b"] = "beta";
			x.MergeIn(y);
			Assert.AreSame(y, y.Find("b").Parent);
			Assert.AreSame(x, x.Find("b").Parent);
		}


		[Test]
		public void MergeWithEmpty()
		{
			MultiText old = new MultiText();
			MultiText newGuy = new MultiText();
			old.MergeIn(newGuy);
			Assert.AreEqual(0, old.Count);

			old = new MultiText();
			old["a"] = "alpha";
			old.MergeIn(newGuy);
			Assert.AreEqual(1, old.Count);
		}

		[Test]
		public void MergeWithOverlap()
		{
			MultiText old = new MultiText();
			old["a"] = "alpha";
			old["b"] = "beta";
			MultiText newGuy = new MultiText();
			newGuy["b"] = "newbeta";
			newGuy["c"] = "charlie";
			old.MergeIn(newGuy);
			Assert.AreEqual(3, old.Count);
			Assert.AreEqual("newbeta", old["b"]);
		}
	}
}
