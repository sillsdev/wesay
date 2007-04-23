using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using MultithreadProgress;
using WeSay.Data;
using WeSay.Foundation;
using WeSay.Foundation.Progress;
using WeSay.Project;
using WeSay.Setup.Properties;
using WeSay.UI;

namespace WeSay.Setup
{
	public partial class AdminWindow : Form
	{
		private WelcomeControl _welcomePage = new WelcomeControl();
		private ProjectTabs _projectTabs;
		private WeSayWordsProject _project;
		ProgressDialogHandler _progressHandler;
		private ProgressDialogProgressState _progressState;
		private string _progressLog;

 public AdminWindow(string[] args)
		{
			InitializeComponent();

			this.Project = null;

//            if (this.DesignMode)
//                return;
//
			InstallWelcomePage();
			UpdateEnabledStates();

			if (args.Length > 0)
			{
				OpenProject(args[0].Trim(new char[] { '"' }));
			}


			if (!this.DesignMode)
			{
				UpdateWindowCaption();
			}
		}

		private WeSayWordsProject Project
		{
			get { return this._project; }
			set
			{
				this._project = value;
				UpdateEnabledStates();
			}
		}

		// This delegate enables asynchronous calls for setting
		// the properties from another thread.
		delegate void UpdateStuffCallback();

		private void UpdateEnabledStates()
		{
			if (this.menuStrip1.InvokeRequired)
			{
				UpdateStuffCallback d = new UpdateStuffCallback(UpdateEnabledStates);
				this.Invoke(d, new object[] {});
			}
			else
			{
				this.openThisProjectInWeSayToolStripMenuItem.Enabled = (_project != null) && (_progressHandler == null);
			 //   exportToLIFTXmlToolStripMenuItem.Enabled = (_project != null) && (_progressHandler == null);
			   // importFromLIFTXMLToolStripMenuItem.Enabled = (_project != null) && (_progressHandler == null);
			}

		}

		void OnOpenProject(object sender, EventArgs e)
		{
//            string selectedPath = sender as string;
//            if (selectedPath == null)
//            {
				string initialDirectory = Settings.Default.LastProjectPath;

				if (initialDirectory == null || initialDirectory == "")
				{
					initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				}
//                this._chooseProjectLocationDialog.SelectedPath = s;

//                if (DialogResult.OK != this._chooseProjectLocationDialog.ShowDialog())
//                    return;
//
//                selectedPath = this._chooseProjectLocationDialog.SelectedPath;
//            }

			System.Windows.Forms.OpenFileDialog dlg = new OpenFileDialog();
			dlg.DefaultExt = ".WeSayConfig";
			dlg.Filter = "WeSay Configuration File (*.WeSayConfig)|*.WeSayConfig";
			dlg.Multiselect = false;
			dlg.InitialDirectory = initialDirectory;
			if (DialogResult.OK != dlg.ShowDialog(this))
				return;


			if (WeSayWordsProject.IsValidProjectDirectory(System.IO.Directory.GetParent(dlg.FileName).FullName))
			{
				OpenProject(dlg.FileName);
			}

			Settings.Default.LastProjectPath = dlg.FileName;

//            if (WeSayWordsProject.IsValidProjectDirectory(selectedPath))
//            {
//                OpenProject(selectedPath);
//            }
//                //allow them to click the "wesay" subdirectory, since that's a reasonable thing to try
//            else if (WeSayWordsProject.IsValidProjectDirectory(System.IO.Directory.GetParent(selectedPath).FullName))
//            {
//                OpenProject(System.IO.Directory.GetParent(selectedPath).FullName);
//            }
//
//            else
//            {
//                Reporting.ErrorReporter.ReportNonFatalMessage("That directory does not appear to be a valid WeSay or Basil Project directory.");
//            }
		}




		private void OnCreateProject(object sender, EventArgs e)
		{
			NewProject dlg = new NewProject();
			if (DialogResult.OK != dlg.ShowDialog())
				return;
			CreateAndOpenProject(dlg.SelectedPath);
		}

		public void CreateAndOpenProject(string path)
		{
			CreateNewProject(path);
			OpenProject(path);
			_project.Save();
		}

		private void CreateNewProject(string path)
		{
			WeSayWordsProject p;

			try
			{
				p = new WeSayWordsProject();
				p.CreateEmptyProjectFiles(path);

//                Db4oDataSource d = new Db4oDataSource(p.PathToDb4oLexicalModelDB);
//                d.Data.Commit();
//                d.Data.Close();
			}
			catch (Exception e)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("WeSay was not able to create a project there. \r\n" + e.Message);
				return;
			}

			if (this.Project != null)
			{
				this.Project.Dispose();
			}
			this.Project = p;
			SetupProjectControls();
		}

		public void OpenProject(string path)
		{
			//System.Configuration.ConfigurationManager.AppSettings["LastProjectPath"] = path;

			//strip off any trailing '\'
			if (path[path.Length - 1] == Path.DirectorySeparatorChar
				|| path[path.Length - 1] == Path.AltDirectorySeparatorChar)
			{
				path = path.Substring(0, path.Length - 1);
			}

			try
			{
				this.Project = new WeSayWordsProject();

				//just open the accompanying lift file.
				path = path.Replace(".WeSayConfig", ".lift");

				if (path.Contains(".lift"))
				{
					this.Project.LoadFromLiftLexiconPath(path);
				}
//                else if (path.Contains(".WeSayConfig"))
//                {
//                    this.Project.LoadFromConfigFilePath(path);
//                }
				else if (Directory.Exists(path))
				{
					this.Project.LoadFromProjectDirectoryPath(path);
				}
				else
				{
					throw new ApplicationException(path + " is not named as a .lift file or .WeSayConfig file.");
				}
			}
			catch (Exception e)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("WeSay was not able to open that project. \r\n" + e.Message);
				return;
			}

			SetupProjectControls();
			Settings.Default.LastProjectPath = path;
		}

		private void SetupProjectControls()
		{
		   // try
		   // {
			UpdateWindowCaption();
			RemoveExistingControls();
				InstallProjectsControls();
			//}
//            catch (Exception e)
//            {
//                MessageBox.Show("WeSay was not able to display that project. \r\n"+e.Message);
//            }
		}

		private void UpdateWindowCaption()
		{
			string projectName="";
			if (this.Project != null)
			{
				projectName = this.Project.Name;
			}
			this.Text = "WeSay Configuration Tool: " + projectName+ "   "+ Reporting.ErrorReporter.UserFriendlyVersionString;
		}


		private void InstallWelcomePage()
		{
			this.Controls.Add(this._welcomePage);
			this._welcomePage.BringToFront();
			this._welcomePage.Dock = DockStyle.Fill;
			this._welcomePage.NewProjectClicked += new EventHandler(OnCreateProject);
			this._welcomePage.OpenProjectClicked += new EventHandler(OnOpenProject);
		}
		private void InstallProjectsControls()
		{
			this._projectTabs = new ProjectTabs();
			this.Controls.Add(this._projectTabs);
			this._projectTabs.BringToFront();
			this._projectTabs.Dock = DockStyle.Fill;
		}

		private void RemoveExistingControls()
		{
			if (this._welcomePage != null)
			{
				this.Controls.Remove(this._welcomePage);
				this._welcomePage.Dispose();
				this._welcomePage = null;
			}
			if (this._projectTabs != null)
			{
				this.Controls.Remove(this._projectTabs);
				this._projectTabs.Dispose();
				this._projectTabs = null;
			}
		}

		private void AdminWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_projectTabs != null)
			{
				_projectTabs.Dispose();
			}
		}

		private void AdminWindow_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (_progressHandler != null)
			{
				// Then Close() the dialog, to force a cancel
				// Note, we don't use ForceClose() because we
				// want to invoke the cancel behaviour
				_progressHandler.CloseByCancellingThenCloseParent();
				e.Cancel = true;
				return;
			}

			try
			{
				if (this.Project != null)
				{
					this.Project.Save();
				}
				Settings.Default.Save();
			}
			catch (Exception error)
			{
				//would make it impossible to quit. e.Cancel = true;
				Reporting.ErrorReporter.ReportNonFatalMessage(error.Message);
			}
		}

		private void OnAboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutBox().ShowDialog();
		}


		private void OnExportToLiftXmlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			if (!File.Exists(_project.PathToDb4oLexicalModelDB))
			{
				Reporting.ErrorReporter.ReportNonFatalMessage(
					string.Format(
						"Sorry, {0} cannot find a file which is necessary to perform the export on this project ({1})",
						Application.ProductName, _project.PathToDb4oLexicalModelDB));
				return;
			}

//            OpenFileDialog openDialog = new OpenFileDialog();
//            openDialog.Title="Choose the Words file to convert to LIFT";
//            openDialog.FileName = WeSayWordsProject.Project.PathToLexicalModelDB;
//            openDialog.Filter = "WeSay Words(*.words)|*.words";
//            if (openDialog.ShowDialog() != DialogResult.OK)
//            {
//                return;
//            }

			SaveFileDialog saveDialog = new SaveFileDialog();
			if (Settings.Default.LastLiftExportPath == String.Empty || !Directory.Exists(Settings.Default.LastLiftExportPath))
			{
				saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
			else
			{
				saveDialog.InitialDirectory = Settings.Default.LastLiftExportPath;
			}
			saveDialog.Title="Save LIFT file as";
			saveDialog.Filter = "LIFT XML (*.xml)|*.xml";
			saveDialog.FileName=_project.Name+".lift.xml";
			if (saveDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			Settings.Default.LastLiftExportPath = Path.GetDirectoryName(saveDialog.FileName);

			RunCommand(new ExportLIFTCommand(saveDialog.FileName, _project.PathToDb4oLexicalModelDB));
		}


		private void RunCommand(BasicCommand command)
		{
			_progressLog = "";
			_progressHandler = new ProgressDialogHandler(this, command);
			_progressHandler.Finished += new EventHandler(OnProgressHandler_Finished);
			_progressState = new WeSay.UI.ProgressDialogProgressState(_progressHandler);
			_progressState.Log += new EventHandler<ProgressState.LogEvent>(OnProgressState_Log);
			UpdateEnabledStates();
			command.BeginInvoke(_progressState);
		}

		void OnProgressState_Log(object sender, ProgressState.LogEvent e)
		{
			_progressLog += e.message + "\r\n";
		}

		void OnProgressHandler_Finished(object sender, EventArgs e)
		{
			_progressHandler = null;
			UpdateEnabledStates();
			if (_progressState.State == ProgressState.StateValue.StoppedWithError)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("WeSay ran into a problem.\r\n"+_progressLog);
			}
		}


		private void OnImportFromLiftXml(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Title = "Choose the LIFT xml file to convert to a WeSay Words file";

			if (Settings.Default.LastLiftImportPath == String.Empty || !Directory.Exists(Settings.Default.LastLiftImportPath))
			{
				openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
			else
			{
				openDialog.InitialDirectory = Settings.Default.LastLiftImportPath;
			}

			openDialog.Filter = "LIFT XML (*.xml;*.lift)|*.xml;*.lift";
			if (openDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			Settings.Default.LastLiftImportPath = Path.GetDirectoryName(openDialog.FileName);

//            SaveFileDialog saveDialog = new SaveFileDialog();
//            saveDialog.Title = "Save WeSay Words file as";
//            saveDialog.Filter = "WeSay Words(*.words)|*.words";
//            saveDialog.InitialDirectory = WeSayWordsProject.Project.PathToLexicalModelDB;
//            if (saveDialog.ShowDialog() != DialogResult.OK)
//            {
//                return;
//            }
//            RunCommand(new ImportLIFTCommand(saveDialog.FileName, openDialog.FileName));
			RunCommand(new ImportLIFTCommand(openDialog.FileName));
		}

		private void OnOpenThisProjectInWeSay(object sender, EventArgs e)
		{
			this._project.Save();//want the client to see the latest
			string dir = Directory.GetParent(Application.ExecutablePath).FullName;
			ProcessStartInfo startInfo =
				new ProcessStartInfo(Path.Combine(dir, "WeSay.App.exe"),
														string.Format("\"{0}\"", _project.PathToLiftFile));
			Process.Start(startInfo);
		}



		void OnExit_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}
	}
}
