using WeSay.Language;
using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class BindingTests
	{
		[SetUp]
		public void Setup()
		{
		}

		[Test]
		public void TargetToWidget()
		{
			MultiText text = new MultiText();
			Gtk.Entry widget = new Gtk.Entry();

			Binding binding = new Binding(text, "en", widget);

			text["en"] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["en"] = null;
			Assert.AreEqual("",widget.Text);
		}
	}
}
