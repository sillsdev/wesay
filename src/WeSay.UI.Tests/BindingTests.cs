using System.Drawing;
using NUnit.Framework;
using WeSay.Foundation;
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
			WeSayTextBox widget =
					new WeSayTextBox(new WritingSystem("vernacular", new Font("Arial", 12)), null);
			new TextBinding(text, "vernacular", widget);

			text["vernacular"] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["vernacular"] = null;
			Assert.AreEqual("", widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget =
					new WeSayTextBox(new WritingSystem("vernacular", new Font("Arial", 12)), null);

			var binding = new TextBinding(text, "vernacular", widget);

			widget.Text = "aaa";
			widget.Dispose();//this is hard to test now, because the biding only fires when focus is lost or the target ui control goes away
			Assert.AreEqual("aaa", text["vernacular"]);
		}
	}
}