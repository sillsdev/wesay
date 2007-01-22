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
			WeSayTextBox widget = new WeSayTextBox(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault);
			TextBinding binding = new TextBinding(text, BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id, widget);

			text[BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text["en"] = null;
			Assert.AreEqual("",widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget = new WeSayTextBox(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault);

			TextBinding binding = new TextBinding(text, BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault.Id, widget);

			widget.Text = "aaa";
			Assert.AreEqual("aaa", text["en"]);
			widget.Text = "";
			Assert.AreEqual("", text["en"]);
		}
	}
}
