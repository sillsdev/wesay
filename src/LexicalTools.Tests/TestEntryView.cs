using NUnit.Framework;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	[Ignore("Completely commented out")]
	public class TestEntryView
	{
		[SetUp]
		public void Setup()
		{
			// Application.Init();
		}

		//        [Test]
		//        public void SmokeTest()
		//        {
		//            EntryViewTool tool = new EntryViewTool(new PretendRecordList());
		//            VBox container = new VBox();
		//            tool.Container = container;
		//
		//            tool.Activate();
		//            tool.Deactivate();
		//            tool.Activate();
		//            tool.Deactivate();
		//
		//            Assert.AreNotEqual(null, tool.Label);
		//        }
		//
		//        [Test]
		//        public void IterateForwardThroughAllRecords()
		//        {
		//            PretendRecordList records = new PretendRecordList();
		//            EntryViewTool tool = new EntryViewTool(records);
		//            VBox container = new VBox();
		//            tool.Container = container;
		//            tool.Activate();
		//
		//            int count = records.Count;
		//            for(int i = 0; i < count; ++i){
		//                tool.OnNextClicked(tool, new System.EventArgs());
		//            }
		//
		//            tool.Deactivate();
		//
		//            Assert.AreNotEqual(null, tool.Label);
		//        }
	}

	//[TestFixture]
	//public class GTKReturningWidget
	//{
	//    [TestFixtureSetUp]
	//    public void TestFixtureSetupUp()
	//    {
	//        Application.Init();
	//    }

	//    VBox WrapWithVBox(Widget widget)
	//    {

	//        VBox vbox = new VBox();
	//        vbox.PackStart(widget);
	//        return vbox;
	//    }

	//    [Test]
	//    public void ReturnedWrappedWidgets()
	//    {
	//        Label label = new Label("hello world");
	//        Widget widget = label;
	//        for (int i = 0; i < 50; ++i)
	//        {
	//            widget = WrapWithVBox(widget);
	//            System.GC.Collect();
	//        }
	//    }

	//    [Test]
	//    public void LoopedWrappedWidgetsBuildingInsideOut()
	//    {
	//        Label label = new Label("hello world");
	//        Widget child = label;
	//        for (int i = 0; i < 50; ++i)
	//        {
	//            VBox vbox = new VBox();
	//            vbox.PackStart(child);
	//            child = vbox;
	//        }
	//    }

	//    [Test]
	//    public void LoopedWrappedWidgetsBuildingOutsideIn()
	//    {
	//        VBox bigParent = new VBox();
	//        Box parent = bigParent;
	//        for (int i = 0; i < 50; ++i)
	//        {
	//            VBox vbox = new VBox();
	//            parent.PackStart(vbox);
	//            parent = vbox;
	//        }
	//        Label label = new Label("hello world");
	//        parent.PackStart(label);

	//        while (bigParent.Children.Length > 0)
	//        {
	//            bigParent.Children[0].Destroy();
	//        }

	//        parent = bigParent;
	//        for (int i = 0; i < 50; ++i)
	//        {
	//            VBox vbox = new VBox();
	//            parent.PackStart(vbox);
	//            parent = vbox;
	//        }
	//        label = new Label("hello world");
	//        parent.PackStart(label);

	//    }

	//}
}