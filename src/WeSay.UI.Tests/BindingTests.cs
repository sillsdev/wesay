using System.Windows.Forms;
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
			Gtk.Application.Init();
		}

		[Test]
		public void TargetToWidget()
		{
			MultiText text = new MultiText();
			TextBox widget = new TextBox();
			Binding binding = new Binding(text, "en", widget);

			text["en"] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["en"] = null;
			Assert.AreEqual("",widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			TextBox widget = new TextBox();

			Binding binding = new Binding(text, "en", widget);

			widget.Text = "aaa";
			Assert.AreEqual("aaa", text["en"]);
			widget.Text = "";
			Assert.AreEqual("", text["en"]);
		}
	}
}
