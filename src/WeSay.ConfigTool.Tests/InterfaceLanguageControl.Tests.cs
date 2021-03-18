using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using NUnit.Framework;
using SIL.IO;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture, Apartment(ApartmentState.STA)]
	public class InterfaceLanguageControl
	{
		private ConfigurationWindow _window;

		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Sldr.Initialize(true);
		}

		[OneTimeTearDown]
		public void OneTimeTeardown()
		{
			Sldr.Cleanup();
		}

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();

			//           _window.OpenProject(BasilProject.GetPretendProjectDirectory());
		}

		private void GoToUILanguageTab()
		{
			var toolstrip = (ToolStrip) _window.Controls.Find("_areasToolStrip", true)[0];
			foreach (ToolStripButton button in toolstrip.Items)
			{
				if (button.Text.Contains("Language"))
				{
					button.PerformClick();
					Application.DoEvents();
					break;
				}
			}
		}

		[TearDown]
		public void TearDown()
		{
			SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
		}

		[Test]
		public void PoProxy_TransifexStyleLanguageTeamLinePresent_FindsValidLanguageString()
		{
			string contents = @"
# An additional comment
'Language-Team: Spanish <es@li.org>\n'
".Replace("'", "\"");
			using (var tempFile = new TempFile(contents))
			{
				var poProxy = new ConfigTool.InterfaceLanguageControl.PoProxy(tempFile.Path);
				Assert.AreEqual("Spanish", poProxy.ToString());
			}
		}

		[Test]
		public void PoProxy_PoFilesInCommonFolder_EachPoFileHasLanguageName()
		{
			foreach (string file in Directory.GetFiles(BasilProject.ApplicationCommonDirectory, "*.po"))
			{
				var poProxy = new ConfigTool.InterfaceLanguageControl.PoProxy(file);
				Assert.IsNotEmpty(poProxy.ToString(), String.Format("Could not extract language name from po file: {0}", file));
			}


		}


		private static object FindDefaultEnglishItem(ComboBox combo)
		{
			foreach (object o in combo.Items)
			{
				if (o.ToString() == "English (Default)")
				{
					return o;
				}
			}
			return null;
		}

		private void CloseApp()
		{
			if (_window != null)
			{
				_window.Close();
				_window.Dispose();
				_window = null;
			}
		}

		private void OpenExisting(string path)
		{
			_window = new ConfigurationWindow(new string[] {});
			_window.DisableBackupAndChorusStuffForTests();
			_window.Show();
			_window.OpenProject(path);
		}
	}
}
