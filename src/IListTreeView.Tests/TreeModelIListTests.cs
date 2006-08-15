using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.IListTreeView.Tests
{
	[TestFixture]
	public class TreeModelIListTests : IListTreeModelAdaptor
	{
		public TreeModelIListTests()
			: base(new IListTreeModelConfiguration())
		{
		}

		[TestFixtureSetUp]
		public void TestFixtureSetUp()
		{
			this.Configuration.ColumnTypes.Add(GLib.GType.String);
			this.Configuration.GetValueStrategy = delegate(object o, int column)
			{
				Assert.AreEqual(0, column);
				string s = (string)o;
				return GetValueHelper.Convert<string>(s);
			};
			List<string> stringList = new List<string>();
			stringList.Add("first item");
			stringList.Add("second item");
			stringList.Add("third item");
			stringList.Add("fourth item");
			this.Configuration.DataSource = stringList;
		}

		[Test]
		public void GetFlags()
		{
			Assert.AreEqual((int)(Gtk.TreeModelFlags.ItersPersist | Gtk.TreeModelFlags.ListOnly),
				GetFlags_callback());
		}

		//private delegate int GetFlagsDelegate();
		//private delegate int GetNColumnsDelegate();
		//private delegate IntPtr GetColumnTypeDelegate(int column);
		//private delegate bool GetNodeDelegate(out int index, IntPtr path);
		//private delegate IntPtr GetPathDelegate(int index);
		//private delegate void GetValueDelegate(int index, int column, ref GLib.Value value);
		//private delegate bool NextDelegate(ref int index);
		//private delegate bool ChildrenDelegate(out int child, int parent);
		//private delegate bool HasChildDelegate(int index);
		//private delegate int NChildrenDelegate(int index);
		//private delegate bool NthChildDelegate(out int child, int parent, int n);
		//private delegate bool ParentDelegate(out int parent, int child);

	}
}
