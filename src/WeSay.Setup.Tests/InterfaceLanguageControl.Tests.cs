using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NUnit.Framework;
using WeSay.Project;
using NUnit.Extensions.Forms;

namespace WeSay.Setup.Tests
{
	[TestFixture]
	public class InterfaceLanguageControl
	{
		private AdminWindow _window;

		[SetUp]
		public void Setup()
		{
			Palaso.Reporting.ErrorReport.IsOkToInteractWithUser = false;

 //           _window.OpenProject(BasilProject.GetPretendProjectDirectory());


		}

		private void GoToUILanguageTab()
		{
			ToolStrip toolstrip = (ToolStrip)_window.Controls.Find("_areasToolStrip", true)[0];
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
				WeSay.Foundation.Tests.TestUtilities.DeleteFolderThatMayBeInUse(path);
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
				WeSay.Foundation.Tests.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test]
		public void CycleWithoutGoingToUITab_DoesntLooseLanguageSetting() // Regression for WS599
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				NUnit.Extensions.Forms.ComboBoxTester t = CreateNewAndGetLanguageCombo(path);
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
				WeSay.Foundation.Tests.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test]
		public void RevertingToEnglish_Works()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				NUnit.Extensions.Forms.ComboBoxTester t = CreateNewAndGetLanguageCombo(path);
				//select the last one
				t.Properties.SelectedItem = t.Properties.Items[t.Properties.Items.Count - 1];
				string previouslySelected = t.Properties.SelectedItem.ToString();
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
				WeSay.Foundation.Tests.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		private object FindDefaultEnglishItem(ComboBox combo)
		{
			foreach (object o in combo.Items)
			{
				if(o.ToString() == "English (Default)")
				{
					return o;
				}
			}
				return null;
		}

		private void CloseApp()
		{
			if(_window !=null)
			{
				_window.Close();
				_window.Dispose();
				_window = null;
			}
		}


		private ComboBoxTester CreateNewAndGetLanguageCombo(string path)
		{
			_window = new AdminWindow(new string[] { });
			_window.Show();
			_window.CreateAndOpenProject(path);
			GoToUILanguageTab();
			return new ComboBoxTester("_languageCombo", _window);
		}

		private ComboBoxTester GoToTabAndGetLanguageCombo()
		{
			GoToUILanguageTab();
			ComboBoxTester t = new ComboBoxTester("_languageCombo", _window);
			return t;
		}

		private void OpenExisting(string path)
		{
			_window = new AdminWindow(new string[] { });
			_window.Show();
			_window = new AdminWindow(new string[] { });
			_window.Show();
			_window.OpenProject(path);
		}

	}
}
