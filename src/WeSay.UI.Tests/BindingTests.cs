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
			WeSayTextBox widget = new WeSayTextBox(BasilProject.Project.WritingSystems.TestGetWritingSystemAnal);
			TextBinding binding = new TextBinding(text, BasilProject.Project.WritingSystems.TestGetWritingSystemAnal.Id, widget);

			text[BasilProject.Project.WritingSystems.TestGetWritingSystemAnal.Id] = "hello";
			Assert.AreEqual("hello", widget.Text);
			text[BasilProject.Project.WritingSystems.TestGetWritingSystemAnal.Id] = null;
			Assert.AreEqual("",widget.Text);
		}

		[Test]
		public void WidgetToTarget()
		{
			MultiText text = new MultiText();
			WeSayTextBox widget = new WeSayTextBox(BasilProject.Project.WritingSystems.TestGetWritingSystemAnal);

			TextBinding binding = new TextBinding(text, BasilProject.Project.WritingSystems.TestGetWritingSystemAnal.Id, widget);

			widget.Text = "aaa";
			Assert.AreEqual("aaa", text[BasilProject.Project.WritingSystems.TestGetWritingSystemAnal.Id]);
			widget.Text = "";
			Assert.AreEqual("", text[BasilProject.Project.WritingSystems.TestGetWritingSystemAnal.Id]);
		}
	}
}
