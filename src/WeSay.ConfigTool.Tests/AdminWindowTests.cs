using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.XPath;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Palaso.Reporting;
using TestUtilities;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture]
	public class AdminWindowTests: NUnitFormTest
	{
		private ConfigurationWindow _window;
		private string _projectFolder;
		private FormTester _mainWindowTester;

		public override void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			base.Setup();
			_window = new ConfigurationWindow(new string[] {});
			_window.Show();
			_mainWindowTester = new FormTester(_window.Name, _window);

			_projectFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		}

		public override void TearDown()
		{
			base.TearDown();
			if (BasilProject.IsInitialized)
			{
				WeSayWordsProject.Project.Dispose();
			}

			TestUtilities.FileUtilities.DeleteFolderThatMayBeInUse(_projectFolder);
		}

		[Test]
		public void ProjectFilesTouched()
		{
			_window.OpenProject(BasilProject.GetPretendProjectDirectory());
			string p = BasilProject.Project.PathToWritingSystemPrefs;
			DateTime before = File.GetLastWriteTime(p);
			_mainWindowTester.Close();
			DateTime after = File.GetLastWriteTime(p);
			Assert.AreNotEqual(before, after);
		}

		//stupid nunitforms will freak 'cause window was closed
		[Test] //, ExpectedException(typeof(FormsTestAssertionException))]
		public void AfterCreateProjectAndQuitFilesExist()
		{
			List<string> paths = new List<string>();
			_window.CreateAndOpenProject(_projectFolder);
			paths.Add(BasilProject.Project.PathToWritingSystemPrefs);
			paths.Add(WeSayWordsProject.Project.PathToConfigFile);
			//paths.Add(WeSayWordsProject.Project.PathToRepository);
			_mainWindowTester.Close();
			foreach (string p in paths)
			{
				if (!File.Exists(p))
				{
					Assert.Fail("Did not create " + p);
				}
			}
		}

		[Test]
		[Ignore("Mysteriously Causes AutoCompleteWithCreationBoxTestsToFail")]
		public void WalkTabsAfterOpeningPretendProject()
		{
			_window.OpenProject(BasilProject.GetPretendProjectDirectory());
			//create or overwrite the tasks with our stored resource
			//            File.Delete(WeSayWordsProject.Project.PathToProjectTaskInventory);
			//            StreamWriter writer = File.CreateText(WeSayWordsProject.Project.PathToProjectTaskInventory);
			//            writer.Write(TestResources.tasks);
			//            writer.Close();
			File.Copy(
					Path.Combine(WeSayWordsProject.Project.ApplicationTestDirectory,
								 "PRETEND.WeSayConfig"),
					WeSayWordsProject.Project.PathToConfigFile,
					true);
			WalkTopLevelTabs();
		}

		[Test]
		[ExpectedException(typeof (ErrorReport.NonFatalMessageSentToUserException))]
		public void TryingToOpenNonExistantProjectDoesntCrash()
		{
			_window.OnOpenProject(@"C:\notreallythere.WeSayConfig", null);
		}

		[Test]
		[Ignore("Mysteriously Causes AutoCompleteWithCreationBoxTestsToFail")]
		public void WalkTabsAfterCreateNewProject()
		{
			_window.CreateAndOpenProject(_projectFolder);
			WalkTopLevelTabs();
		}

		[Test]
		public void CreateNewProjectThenOpen()
		{
			_window.CreateAndOpenProject(_projectFolder);
		}

		[Test]
		public void NewProjectShowsTasks()
		{
			CreateProjectAndGoToTaskControl();
			CheckedListBoxTester c = new CheckedListBoxTester("_taskList", _window);
			Assert.Greater(c.Properties.Items.Count, 0);
		}

		[Test]
		public void NewProjectShowSomeDefaultTasks()
		{
			CreateProjectAndGoToTaskControl();
			CheckedListBoxTester c = new CheckedListBoxTester("_taskList", _window);
			Assert.Greater(c.Properties.CheckedItems.Count, 0);
		}

		[Test]
		public void NewProjectHasValidStructure()
		{
			_window.CreateAndOpenProject(_projectFolder);
			string path = WeSayWordsProject.Project.PathToConfigFile;
			XPathDocument doc = new XPathDocument(path);
			Assert.IsNotNull(doc.CreateNavigator().SelectSingleNode("configuration[@version]"));
			Assert.IsNotNull(doc.CreateNavigator().SelectSingleNode("configuration/tasks"));
			Assert.IsNotNull(doc.CreateNavigator().SelectSingleNode("configuration/components"));
		}

		private void CreateProjectAndGoToTaskControl()
		{
			_window.CreateAndOpenProject(_projectFolder);

			ClickToolStripButton("_tasksButton");
			//            GotoProjectTab("_tasksPage");
		}

		[Test]
		public void NewProjectShowsMultipleWritingSystems()
		{
			_window.CreateAndOpenProject(_projectFolder);

			ClickToolStripButton("_writingSystemButton");
			//GotoProjectTab("_writingSystemPage");
			ListBoxTester c = new ListBoxTester("_wsListBox", _window);
			Assert.Greater(c.Properties.Items.Count, 2);
		}

		//        private static void GotoProjectTab(string projectTabName)
		//        {
		//            TabControlTester t = new TabControlTester("_projectTabControl");
		//
		//            foreach (TabPage page in t.Properties.TabPages)
		//            {
		//                if (page.Name == projectTabName)
		//                {
		//                    t.Properties.SelectedTab = page;
		//                    break;
		//                }
		//            }
		////            t.Properties.SelectedTab = t.Properties.TabPages[projectTabName];
		//
		//            Assert.IsNotNull(t.Properties.SelectedTab, "Couldn't find "+projectTabName);
		//        }

		private void WalkTopLevelTabs()
		{
			//            for(int i = 0; i<10000;i++)
			//            {
			//                Application.DoEvents();
			//            }
			ControlFinder f = new ControlFinder("_areasToolStrip");
			ToolStrip toolstrip = (ToolStrip) f.Find();
			foreach (ToolStripButton button in toolstrip.Items)
			{
				string name = button.Name;
				ClickToolStripButton(name);
			}
		}

		private void ClickToolStripButton(string name)
		{
			ToolStripButtonTester tester = new ToolStripButtonTester(name, _window);
			tester.Click();
		}

		[Test]
		public void RunAndExitWithoutOpening()
		{
			_mainWindowTester.Close();
			WeSayWordsProject.InitializeForTests(); // for Teardown
		}

		[Test]
		public void ExistingProjectGetsNewTasks()
		{
			_window.CreateAndOpenProject(_projectFolder);
		}
	}
}