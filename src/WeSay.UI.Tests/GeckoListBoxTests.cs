﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using Palaso.WritingSystems;
using Palaso.IO;
using WeSay.UI.TextBoxes;
using Gecko;

namespace WeSay.UI.Tests
{
	[TestFixture]
	[Platform(Exclude = "Unix")] // Cant initialize XULRunner in these tests on Linux.
	internal class GeckoListBoxTests : NUnitFormTest
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		private static extern bool SetDllDirectory(string lpPathName);

		private Form _window;
		private GeckoListBox _listBox;
		private bool _itemToNotDrawYetDrawn;
		private int _countOfItemsDrawn;

		[TestFixtureTearDown]
		public void FixtureCleanup()
		{
			// Shutting down xul runner prevents subsequent tests from running successfully
//			ShutDownXulRunner();
		}

		[TestFixtureSetUp]
		public void FixtureSetup()
		{
			SetUpXulRunner();
		}

		[SetUp]
		public override void Setup()
		{
			base.Setup();
			_window = new Form();
			_window.Size = new Size(500, 500);
		}

		[TearDown]
		public override void TearDown()
		{
			_window.Dispose();
			base.TearDown();
		}

		[Test]
		public void CreateWithWritingSystem()
		{
			IWritingSystemDefinition ws = WritingSystemDefinition.Parse("fr");
			var listBox = new GeckoListBox(ws, null);
			Assert.IsNotNull(listBox);
			Assert.AreSame(ws, listBox.WritingSystem);
			Assert.AreSame(ws, listBox.FormWritingSystem);
		}

		[Test]
		public void TestAddItem()
		{
			IWritingSystemDefinition ws = WritingSystemDefinition.Parse("fr");
			IWritingSystemDefinition ws2 = WritingSystemDefinition.Parse("en");
			var listBox = new GeckoListBox();
			listBox.FormWritingSystem = ws;
			listBox.MeaningWritingSystem = ws2;
			listBox.Name = "ControlUnderTest";
			Assert.IsNotNull(listBox);

			String volvo = "Volvo";
			String saab = "Saab";
			String toyota = "Toyota";

			listBox.AddItem(volvo);
			listBox.AddItem(saab);
			listBox.AddItem(toyota);

			Assert.AreSame(saab, listBox.GetItem(1));
			Assert.AreSame(toyota, listBox.GetItem(2));
			Assert.AreEqual(3, listBox.Length);
			Assert.AreSame(ws2, listBox.MeaningWritingSystem);

			listBox.Clear();
			Assert.AreEqual(0, listBox.Length);

		}

		[Test]
		public void TestDrawItem()
		{
			_countOfItemsDrawn = 0;
			_itemToNotDrawYetDrawn  = false;
			IWritingSystemDefinition ws = WritingSystemDefinition.Parse("fr");
			IWritingSystemDefinition ws2 = WritingSystemDefinition.Parse("en");
			_listBox = new GeckoListBox();
			_listBox.FormWritingSystem = ws;
			_listBox.MeaningWritingSystem = ws2;
			_listBox.Name = "ControlUnderTest";
			Assert.IsNotNull(_listBox);

			String volvo = "Volvo";
			String saab = "Saab";
			String toyota = "Toyota";

			_listBox.AddItem(volvo);
			_listBox.AddItem(saab);
			_listBox.AddItem(toyota);

			_listBox.ItemToNotDrawYet = saab;
			_listBox.ItemDrawer = TestDrawItem;
			_listBox.ListCompleted();

			Application.DoEvents();

			_window.Controls.Add((GeckoListBox)_listBox);
			_window.Show();

			Application.DoEvents();

			// Count may be 2 or 4 depending on whether adjust height
			// ran yet or not
			if (_countOfItemsDrawn != 2)
			{
				Assert.AreEqual(4, _countOfItemsDrawn);
			}
			Assert.IsTrue(_itemToNotDrawYetDrawn == false);
		}

		[Test]
		public void TestGetRectangle()
		{
			_countOfItemsDrawn = 0;
			_itemToNotDrawYetDrawn = false;
			IWritingSystemDefinition ws = WritingSystemDefinition.Parse("fr");
			IWritingSystemDefinition ws2 = WritingSystemDefinition.Parse("en");
			_listBox = new GeckoListBox();
			_listBox.FormWritingSystem = ws;
			_listBox.MeaningWritingSystem = ws2;
			_listBox.Name = "ControlUnderTest";
			Assert.IsNotNull(_listBox);

			String volvo = "Volvo";
			String saab = "Saab";
			String toyota = "Toyota";

			_listBox.AddItem(volvo);
			_listBox.AddItem(saab);
			_listBox.AddItem(toyota);

			_listBox.ItemDrawer = TestDrawItem;
			_listBox.ListCompleted();

			Application.DoEvents();

			_window.Controls.Add((GeckoListBox)_listBox);
			_window.Show();
			ControlTester t = new ControlTester("ControlUnderTest", _window);

			Application.DoEvents();
			Application.DoEvents();

			using (MouseController mc = new MouseController(t))
			{
				Rectangle r = _listBox.GetItemRectangle(1);
				mc.Click(r.Right + 1, r.Top + 1);
			}

			Application.DoEvents();
			Application.DoEvents();

			Assert.AreEqual(1, _listBox.SelectedIndex);
			Assert.AreSame(_listBox.SelectedItem, saab);
		}

		private void TestDrawItem(object item, object a)
		{
			_countOfItemsDrawn++;
			int itemIndex = (int)a;
			if (itemIndex == 1)
			{
				_itemToNotDrawYetDrawn = true;
			}
			_listBox.ItemToHtml(item.ToString(), itemIndex, true, Color.Black);
		}

		public static void SetUpXulRunner()
		{
			if (!Xpcom.IsInitialized)
			{
				try
				{
#if __MonoCS__
					// Initialize XULRunner - required to use the geckofx WebBrowser Control (GeckoWebBrowser).
					string xulRunnerLocation = XULRunnerLocator.GetXULRunnerLocation();
					if (String.IsNullOrEmpty(xulRunnerLocation))
						throw new ApplicationException("The XULRunner library is missing or has the wrong version");
					Environment.SetEnvironmentVariable("LD_LIBRARY_PATH", xulRunnerLocation);
					string librarySearchPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? String.Empty;
					if (!librarySearchPath.Contains(xulRunnerLocation))
						throw new ApplicationException("LD_LIBRARY_PATH must contain " + xulRunnerLocation);
#else
					string xulRunnerLocation = Path.Combine(FileLocator.DirectoryOfApplicationOrSolution, "xulrunner");
					if (!Directory.Exists(xulRunnerLocation))
					{
						throw new ApplicationException("XULRunner needs to be installed to " + xulRunnerLocation);
					}
					if (!SetDllDirectory(xulRunnerLocation))
					{
						throw new ApplicationException("SetDllDirectory failed for " + xulRunnerLocation);
					}
#endif
					Xpcom.Initialize(xulRunnerLocation);
					GeckoPreferences.User["gfx.font_rendering.graphite.enabled"] = true;
				}
				catch (ApplicationException e)
				{
					Assert.Fail();
				}
				catch (Exception e)
				{
					Assert.Fail();
				}
			}

		}
		private static void ShutDownXulRunner()
		{
			if (Xpcom.IsInitialized)
			{
				// The following line appears to be necessary to keep Xpcom.Shutdown()
				// from triggering a scary looking "double free or corruption" message most
				// of the time.  But the Xpcom.Shutdown() appears to be needed to keep the
				// program from hanging around sometimes after it supposedly exits.
				var foo = new GeckoWebBrowser();
				Xpcom.Shutdown();
			}
		}
	}
}