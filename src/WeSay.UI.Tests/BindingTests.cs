using System.Drawing;
using NUnit.Framework;
using WeSay.Language;
using WeSay.Project;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class BindingTests
	{
		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();
		}

		[Test]
		public void TargetToWidget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget = new WeSayTextBox(new WritingSystem("vernacular", new Font("Arial", 12)));
			new TextBinding(text, "vernacular", widget);

			text["vernacular"] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["vernacular"] = null;
			Assert.AreEqual("",widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget = new WeSayTextBox(new WritingSystem("vernacular", new Font("Arial", 12)));

			new TextBinding(text, "vernacular", widget);

			widget.Text = "aaa";
			Assert.AreEqual("aaa", text["vernacular"]);
			widget.Text = "";
			Assert.AreEqual("", text["vernacular"]);
		}
	}
}
