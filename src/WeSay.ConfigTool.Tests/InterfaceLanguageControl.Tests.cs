using System;
using System.IO;
using System.Windows.Forms;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using SIL.IO;
using SIL.Reporting;
using SIL.TestUtilities;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture, RequiresSTA]
	public class InterfaceLanguageControl
	{
		private ConfigurationWindow _window;

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
		public void NewProject_LanguageChooser_ShowsEnglish()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				ComboBoxTester t = CreateNewAndGetLanguageCombo(path);
				Assert.AreEqual("English (Default)", t.Properties.SelectedItem.ToString());
			}
			finally
			{
				CloseApp();
				SIL.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test]
		public void ChangeLanguage_Reopen_HasSameProject() // RegressionTest_WS599()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				ComboBoxTester t = CreateNewAndGetLanguageCombo(path);
				//select the last one
				t.Properties.SelectedItem = t.Properties.Items[t.Properties.Items.Count - 1];
				string previouslySelected = t.Properties.SelectedItem.ToString();
				CloseApp();

				//now reopen
				OpenExisting(path);
				t = GoToTabAndGetLanguageCombo();
				Assert.AreEqual(previouslySelected, t.Properties.SelectedItem.ToString());
			}
			finally
			{
				CloseApp();
				SIL.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test]
		public void CycleWithoutGoingToUITab_DoesntLooseLanguageSetting() // Regression for WS599
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				ComboBoxTester t = CreateNewAndGetLanguageCombo(path);
				//select the last one
				t.Properties.SelectedItem = t.Properties.Items[t.Properties.Items.Count - 1];
				string previouslySelected = t.Properties.SelectedItem.ToString();
				CloseApp();

				//now reopen, close before going to ui language tab
				OpenExisting(path);
				CloseApp();

				//now reopen, this time go to uilanguage
				OpenExisting(path);
				t = GoToTabAndGetLanguageCombo();
				Assert.AreEqual(previouslySelected, t.Properties.SelectedItem.ToString());
			}
			finally
			{
				CloseApp();
				SIL.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test]
		public void RevertingToEnglish_Works()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				ComboBoxTester t = CreateNewAndGetLanguageCombo(path);
				//select the last one
				t.Properties.SelectedItem = t.Properties.Items[t.Properties.Items.Count - 1];
				CloseApp();

				//now reopen, close before going to ui language tab
				OpenExisting(path);
				t = GoToTabAndGetLanguageCombo();
				t.Properties.SelectedItem = FindDefaultEnglishItem(t.Properties);
				CloseApp();

				//now reopen, this time go to uilanguage
				OpenExisting(path);
				t = GoToTabAndGetLanguageCombo();
				Assert.AreEqual("English (Default)", t.Properties.SelectedItem.ToString());
			}
			finally
			{
				CloseApp();
				SIL.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test]
		public void SetToLanguage_FileHas2LetterLanguageCode()
		{
			using (var folder = new TemporaryFolder("InterfaceLanguageControlTests"))
			{
				var t = CreateNewAndGetLanguageCombo(folder.Path);
				t.Select("Thai"); // Select a known tranlsation; language = th
				CloseApp();
				var files = Directory.GetFiles(folder.Path, "*.WeSayUserConfig");
				Assert.That(files.Length, Is.EqualTo(1));
				AssertThatXmlIn.File(files[0]).HasAtLeastOneMatchForXpath("configuration/uiOptions[language='th']");
			}
		}

		[Test]
		public void PoProxy_LegacyStyleTranslationLinePresent_FindsValidLanguageString()
		{
			string contents = @"
# Spanish translation for wesay
# An additional comment
".Replace("'", "\"");
			using (var tempFile = new TempFile(contents))
			{
				var poProxy = new ConfigTool.InterfaceLanguageControl.PoProxy(tempFile.Path);
				Assert.IsNotEmpty(poProxy.ToString());
			}
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

		private ComboBoxTester CreateNewAndGetLanguageCombo(string path)
		{
			_window = new ConfigurationWindow(new string[] {});
			_window.DisableBackupAndChorusStuffForTests();
			_window.Show();
			_window.CreateAndOpenProject(path, "th", "Thai");
			GoToUILanguageTab();
			return new ComboBoxTester("_languageCombo", _window);
		}

		private ComboBoxTester GoToTabAndGetLanguageCombo()
		{
			GoToUILanguageTab();
			var t = new ComboBoxTester("_languageCombo", _window);
			return t;
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
