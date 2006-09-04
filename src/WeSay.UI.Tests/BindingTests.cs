using NUnit.Framework;
using WeSay.Language;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class BindingTests
	{
		[SetUp]
		public void Setup()
		{
		   // Gtk.Application.Init();
		}

		[Test]
		public void TargetToWidget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget = new WeSayTextBox(BasilProject.Project.AnalysisWritingSystemDefault);
			Binding binding = new Binding(text, BasilProject.Project.AnalysisWritingSystemDefault, widget);

			text[BasilProject.Project.AnalysisWritingSystemDefault.Id] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["en"] = null;
			Assert.AreEqual("",widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget = new WeSayTextBox(BasilProject.Project.AnalysisWritingSystemDefault);

			Binding binding = new Binding(text, BasilProject.Project.AnalysisWritingSystemDefault, widget);

			widget.Text = "aaa";
			Assert.AreEqual("aaa", text["en"]);
			widget.Text = "";
			Assert.AreEqual("", text["en"]);
		}
	}
}
