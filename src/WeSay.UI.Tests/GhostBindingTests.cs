using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using NUnit.Framework;
using WeSay.Project;
using WeSay.Project;
using WeSay.LexicalModel.Foundation;
using WeSay.Project;
using WeSay.UI.TextBoxes;
using Palaso.Lift;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class GhostBindingTests
	{
		private readonly Papa _papa = new Papa();
		private WeSayTextBox _ghostFirstNameWidget;
		private WeSayTextBox _papaNameWidget;
		private GhostBinding<Child> _binding;
		protected bool _didNotify;

		private readonly string _writingSystemId =
				BasilProject.Project.WritingSystems.TestWritingSystemAnalId;

		public class Child: PalasoDataObject
		{
			private MultiText first = new MultiText();
			private MultiText middle = new MultiText();

			public Child(): base(null) {}

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

			public override bool IsEmpty
			{
				get { return First.Empty && Middle.Empty; }
			}
		}

		public class Papa: PalasoDataObject
		{
			private readonly BindingList<Child> _children = new BindingList<Child>();

			public Papa(): base(null)
			{
				_children = new BindingList<Child>();

				WireUpEvents();
			}

			public BindingList<Child> Children
			{
				get { return _children; }
			}

			public override bool IsEmpty
			{
				get { return _children.Count == 0; }
			}

			protected override void WireUpEvents()
			{
				base.WireUpEvents();
				WireUpList(_children, "children");
			}
		}

		private void _binding_LayoutNeededAfterMadeReal(object sender,
														IList<Child> list,
														int index,
														MultiTextControl
																previouslyGhostedControlToReuse,
														bool doGoToNextField,
														EventArgs args)
		{
			_didNotify = true;
		}

		[SetUp]
		public void Setup()
		{
			BasilProject.InitializeForTests();

			WritingSystem writingSystem = new WritingSystem(_writingSystemId,
															new Font(FontFamily.GenericSansSerif, 24));
			_papaNameWidget = new WeSayTextBox(writingSystem, null);
			_papaNameWidget.Text = "John";
			_ghostFirstNameWidget = new WeSayTextBox(writingSystem, null);
			_binding = new GhostBinding<Child>(null,
				_papa.Children,
											   "First",
											   writingSystem,
											   _ghostFirstNameWidget);
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
			Assert.AreEqual("Samuel", _papa.Children[0].First[_writingSystemId]);
		}

		[Test]
		public void NewItemTriggersEvent()
		{
			_binding.ReferenceControl = _papaNameWidget;
			//just has to be *something*, else the trigger won't call us back

			_binding.LayoutNeededAfterMadeReal += _binding_LayoutNeededAfterMadeReal;
			_ghostFirstNameWidget.Text = "Samuel";
			_ghostFirstNameWidget.PretendLostFocus();
			Assert.IsTrue(_didNotify);
		}
	}
}