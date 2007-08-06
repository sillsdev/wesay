using System.IO;
using NUnit.Framework;
using NUnit.Extensions.Forms;
using WeSay.Setup;

namespace WeSay.Admin.Tests
{
	[TestFixture]
	public class TestProjectMenu//: NUnitFormTest
	{
		private AdminWindow _window;

		public TestProjectMenu()
		{
		}

		[SetUp]
		public void Setup()
		{
			Palaso.Reporting.ErrorReport.OkToInteractWithUser = false;
			_window = new AdminWindow(new string[] { });
			_window.Show();
		 }



		[Test]
		public void ProjectIsCreated()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
			try
			{
				_window.CreateAndOpenProject(path);
				_window.Close();
				_window.Dispose();
			}
			finally
			{
				WeSay.Foundation.Tests.TestUtilities.DeleteFolderThatMayBeInUse(path);
			}
		}

		[Test, Ignore("Haven't got the ability to find controls inside the filedialog yet")]
		public void TestUsingOpenProject()
		{
			FormTester AdminWindow = new FormTester("WeSay Admin");

			ToolStripMenuItemTester projectToolStripMenuItem = new ToolStripMenuItemTester("projectToolStripMenuItem");
			ToolStripMenuItemTester newProjectToolStripMenuItem = new ToolStripMenuItemTester("newProjectToolStripMenuItem");


			projectToolStripMenuItem.Click();
			//       ExpectModal("Browse For Folder", "ClickOKInFileDialog", true);

			newProjectToolStripMenuItem.Click();

			AdminWindow.Close();
		}

		[Test,Ignore("couldn't get test to word")]
		public void TestOpenProjectInWeSayClient()
		{
			string path = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(Path.GetRandomFileName  ()));
			try
			{
				_window.CreateAndOpenProject(path);

				ToolStripMenuItemTester projectToolStripMenuItem = new ToolStripMenuItemTester("projectToolStripMenuItem");
				ToolStripMenuItemTester launchProjectToolStripMenuItem =
					new ToolStripMenuItemTester("openThisProjectInWeSayToolStripMenuItem");
				projectToolStripMenuItem.Click();
				launchProjectToolStripMenuItem.Click();
				_window.Close();

			}
			finally
			{
				Directory.Delete(path,true);
			}
		}

		//		[Test]
		//		public void RememberOkBox()
		//		{
		//			string name="X";
		//			MessageBoxEx msgBox = MessageBoxExManager.CreateMessageBox(name);
		//			msgBox.Caption = name;
		//			msgBox.Text = "Blah blah blah?";
		//
		//			msgBox.AddButtons(MessageBoxButtons.YesNo);
		//
		//			msgBox.SaveResponseText = "Don't ask me again";
		//			msgBox.UseSavedResponse = false;
		//			msgBox.AllowSaveResponse  = true;
		//
		//			//click the yes button when the dialog comes up
		//			ExpectModal(name, "ConfirmModalByYesAndRemember",true);
		//
		//			Assert.AreEqual("Yes", 	msgBox.Show());
		//
		//			ExpectModal(name, "DoNothing",false /*don't expect it, because it should use our saved response*/);
		//			msgBox.UseSavedResponse = true;
		//			Assert.AreEqual("Yes", 	msgBox.Show());
		//
		//		}

		//		public void DoNothing()
		//		{
		//		}
		//
		public void ConfirmModalByYes()
		{
			ButtonTester t = new ButtonTester("Yes");
			t.Click();
		}

		public void CancelModal()
		{
			NUnit.Extensions.Forms.FileDialogTester x = new FileDialogTester("Browse For Folder");
			x.ClickCancel();
			ButtonTester t = new NUnit.Extensions.Forms.ButtonTester("Cancel");
			t.Click();
		}

		public void ClickOKInFileDialog()
		{
			ButtonTester t = new NUnit.Extensions.Forms.ButtonTester("OK");
			t.Click();
		}

		//		public void ConfirmModalByYesAndRemember()
		//		{
		//			new CheckBoxTester("chbSaveResponse").Check(true);
		//			new ButtonTester("Yes").Click();
		//		}
	}
}
