using System;
using System.ComponentModel;
using Gtk;
using NUnit.Framework;
using WeSay.Language;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class GhostBindingTests
	{
		private Papa _papa = new Papa();
		private Entry _ghostFirstNameWidget;
		private GhostBinding _binding;

		public class Child : WeSay.LexicalModel.WeSayDataObject
		{
			private MultiText first=new MultiText();
			private MultiText middle=new MultiText();

			public MultiText First
			{
				get { return first; }
				set { first = value; }
			}

			public MultiText Middle
			{
				get { return middle; }
				set { middle = value; }
			}
		}

		public class Papa : WeSay.LexicalModel.WeSayDataObject
		{
			private BindingList<Child> _children = new BindingList<Child>();

			public Papa()
			{
				_children = new BindingList<Child>();

				WireUpEvents();
		   }

			public BindingList<Child> Children
			{
				get { return _children; }
			}

			protected override void WireUpEvents()
		   {
			   base.WireUpEvents();
			   WireUpList(_children, "children");
		   }

		}

		[SetUp]
		public void Setup()
		{
			Application.Init();
			_ghostFirstNameWidget = new Entry();
			_binding = new GhostBinding(_papa.Children, "First", "en", _ghostFirstNameWidget);
		}


		[Test]
		public void EmptyListGrows()
		{
			_ghostFirstNameWidget.Text = "Samuel";
			Assert.AreEqual(1, _papa.Children.Count);
		}

		[Test]
		public void NewItemGetsValue()
		{
			_ghostFirstNameWidget.Text = "Samuel";
			Assert.AreEqual("Samuel",_papa.Children[0].First["en"]);
		}
	}
}
