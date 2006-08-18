using Gtk;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalModel.Tests;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class TestEntryView
	{
		[SetUp]
		public void Setup()
		{
			Application.Init();
		}

		[Test]
		public void SmokeTest()
		{
			EntryViewTool tool = new EntryViewTool(new PretendRecordList());
			VBox container = new VBox();
			tool.Container = container;

			tool.Activate();
			tool.Deactivate();
			tool.Activate();
			tool.Deactivate();

			Assert.AreNotEqual(null, tool.Label);
		}


	}
}
