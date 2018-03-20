using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Gecko;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using SIL.IO;
using SIL.WritingSystems;
using WeSay.UI.TextBoxes;

namespace WeSay.UI.Tests
{
	[TestFixture, RequiresSTA]
	[Platform(Exclude = "Unix")]  // Cant initialize XULRunner in these tests on Linux.
	class GeckoTests : NUnitFormTest
	{
		[DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
		[return: MarshalAs(UnmanagedType.Bool)]
		static extern bool SetDllDirectory(string lpPathName);
		private Form _window;

		// GeckoListBoxTests
		private GeckoListBox _listBox;
		private bool _itemToNotDrawYetDrawn;
		private int _countOfItemsDrawn;

		[TestFixtureTearDown]
		public void FixtureCleanup()
		{
			// Shutting down xul runner prevents subsequent tests from running successfully
			//ShutDownXulRunner();
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

		#region GeckoBox
		[Test]
		public void GeckoBox_CreateWithWritingSystem()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			IWeSayTextBox textBox = new GeckoBox(ws, null);
			Assert.IsNotNull(textBox);
			Assert.AreSame(ws, textBox.WritingSystem);
		}
		[Test]
		[Ignore("FLAKY - sometimes fails in tc continuous build.")]
		public void GeckoBox_KeyboardInputTest()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			IWeSayTextBox textBox = new GeckoBox(ws, "ControlUnderTest");
			Assert.IsNotNull(textBox);
			Assert.AreSame(ws, textBox.WritingSystem);
			_window.Controls.Add((GeckoBox)textBox);
			_window.Show();
			ControlTester t = new ControlTester("ControlUnderTest", _window);
			KeyboardController keyboardController = new KeyboardController(t);
			Application.DoEvents();
			keyboardController.Press("T");
			keyboardController.Press("e");
			keyboardController.Press("s");
			keyboardController.Press("t");
			Application.DoEvents();
			Assert.AreEqual("Test", textBox.Text);
			keyboardController.Dispose();
		}

		[Test]
		[Ignore("FLAKY - sometimes fails in tc continuous build.")]
		public void GeckoBox_KeyboardInputAfterInitialValueTest()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			IWeSayTextBox textBox = new GeckoBox(ws, "ControlUnderTest");
			Assert.IsNotNull(textBox);
			Assert.AreSame(ws, textBox.WritingSystem);
			_window.Controls.Add((GeckoBox)textBox);
			_window.Show();
			ControlTester t = new ControlTester("ControlUnderTest", _window);
			textBox.Text = "Test";
			KeyboardController keyboardController = new KeyboardController(t);
			Application.DoEvents();
			keyboardController.Press(Key.HOME);
			Application.DoEvents();
			keyboardController.Press("V");
			keyboardController.Press("a");
			keyboardController.Press("l");
			keyboardController.Press("u");
			keyboardController.Press("e");
			keyboardController.Press(" ");
			Application.DoEvents();
			Assert.AreEqual("Value Test", textBox.Text);
			keyboardController.Dispose();
		}

		[Test]
		public void GeckoBox_KeyboardInputWhenReadOnlyTest()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			IWeSayTextBox textBox = new GeckoBox(ws, "ControlUnderTest");
			textBox.ReadOnly = true;
			Assert.IsNotNull(textBox);
			Assert.AreSame(ws, textBox.WritingSystem);
			_window.Controls.Add((GeckoBox)textBox);
			_window.Show();
			ControlTester t = new ControlTester("ControlUnderTest", _window);
			textBox.Text = "Value";
			KeyboardController keyboardController = new KeyboardController(t);
			Application.DoEvents();
			keyboardController.Press(Key.END);
			Application.DoEvents();
			keyboardController.Press(" ");
			keyboardController.Press("T");
			keyboardController.Press("e");
			keyboardController.Press("s");
			keyboardController.Press("t");
			Application.DoEvents();
			Assert.AreEqual("Value", textBox.Text);
			keyboardController.Dispose();
		}

		[Test]
		public void GeckoBox_SetWritingSystem_Null_Throws()
		{
			IWeSayTextBox textBox = new GeckoBox();
			Assert.Throws<ArgumentNullException>(() => textBox.WritingSystem = null);
		}

		[Test]
		public void GeckoBox_WritingSystem_Unassigned_Get_Throws()
		{
			IWeSayTextBox textBox = new GeckoBox();
			WritingSystemDefinition ws;
			Assert.Throws<InvalidOperationException>(() => ws = textBox.WritingSystem);
		}

		[Test]
		public void GeckoBox_WritingSystem_Unassigned_Focused_Throws()
		{
			IWeSayTextBox textBox = new GeckoBox();
			Assert.Throws<InvalidOperationException>(() => textBox.AssignKeyboardFromWritingSystem());
		}

		[Test]
		public void GeckoBox_WritingSystem_Unassigned_Unfocused_Throws()
		{
			IWeSayTextBox textBox = new GeckoBox();
			Assert.Throws<InvalidOperationException>(() => textBox.ClearKeyboard());
		}


		#endregion

		#region GeckoListBoxTests

		[Test]
		public void GeckoListBox_CreateWithWritingSystem()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr") { DefaultFont = new FontDefinition("Arial") };
			var listBox = new GeckoListBox(ws, null);
			Assert.IsNotNull(listBox);
			Assert.AreSame(ws, listBox.WritingSystem);
			Assert.AreSame(ws, listBox.FormWritingSystem);
		}

		[Test]
		public void GeckoListBox_TestAddItem()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr") { DefaultFont = new FontDefinition("Arial") };
			WritingSystemDefinition ws2 = new WritingSystemDefinition("en") { DefaultFont = new FontDefinition("Arial") };
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
		public void GeckoListBox_TestDrawItem()
		{
			_countOfItemsDrawn = 0;
			_itemToNotDrawYetDrawn = false;
			WritingSystemDefinition ws = new WritingSystemDefinition("fr") { DefaultFont = new FontDefinition("Arial") };
			WritingSystemDefinition ws2 = new WritingSystemDefinition("en") { DefaultFont = new FontDefinition("Arial") };
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
			_listBox.ItemDrawer = GeckoListBox_TestDrawItem;
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
		[Category("SkipOnTeamCity")]
		[Ignore("FLAKY - sometimes fails in run all in VS.")]
		public void GeckoListBox_TestGetRectangle()
		{
			_countOfItemsDrawn = 0;
			_itemToNotDrawYetDrawn = false;
			WritingSystemDefinition ws = new WritingSystemDefinition("fr") { DefaultFont = new FontDefinition("Arial") };
			WritingSystemDefinition ws2 = new WritingSystemDefinition("en") { DefaultFont = new FontDefinition("Arial") };
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

			_listBox.ItemDrawer = GeckoListBox_TestDrawItem;
			_listBox.ListCompleted();

			Application.DoEvents();

			_window.Controls.Add(_listBox);
			_window.Show();
			ControlTester t = new ControlTester("ControlUnderTest", _window);

			Application.DoEvents();
			Rectangle r = _listBox.GetItemRectangle(1);
			Application.DoEvents();

			using (MouseController mc = new MouseController(t))
			{
				mc.Click(r.Right + 5, r.Top + 5);
			}

			Application.DoEvents();
			Application.DoEvents();

			Assert.AreEqual(1, _listBox.SelectedIndex);
			Assert.AreSame(_listBox.SelectedItem, saab);
		}

		private void GeckoListBox_TestDrawItem(object item, object a)
		{
			_countOfItemsDrawn++;
			int itemIndex = (int)a;
			if (itemIndex == 1)
			{
				_itemToNotDrawYetDrawn = true;
			}
			_listBox.ItemToHtml(item.ToString(), itemIndex, true, Color.Black);
		}

		#endregion

		#region GeckoComboBoxTests

		[Test]
		public void GeckoCombox_CreateWithWritingSystem()
		{
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			var comboBox = new GeckoComboBox(ws, null);
			Assert.IsNotNull(comboBox);
			Assert.AreSame(ws, comboBox.WritingSystem);
		}

		[Test]
		public void GeckoCombox_TestAddItem()
		{
			//try
			//{

			int j = 0;
			String value = "";
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			var comboBox = new GeckoComboBox();
			comboBox.WritingSystem = ws;
			comboBox.Name = "ControlUnderTest";
			Assert.IsNotNull(comboBox);
			Assert.AreSame(ws, comboBox.WritingSystem);

			String volvo = "Volvo";
			String saab = "Saab";
			String toyota = "Toyota";

			comboBox.AddItem(volvo);
			comboBox.AddItem(saab);
			comboBox.AddItem(toyota);
			comboBox.ListCompleted();

			Application.DoEvents();

			_window.Controls.Add((GeckoComboBox)comboBox);
			_window.Show();
			ControlTester t = new ControlTester("ControlUnderTest", _window);
			KeyboardController keyboardController = new KeyboardController(t);
			Application.DoEvents();

			j = comboBox.Length;
			keyboardController.Press("{DOWN}");
			Application.DoEvents();
			keyboardController.Release("{DOWN}");
			Application.DoEvents();
			value = (String)comboBox.SelectedItem;
			Assert.AreEqual(3, j);
			Assert.AreEqual("Saab", value);
			keyboardController.Press("{DOWN}");
			Application.DoEvents();
			keyboardController.Release("{DOWN}");
			Application.DoEvents();
			value = (String)comboBox.SelectedItem;
			Application.DoEvents();
			Application.DoEvents();

			Assert.AreEqual(3, j);
			Assert.AreEqual("Toyota", value);
			/*}
			catch (Exception)
			{
				// Team city sometimes throws exception on this test
				// Rather than remove a test that usually works, I am
				// putting this in to allow it to pass when timing problems
				// occur.
			}*/
		}

		[Test]
		public void GeckoCombox_SetWritingSystem_Null_Throws()
		{
			var textBox = new GeckoComboBox();
			Assert.Throws<ArgumentNullException>(() => textBox.WritingSystem = null);
		}
		[Test]
		public void GeckoCombox_SetWritingSystem_DoesntThrow()
		{
			var textBox = new GeckoComboBox();
			WritingSystemDefinition ws = new WritingSystemDefinition("fr");
			ws.DefaultFont = new FontDefinition("Arial");
			ws.DefaultFontSize = 12;
			Assert.DoesNotThrow(() => textBox.WritingSystem = ws);
		}

		[Test]
		public void GeckoCombox_WritingSystem_Unassigned_Get_Throws()
		{
			var textBox = new GeckoComboBox();
			WritingSystemDefinition ws;
			Assert.Throws<InvalidOperationException>(() => ws = textBox.WritingSystem);
		}

		#endregion




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
					string librarySearchPath = Environment.GetEnvironmentVariable("LD_LIBRARY_PATH") ?? String.Empty;
					if (!librarySearchPath.Contains(xulRunnerLocation))
						throw new ApplicationException("LD_LIBRARY_PATH must contain " + xulRunnerLocation);
#else
					string xulRunnerLocation = Path.Combine(FileLocator.DirectoryOfTheApplicationExecutable, "Firefox");
					if (!Directory.Exists(xulRunnerLocation))
					{
						throw new ApplicationException("XULRunner needs to be installed to " + xulRunnerLocation);
					}
					if (!SetDllDirectory(xulRunnerLocation))
					{
						throw new ApplicationException("SetDllDirectory failed for " + xulRunnerLocation);
					}
#endif
					Xpcom.EnableProfileMonitoring = false;
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
