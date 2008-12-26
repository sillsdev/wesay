using System;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Foundation;
using WeSay.UI.TextBoxes;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class DetailListTests
	{
		private readonly WritingSystem _ws = new WritingSystem("test", new Font("Arial", 30));
		private DetailList _control;
		private Control _focussedControl;
		private Form _window;

		[SetUp]
		public void Setup()
		{
			_control = new DetailList();
			//Application.Init();
		}

		[TearDown]
		public void TearDown()
		{
			_control.Dispose();
		}

		/// <summary>
		/// Needed for focus-related tests
		/// </summary>
		private void ActuallyShowOnScreen()
		{
			_window = new Form();
			_control.Dock = DockStyle.Fill;
			_window.Controls.Add(_control);
			_window.Show();
		}

		[Test]
		public void MoveInsertionPoint()
		{
			ActuallyShowOnScreen();
			_control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			_control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			_control.AddWidgetRow("blah", false, MakeWiredUpTextBox());

			_control.MoveInsertionPoint(0);

			Assert.AreSame(_control.GetEditControlFromRow(0), _focussedControl);
			_control.MoveInsertionPoint(1);
			Assert.AreSame(_control.GetEditControlFromRow(1), _focussedControl);
			_control.MoveInsertionPoint(2);
			Assert.AreSame(_control.GetEditControlFromRow(2), _focussedControl);
			_window.Close();
		}

#if (DEBUG)
		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void MoveInsertionPoint_RowLessThan0_throws()
		{
			_control.MoveInsertionPoint(-1);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void MoveInsertionPoint_NoRows_throws()
		{
			_control.MoveInsertionPoint(0);
		}

		[Test]
		[ExpectedException(typeof (ArgumentOutOfRangeException))]
		public void MoveInsertionPoint_PastLastRow_throws()
		{
			_control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			_control.AddWidgetRow("blah", false, MakeWiredUpTextBox());
			_control.MoveInsertionPoint(2);
		}
#endif

		private WeSayTextBox MakeWiredUpTextBox()
		{
			WeSayTextBox box = new WeSayTextBox(_ws, null);
			box.GotFocus += box_GotFocus;
			return box;
		}

		private void box_GotFocus(object sender, EventArgs e)
		{
			_focussedControl = (Control) sender;
		}

		[Test]
		public void Clear()
		{
			Control rowOne = AddRow();
			_control.Clear();
			Assert.IsFalse(_control.Contains(rowOne));
		}

		[Test]
		public void Test()
		{
			AddRow(0);
		}

		[Test]
		public void RowOrder()
		{
			Control rowOne = AddRow();
			Control rowTwo = AddRow();
			Assert.AreEqual(0, _control.GetRow(rowOne));
			Assert.AreEqual(1, _control.GetRow(rowTwo));

			//insert one in between
			Control rowOneHalf = AddRow(1);
			Assert.AreEqual(0, _control.GetRow(rowOne));
			Assert.AreEqual(1, _control.GetRow(rowOneHalf));
			Assert.AreEqual(2, _control.GetRow(rowTwo));

			//stick one at the end
			Control rowLast = AddRow(-1);
			Assert.AreEqual(0, _control.GetRow(rowOne));
			Assert.AreEqual(1, _control.GetRow(rowOneHalf));
			Assert.AreEqual(2, _control.GetRow(rowTwo));
			Assert.AreEqual(3, _control.GetRow(rowLast));

			//insert one before all of the others
			Control rowFirst = AddRow(0);
			Assert.AreEqual(0, _control.GetRow(rowFirst));
			Assert.AreEqual(1, _control.GetRow(rowOne));
			Assert.AreEqual(2, _control.GetRow(rowOneHalf));
			Assert.AreEqual(3, _control.GetRow(rowTwo));
			Assert.AreEqual(4, _control.GetRow(rowLast));
		}

		private Control AddRow()
		{
			//don't factor this to use the version that calls with an explicit row!
			return _control.AddWidgetRow("blah", false, new WeSayTextBox(_ws, null));
		}

		private Control AddRow(int row)
		{
			return _control.AddWidgetRow("blah", false, new WeSayTextBox(_ws, null), row, false);
		}
	}
}
