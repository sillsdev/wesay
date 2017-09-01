using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using System.Xml.XPath;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.Project;
using WeSay.Project.Tests;
using WeSay.TestUtilities;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture, RequiresSTA]
	public class AdminWindowTests: NUnitFormTest
	{
		private ConfigurationWindow _window;
		private string _projectFolder;
		private FormTester _mainWindowTester;

		public override void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			base.Setup();
			SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();
			_window = new ConfigurationWindow(new string[] {});
			_window.DisableBackupAndChorusStuffForTests();
			_window.Show();
			_mainWindowTester = new FormTester(_window.Name, _window);
			Sldr.Initialize(true);

			_projectFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		}

		public override void TearDown()
		{
			_mainWindowTester.Close();
			SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
			base.TearDown();
			if (BasilProject.IsInitialized)
			{
				WeSayWordsProject.Project.Dispose();
			}

			SIL.TestUtilities.TestUtilities.DeleteFolderThatMayBeInUse(_projectFolder);
		}

		[Test]
		public void ProjectFilesTouched()
		{
			_window.OpenProject(BasilProject.GetPretendProjectDirectory());
			string p = WeSayWordsProject.Project.PathToConfigFile;
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
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");
			paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "th.ldml"));
			paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "en.ldml"));
			//paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "es.ldml"));
			//paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "fr.ldml"));
			//paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "id.ldml"));
			//paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "th.ldml"));
			//paths.Add(Path.Combine(BasilProject.GetPathToLdmlWritingSystemsFolder(_projectFolder), "tpi.ldml"));
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
		public void TryingToOpenNonExistantProjectDoesntCrash()
		{
			Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
				() => _window.OnOpenProject(@"C:\notreallythere.WeSayConfig")
			);
		}

		[Test]
		[Ignore("Mysteriously Causes AutoCompleteWithCreationBoxTestsToFail")]
		public void WalkTabsAfterCreateNewProject()
		{
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");
			WalkTopLevelTabs();

		}

		[Test]
		public void CreateNewProjectThenOpen()
		{
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");
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
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");
			string path = WeSayWordsProject.Project.PathToConfigFile;
			XPathDocument doc = new XPathDocument(path);
			Assert.IsNotNull(doc.CreateNavigator().SelectSingleNode("configuration[@version]"));
			Assert.IsNotNull(doc.CreateNavigator().SelectSingleNode("configuration/tasks"));
			Assert.IsNotNull(doc.CreateNavigator().SelectSingleNode("configuration/components"));
		}

		private void CreateProjectAndGoToTaskControl()
		{
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");

			ClickToolStripButton("_tasksButton");
			//            GotoProjectTab("_tasksPage");
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
			WeSayProjectTestHelper.InitializeForTests(); // for Teardown
		}

		[Test]
		public void ExistingProjectGetsNewTasks()
		{
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");
		}

		// WS-34217: Regression test to ensure that English can be selected at project creation
		[Test]
		public void CreateAndOpenProject_EnglishAsVernacular_DoesNotCrash()
		{
			Assert.That(() => _window.CreateAndOpenProject(_projectFolder, "en", "English"), Throws.Nothing);
		}
	}

	/* these are more modern, without use of static, "pretend" project, or the big setup/teardown of the old style */

	[TestFixture]
	public class MoreAdminWindowTests
	{

		[Test, RequiresSTA]
		public void OpenProject_OpenedWithDirNameWhichDoesNotMatchProjectName_Opens()
		{
			using (var projectDir = new ProjectDirectorySetupForTesting(""))
			{
				SIL.Windows.Forms.Keyboarding.KeyboardController.Initialize();
				using (var window = new ConfigurationWindow(new string[] { }))
				{
					Assert.AreNotEqual(
						Path.GetFileNameWithoutExtension(projectDir.PathToLiftFile),
						Path.GetFileName(projectDir.PathToDirectory));

					window.DisableBackupAndChorusStuffForTests();
					window.Show();
					Assert.IsTrue(window.OpenProject(projectDir.PathToDirectory));
				}
				SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
			}
		}
	}
}
