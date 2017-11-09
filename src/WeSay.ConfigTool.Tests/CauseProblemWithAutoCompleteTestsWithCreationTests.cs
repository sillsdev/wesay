using System.IO;
using System.Windows.Forms;
using NUnit.Framework;
using Palaso.Reporting;
using WeSay.Project;

namespace WeSay.ConfigTool.Tests
{
	[TestFixture]
	public class CauseProblemWithAutoCompleteTestsWithCreationTests
	{
		private ConfigurationWindow _window;

		[SetUp]
		public void Setup()
		{
			ErrorReport.IsOkToInteractWithUser = false;
			_window = new ConfigurationWindow(new string[] {});
			_window.Show();
		}

		[TearDown]
		public void TearDown()
		{
			_window.Close();
			_window.Dispose();
			if (BasilProject.IsInitialized)
			{
				WeSayWordsProject.Project.Dispose();
			}
		}

		[Test]
		[Ignore("Mysteriously Causes AutoCompleteWithCreationBoxTestsToFail")]
		public void WalkTabsAfterOpeningPretendProject()
		{
			_window.OpenProject(BasilProject.GetPretendProjectDirectory(), false);
			File.Copy(
					Path.Combine(WeSayWordsProject.Project.ApplicationTestDirectory,
								 "PRETEND.WeSayConfig"),
					WeSayWordsProject.Project.PathToConfigFile,
					true);

			ToolStrip toolstrip = (ToolStrip) _window.Controls.Find("_areasToolStrip", true)[0];
			foreach (ToolStripButton button in toolstrip.Items)
			{
				button.PerformClick();
			}
		}
	}
}