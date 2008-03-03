using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.CommonTools;
using WeSay.Data;
using WeSay.LexicalModel;

namespace WeSay.App.Tests
{
	[TestFixture]
	public class DashBoardTests
	{
		[SetUp]
		public void Setup()
		{
			Form window = new Form();
			window.BackColor = System.Drawing.SystemColors.Window;
			window.Size = new Size(800, 600);
			InMemoryRecordListManager manager = new InMemoryRecordListManager();
			IRecordList<LexEntry> entries = manager.GetListOfType<LexEntry>();
			entries.AddNew();

			Dash dash = new Dash(manager);
			dash.Dock = DockStyle.Fill;
			window.Controls.Add(dash);
			Application.Run(window);
		}
		[Test, Ignore("not really a test")]
		public void Run()
		{
		}
	}


}
