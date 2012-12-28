using System.Drawing;
using NUnit.Framework;
using Palaso.WritingSystems;
using WeSay.Project;
using WeSay.Project;
using Palaso.Lift;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.UI.TextBoxes;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class BindingTests
	{
		[SetUp]
		public void Setup()
		{
			BasilProjectTestHelper.InitializeForTests();
		}

		[Test]
		public void TargetToWidget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget =
					new WeSayTextBox(WritingSystemDefinition.Parse("qaa-x-qaa"), null);
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
					new WeSayTextBox(WritingSystemDefinition.Parse("qaa-x-qaa"), null);

			var binding = new TextBinding(text, "vernacular", widget);

			widget.Text = "aaa";
			widget.Dispose();//this is hard to test now, because the biding only fires when focus is lost or the target ui control goes away
			Assert.AreEqual("aaa", text["vernacular"]);
		}
	}
}