using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace WeSay.UI.Tests
{
	[TestFixture]
	public class WeSayListViewTests: NUnitFormTest
	{
		private Form _window;
		private WeSayListView _listView;
		private List<string> _entries;

		public override void Setup()
		{
			base.Setup();
			_entries = new List<string>();
			_entries.Add("Uno");
			_entries.Add("Dos");
			_entries.Add("Tres");
			_listView = new WeSayListView();
			_listView.Name = "_listView";
			_listView.DataSource = _entries;
			_window = new Form();
			_window.Size = new Size(500, 500);
			_window.Controls.Add(_listView);
			_window.Show();
		}

		public override void TearDown()
		{
			_window.Dispose();
			_listView.Dispose();
			base.TearDown();
		}

		[Test]
		[Category("NUnit Windows Forms;KnownMonoIssue")]
		[Platform(Exclude="Unix")] // MouseController uses Win32.GetCursorPos so not portable
		public void ClickOnWhiteSpaceToRightOfEntry_ThenKeyboardNavigate_CorrectEntrySelected()
		{
			ListViewTester l = new ListViewTester("_listView", _window);
			using (MouseController mc = new MouseController(l))
			{
				using (KeyboardController kc = new KeyboardController(l))
				{
					l.Select(0);
					Rectangle r = l.Properties.GetItemRect(1);
					mc.Click(r.Right + 1, r.Top + 1);
					kc.Press("{DOWN}");
					kc.Release("{DOWN}");
				}
			}
			Assert.AreEqual(2, l.Properties.SelectedIndices[0]);
		}

		[Test]
		[Category("NUnit Windows Forms;KnownMonoIssue")]
		[Platform(Exclude="Unix")] // MouseController uses Win32.GetCursorPos so not portable
		public void DoubleClickOnWhiteSpaceToRightOfEntry_EntryAlreadySelected_EntryStaysSelected()
		{
			ListViewTester l = new ListViewTester("_listView", _window);
			using (MouseController mc = new MouseController(l))
			{
				Rectangle r = l.Properties.GetItemRect(0);
				mc.Click(r.Right + 1, r.Top + 1);
				// move enough to not confuse click with double-click
				mc.DoubleClick(r.Right + SystemInformation.DoubleClickSize.Width + 2, r.Top + 1);
			}
			Assert.AreEqual(1, l.Properties.SelectedIndices.Count);
			Assert.AreEqual(0, l.Properties.SelectedIndices[0]);
		}
	}
}