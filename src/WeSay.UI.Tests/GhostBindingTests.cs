using System;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class GhostBindingTests
	{
		private Papa _papa = new Papa();
		private WeSayTextBox _ghostFirstNameWidget;
		private WeSayTextBox _papaNameWidget;
	   private GhostBinding _binding;
		protected bool _didNotify;


		public class Child : WeSayDataObject
		{
			private MultiText first=new MultiText();
			private MultiText middle=new MultiText();

			public Child() : base(null)
			{
			}

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

			public override bool Empty
			{
				get
				{
					return First.Empty && Middle.Empty;
				}
			}
		}

		public class Papa : WeSayDataObject
		{
			private WeSay.Data.InMemoryBindingList<Child> _children = new WeSay.Data.InMemoryBindingList<Child>();

			public Papa():base(null)
			{
				_children = new WeSay.Data.InMemoryBindingList<Child>();

				WireUpEvents();
		   }

			public WeSay.Data.InMemoryBindingList<Child> Children
			{
				get { return _children; }
			}

			public override bool Empty
			{
				get
				{
					return _children.Count == 0;
				}
			}

			protected override void WireUpEvents()
		   {
			   base.WireUpEvents();
			   WireUpList(_children, "children");
		   }

		}

		void _binding_Triggered(object sender, System.ComponentModel.IBindingList list, int index, MultiTextControl previouslyGhostedControlToReuse, EventArgs args)
		{
			_didNotify = true;
		}

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			_papaNameWidget = new WeSayTextBox(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault);
			_papaNameWidget.Text  =  "John";
			_ghostFirstNameWidget = new WeSayTextBox(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault);
			_binding = new GhostBinding(_papa.Children, "First", BasilProject.Project.WritingSystems.AnalysisWritingSystemDefault, _ghostFirstNameWidget);
			_didNotify = false;
			//Window w = new Window("test");
			//VBox box = new VBox();
			//w.Add(box);
			//box.PackStart(_papaNameWidget);
			//box.PackStart(_ghostFirstNameWidget);
			//box.ShowAll();
			//w.ShowAll();
			_papaNameWidget.Show();
//            while (Gtk.Application.EventsPending())
//            { Gtk.Application.RunIteration(); }

			//Application.Run();
			_papaNameWidget.Focus();
			 _ghostFirstNameWidget.Focus();
	   }


		[Test]
		public void EmptyListGrows()
		{
			 Assert.AreEqual(0, _papa.Children.Count);
		   _ghostFirstNameWidget.Text = "Samuel";
			_ghostFirstNameWidget.PretendLostFocus();
			Assert.AreEqual(1, _papa.Children.Count);
		}

		[Test]
		public void NewItemGetsValue()
		{
			_ghostFirstNameWidget.Text = "Samuel";
			_ghostFirstNameWidget.PretendLostFocus();
			Assert.AreEqual("Samuel", _papa.Children[0].First["en"]);
		}
		 [Test]
		public void NewItemTriggersEvent()
		{
			 _binding.Triggered += new GhostBinding.GhostTriggered(_binding_Triggered);
			_ghostFirstNameWidget.Text = "Samuel";
			_ghostFirstNameWidget.PretendLostFocus();
			Assert.IsTrue(_didNotify);
		}


	}
}
