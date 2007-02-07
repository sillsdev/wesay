using System;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Language;
using WeSay.UI;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class DetailListTests
	{
		private WritingSystem _ws = new WritingSystem("test", new Font("Arial",30));
		private DetailList _control;
		private Control _focussedControl;
		private System.Windows.Forms.Form _window;

		[SetUp]
		public void Setup()
		{
			_control = new DetailList();
			//Application.Init();
		}

		/// <summary>
		/// Needed for focus-related tests
		/// </summary>
		private void ActuallyShowOnScreen()
		{
			_window =  new Form();
			_control.Dock = DockStyle.Fill;
			_window.Controls.Add(_control);
			_window.Show();
		}

		[Test]
		public void MoveInsertionPoint()
		{
			ActuallyShowOnScreen();
			Control rowOne = _control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			Control rowTwo = _control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			Control rowThree = _control.AddWidgetRow("blah", false, MakeWiredUpTextBox());

			_control.MoveInsertionPoint(0);

			Assert.AreSame(_control.GetEditControlFromReferenceControl(rowOne), _focussedControl);
			 _control.MoveInsertionPoint(1);
			Assert.AreSame(_control.GetEditControlFromReferenceControl(rowTwo), _focussedControl);
			 _control.MoveInsertionPoint(2);
			Assert.AreSame(_control.GetEditControlFromReferenceControl(rowThree), _focussedControl);
			_window.Close();
	  }

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MoveInsertionPoint_RowLessThan0_throws()
		{
			_control.MoveInsertionPoint(-1);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MoveInsertionPoint_NoRows_throws()
		{
			_control.MoveInsertionPoint(0);
		}

		[Test]
		[ExpectedException(typeof(ArgumentOutOfRangeException))]
		public void MoveInsertionPoint_PastLastRow_throws()
		{
			Control rowOne = _control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			Control rowTwo = _control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			_control.MoveInsertionPoint(2);
		}

		private WeSayTextBox MakeWiredUpTextBox()
		{
			WeSayTextBox box = new WeSayTextBox(_ws);
			box.GotFocus+=new System.EventHandler(box_GotFocus);
			return box;
		}

		void box_GotFocus(object sender, EventArgs e)
		{
			_focussedControl = (Control)sender;
		}

		[Test]
		[ExpectedException(typeof(ArgumentException))]
		public void Clear()
		{
			Control rowOne = AddRow();
			_control.Clear();
			_control.GetRowOfControl(rowOne); //should throw
		}

		[Test]
		public void RowOrder()
		{
			Control rowOne = AddRow();
			Control rowTwo = AddRow();
			Assert.AreEqual(0,_control.GetRowOfControl(rowOne));
			Assert.AreEqual(1,_control.GetRowOfControl(rowTwo));

			//insert one in between
			Control rowOneHalf = AddRow(1);
			 Assert.AreEqual(0,_control.GetRowOfControl(rowOne));
		   Assert.AreEqual(1, _control.GetRowOfControl(rowOneHalf));
			Assert.AreEqual(2,_control.GetRowOfControl(rowTwo));

			//stick one at the end
			Control rowLast = AddRow(-1);
		   Assert.AreEqual(0,_control.GetRowOfControl(rowOne));
		   Assert.AreEqual(1, _control.GetRowOfControl(rowOneHalf));
		   Assert.AreEqual(2,_control.GetRowOfControl(rowTwo));
		   Assert.AreEqual(3,_control.GetRowOfControl(rowLast));

			//insert one before all of the others
		   Control rowFirst= AddRow(0);
		   Assert.AreEqual(0,_control.GetRowOfControl(rowFirst));
		   Assert.AreEqual(1,_control.GetRowOfControl(rowOne));
		   Assert.AreEqual(2, _control.GetRowOfControl(rowOneHalf));
		   Assert.AreEqual(3,_control.GetRowOfControl(rowTwo));
		   Assert.AreEqual(4,_control.GetRowOfControl(rowLast));
	  }

		private Control AddRow()
		{
			//don't factor this to use the version that calls with an explicit row!
			return _control.AddWidgetRow("blah", false, new WeSayTextBox(_ws));
		}
		 private Control AddRow(int row)
		{
			return _control.AddWidgetRow("blah", false, new WeSayTextBox(_ws), row);
		}
	}
}
