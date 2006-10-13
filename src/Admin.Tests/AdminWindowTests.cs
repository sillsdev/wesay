using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using WeSay.Project;

namespace WeSay.Admin.Tests
{
	[TestFixture]
	public class AdminWindowTests : NUnitFormTest
	{
		private AdminWindow _window;
		private string _projectFolder;
		private FormTester _mainWindowTester;

		public override void Setup()
		{
			base.Setup();
			_window = new AdminWindow();
			_window.Show();
			string name = new Finder().Name(_window);
			_mainWindowTester = new FormTester(name);

			this._projectFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

		}

		private void CreateNewProject()
		{
			this._window.CreateNewProject(this._projectFolder);
		}


		public override void TearDown()
		{
			base.TearDown();
			if (Directory.Exists(_projectFolder))
			{
				Directory.Delete(_projectFolder, true);
			}
	   }

		[Test]
		public void ProjectFilesTouched()
		{
			_window.OpenProject( BasilProject.GetPretendProjectDirectory());
			string p = BasilProject.Project.PathToWritingSystemPrefs;
			DateTime before = File.GetLastWriteTime(p);
			_mainWindowTester.Close();
			DateTime after = File.GetLastWriteTime(p);
			Assert.AreNotEqual(before, after);
		}

		//stupid nunitforms will freak 'cause window was closed
		[Test]//, ExpectedException(typeof(FormsTestAssertionException))]
		public void AfterCreateProjectAndQuitFilesExist()
		{
			List<string> paths = new List<string>();
			CreateNewProject();
			paths.Add(BasilProject.Project.PathToWritingSystemPrefs);
			paths.Add(WeSayWordsProject.Project.PathToProjectTaskInventory);
			//paths.Add(WeSayWordsProject.Project.PathToLexicalModelDB);
			_mainWindowTester.Close();
			foreach (string p in paths)
			{
				if (!File.Exists(p))
				{
					Assert.Fail("Did not create "+p);
				}
			}
		}

		[Test]
		public void WalkTabsAfterOpeningPretendProject()
		{
			  _window.OpenProject(BasilProject.GetPretendProjectDirectory());
		  //create or overwrite the tasks with our stored resource
			StreamWriter writer = File.CreateText(WeSayWordsProject.Project.PathToProjectTaskInventory);
			writer.Write(TestResources.tasks);
			writer.Close();

			WalkTopLevelTabs();
		}

		[Test]
		public void WalkTabsAfterCreateNewProject()
		{
			CreateNewProject();
			WalkTopLevelTabs();
		}
		[Test]
		public void CreateNewProjectThenOpen()
		{
			_window.CreateNewProject(_projectFolder);
			_window.OpenProject(_projectFolder);
		}

		[Test]
		public void NewProjectShowsTasks()
		{
			CreateProjectAndGoToTaskControl();
			CheckedListBoxTester c = new CheckedListBoxTester("_taskList");
			Assert.Greater(c.Properties.Items.Count, 0);
		}

		[Test]
		public void NewProjectShowSomeDefaultTasks()
		{
			CreateProjectAndGoToTaskControl();
			CheckedListBoxTester c = new CheckedListBoxTester("_taskList");
			Assert.Greater(c.Properties.CheckedItems.Count, 0);
		}

		private void CreateProjectAndGoToTaskControl()
		{
			this._window.CreateNewProject(this._projectFolder);
			this._window.OpenProject(this._projectFolder);

			GotoProjectTab("_tasksPage");
		}

		[Test]
		public void NewProjectShowsOneWritingSystem()
		{
			_window.CreateNewProject(_projectFolder);
			_window.OpenProject(_projectFolder);

			GotoProjectTab("_writingSystemPage");
			ListBoxTester c = new ListBoxTester("_wsListBox");
			Assert.AreEqual(1, c.Properties.Items.Count);
		}

		private static void GotoProjectTab(string projectTabName)
		{
			TabControlTester t = new TabControlTester("_projectTabControl");

			t.Properties.SelectedTab = t.Properties.TabPages[projectTabName];

			Assert.IsNotNull(t.Properties.SelectedTab, "Couldn't find "+projectTabName);
		}

		private void WalkTopLevelTabs()
		{
//            for(int i = 0; i<10000;i++)
//            {
//                Application.DoEvents();
//            }
			TabControlTester t = new TabControlTester("_projectTabControl");
			foreach(TabPage p in t.Properties.TabPages)
			{
				t.Properties.SelectedTab = p;
			}
		}

		[Test]
		public void RunAndExitWithoutOpening()
		{
			_mainWindowTester.Close();
		}


	}
}
