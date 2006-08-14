using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.UI.Tests
{
	 [TestFixture]
	public class TableBuilderTests
	{
		[SetUp]
		public void Setup()
		{
			Gtk.Application.Init();
		}

		[Test]
		public void BuildOne()
		{
			TableBuilder builder = new TableBuilder();
			builder.AddLabelRow("one");

			Gtk.Table table = builder.BuildTable();
			Assert.AreEqual(1, table.NRows);
			Assert.AreEqual(2, table.NColumns);

			Gtk.Widget widget = new Gtk.Entry("entry in two");
			builder.AddWidgetRow("two", widget);
			table = builder.BuildTable();


			Assert.AreEqual(2, table.NRows);

			//at the moment, I am not seeing a reasonable way to look
			//into the table to see that it built what we want it to build.
			//there is a list of children, but it is not clear that the ordering
			//is something we are supposed to depend on...
		}
	}
}
