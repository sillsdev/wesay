using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Xml.XPath;
using NUnit.Framework;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.Project;
using WeSay.Project.Tests;
using WeSay.TestUtilities;

namespace WeSay.ConfigTool.Tests
{
	[Category("SkipOnTeamCity")]
	[TestFixture, Apartment(ApartmentState.STA)]
	public class AdminWindowTests
	{
		private ConfigurationWindow _window;
		private string _projectFolder;

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
			_window = new ConfigurationWindow(new string[] {});
			_window.DisableBackupAndChorusStuffForTests();
			_window.Show();

			_projectFolder = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
		}

		[TearDown]
		public void TearDown()
		{
			SIL.Windows.Forms.Keyboarding.KeyboardController.Shutdown();
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
			File.WriteAllLines(p, File.ReadAllLines(p)); //touches file leaving it in original state
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
			foreach (string p in paths)
			{
				if (!File.Exists(p))
				{
					Assert.Fail("Did not create " + p);
				}
			}
		}

		[Test]
		public void TryingToOpenNonExistantProjectDoesntCrash()
		{
			Assert.Throws<ErrorReport.ProblemNotificationSentToUserException>(
				() => _window.OnOpenProject(@"C:\notreallythere.WeSayConfig", false)
			);
		}

		[Test]
		public void CreateNewProjectThenOpen()
		{
			_window.CreateAndOpenProject(_projectFolder, "th", "Thai");
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

		[Test]
		public void RunAndExitWithoutOpening()
		{
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
	[Category("SkipOnTeamCity")]
	[TestFixture]
	public class MoreAdminWindowTests
	{
		[OneTimeSetUp]
		public void OneTimeSetUp()
		{
			Sldr.Initialize(true);
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			Sldr.Cleanup();
		}

		[Test, Apartment(ApartmentState.STA)]
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
