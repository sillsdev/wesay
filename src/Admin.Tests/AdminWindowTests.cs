using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using WeSay.UI;

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
		//stupid nunitforms will freak 'cause window was closed
		[Test, ExpectedException(typeof(FormsTestAssertionException))]
		public void ProjectFilesTouched()
		{
			_window.OpenProject( BasilProject.GetPretendProjectDirectory());
			string p = BasilProject.Project.PathToWritingSystemPrefs;
			DateTime before = File.GetLastWriteTime(p);
			_mainWindowTester.Close();
			MessageBox.Show("hello");
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
			WalkTopLevelTabs();
		}

		[Test]
		public void WalkTabsAfterCreateNewProject()
		{
			CreateNewProject();
			WalkTopLevelTabs();
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
