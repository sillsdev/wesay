using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using WeSay.LexicalModel;
using WeSay.LexicalTools;
using Gtk;

namespace WeSay.LexicalTools.Tests
{
	[TestFixture]
	public class TestEntryView
	{
		[SetUp]
		public void Setup()
		{
			Gtk.Application.Init();
		}

		[Test]
		public void SmokeTest()
		{
			EntryViewTool tool = new EntryViewTool(new WeSay.LexicalModel.Tests.PretendRecordList());
			VBox container = new VBox();
			tool.Container = container;

			tool.Activate();
			tool.Deactivate();
			tool.Activate();
			tool.Deactivate();

			Assert.AreNotEqual(null, tool.Label);
		}

		[Test]
		public void LoadThai()
		{

			WeSay.Data.Db4oDataSource ds = new WeSay.Data.Db4oDataSource( @"..\..\SampleProjects\Thai\thai.yap");
		   // WeSay.Data.Db4oBindingListConfiguration<LexEntry> config = new WeSay.Data.Db4oBindingListConfiguration<LexEntry>(ds);
			WeSay.Data.Db4oBindingList<LexEntry> entries = new WeSay.Data.Db4oBindingList<LexEntry>(ds);

			Assert.Greater(entries.Count,0);
		}
	}
}
