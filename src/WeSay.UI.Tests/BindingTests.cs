using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using SIL.WritingSystems;
using WeSay.Project;
using WeSay.Project;
using SIL.Lift;
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
			var ws = new WritingSystemDefinition("qaa");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;

			IWeSayTextBox widget =
					new WeSayTextBox(ws, null);
			new TextBinding(text, "vernacular", (Control)widget);

			text["vernacular"] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["vernacular"] = null;
			Assert.AreEqual("", widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			var ws = new WritingSystemDefinition("qaa");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			WeSayTextBox widget =
					new WeSayTextBox(ws, null);

			var binding = new TextBinding(text, "vernacular", widget);

			widget.Text = "aaa";
			widget.Dispose();//this is hard to test now, because the biding only fires when focus is lost or the target ui control goes away
			Assert.AreEqual("aaa", text["vernacular"]);
		}
	}
}