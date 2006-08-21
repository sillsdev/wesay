using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;

namespace WeSay.TreeViewIList.Tests
{
	public class TestPerson
{
	public enum GenderType {
		Male,
		Female
	}

	private string _name;
	private int _number;
	private GenderType _gender;

	public string Name
	{
		get { return this._name; }
	}

	public int Number
	{
		get { return this._number; }
	}

	public GenderType Gender
	{
		get { return this._gender; }
	}


	public TestPerson (string name, int number, GenderType gender)
	{
		this._name = name;
		this._number = number;
		this._gender = gender;
	}

	static public TestPerson GetRandomPerson(){
		Random random = new Random();
		GenderType gender = GenderType.Male;
		if(random.Next(0,1) == 0){
			gender = GenderType.Female;
		}
		string name = string.Empty;
		int max = random.Next(6);
		for(int i = 0; i < max; i++){
			name += Convert.ToChar(random.Next(64, 128));
		}
		return new TestPerson(name, random.Next(), gender);
	}
}



	[TestFixture]
	public class TreeViewTests
	{
		TreeViewAdaptorIList treeview;

		[TestFixtureSetUp]
		public void TestFixtureSetup()
		{
			Gtk.Application.Init();

			List<TestPerson> list = new List<TestPerson>();
			for (int i = 0; i < 12; ++i)
			{
				list.Add(TestPerson.GetRandomPerson());
			}
			treeview = new TreeViewAdaptorIList(list);
			treeview.AppendColumn("Name", new Gtk.CellRendererText());
			treeview.Column_Types.Add(GLib.GType.String);
			treeview.AppendColumn("Number", new Gtk.CellRendererText());
			treeview.Column_Types.Add(GLib.GType.Int);
			treeview.AppendColumn("Gender", new Gtk.CellRendererText());
			treeview.Column_Types.Add(GLib.GType.String);

			treeview.GetValueStrategy = delegate(object o, int column)
				{
					TestPerson p = (TestPerson) o;
					switch (column)
					{
						case 0:
							return p.Name;
						case 1:
							return p.Number;
						case 2:
							if (p.Gender == TestPerson.GenderType.Male)
							{
								return "Male";
							}
							return "Female";
						default:
							Assert.Fail("Invalid column number passed to GetValueStrategy");
							break;
					}
					return o;
				};
			treeview.Activate();
		}

		[Test]
		public void Column_Types()
		{
			Assert.AreEqual(3, treeview.Column_Types.Count);
			Assert.AreEqual(GLib.GType.String,treeview.Column_Types[0]);
			Assert.AreEqual(GLib.GType.Int, treeview.Column_Types[1]);
			Assert.AreEqual(GLib.GType.String, treeview.Column_Types[2]);
		}

		[Test]
		public void Columns()
		{
			Assert.AreEqual(3, treeview.Columns.Length);
		}

	}
}
