using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Foundation.Progress;
using WeSay.Project;
using WeSay.Setup.Properties;
using WeSay.UI;

namespace WeSay.Setup
{
	public partial class AdminWindow : Form
	{
		private WelcomeControl _welcomePage;
		private SettingsControl _projectSettingsControl;
		private WeSayWordsProject _project;
		private ProgressDialogHandler _progressHandler;
		private ProgressDialogProgressState _progressState;
		private string _progressLog;

		public AdminWindow(string[] args)
		{
			InitializeComponent();

			Project = null;

//            if (this.DesignMode)
//                return;
//
			InstallWelcomePage();
			UpdateEnabledStates();

			if (args.Length > 0)
			{
				OpenProject(args[0].Trim(new char[] {'"'}));
			}

			if (!DesignMode)
			{
				UpdateWindowCaption();
			}
		}

		private WeSayWordsProject Project
		{
			get { return _project; }
			set
			{
				_project = value;
				UpdateEnabledStates();
			}
		}

		// This delegate enables asynchronous calls for setting
		// the properties from another thread.
		private delegate void UpdateStuffCallback();

		private void UpdateEnabledStates()
		{
			if (this.toolStrip2.InvokeRequired)
			{
				UpdateStuffCallback d = new UpdateStuffCallback(UpdateEnabledStates);
				Invoke(d, new object[] {});
			}
			else
			{
				openProjectInWeSayToolStripMenuItem.Enabled = (_project != null) && (_progressHandler == null);
			}
		}

		private void OnChooseProject(object sender, EventArgs e)
		{
			string initialDirectory = null;
			if (!String.IsNullOrEmpty(Settings.Default.LastConfigFilePath))
			{
				try
				{
					if (File.Exists(Settings.Default.LastConfigFilePath))
					{
						initialDirectory = Path.GetDirectoryName(Settings.Default.LastConfigFilePath);
					}
				}
				catch
				{
					//swallow
				}
			}

			if (initialDirectory == null || initialDirectory == "")
			{
				initialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}

			OpenFileDialog dlg = new OpenFileDialog();
			dlg.Title = "Open WeSay Project...";
			dlg.DefaultExt = ".WeSayConfig";
			dlg.Filter = "WeSay Configuration File (*.WeSayConfig)|*.WeSayConfig";
			dlg.Multiselect = false;
			dlg.InitialDirectory = initialDirectory;
			if (DialogResult.OK != dlg.ShowDialog(this))
			{
				return;
			}

			OnOpenProject(dlg.FileName, null);
		}

		public void OnOpenProject(object sender, EventArgs e)
		{
			string configFilePath = (string) sender;
			if (!File.Exists(configFilePath))
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(
					"WeSay could not find the file at {0} anymore.  Maybe it was moved or renamed?", configFilePath);
				return;
			}
			string first = Directory.GetParent(configFilePath).FullName;
			if (WeSayWordsProject.IsValidProjectDirectory(Directory.GetParent(first).FullName))
			{
				OpenProject(configFilePath);
			}
			else
			{
				ErrorReport.ReportNonFatalMessage(
						"Sorry, that file does not appear to be located in a valid WeSay Project directory.");
			}
			if (Project != null)
			{
				Settings.Default.LastConfigFilePath = Project.PathToConfigFile;
			}
		}

		private void OnCreateProject(object sender, EventArgs e)
		{
			NewProject dlg = new NewProject();
			if (DialogResult.OK != dlg.ShowDialog())
			{
				return;
			}
			CreateAndOpenProject(dlg.SelectedPath);
		}

		public void CreateAndOpenProject(string path)
		{
			//the "wesay" part my not exist yet
			if (!Directory.GetParent(path).Exists)
			{
				Directory.GetParent(path).Create();
			}

			CreateNewProject(path);
			_project.Save();
			OpenProject(path);
		}

		private void CreateNewProject(string path)
		{
			WeSayWordsProject p;

			try
			{
				p = new WeSayWordsProject();
				p.CreateEmptyProjectFiles(path);
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalMessage("WeSay was not able to create a project there. \r\n" + e.Message);
				return;
			}

			if (Project != null)
			{
				Project.Dispose();
			}
			Project = p;
			SetupProjectControls();
		}

		public void OpenProject(string path)
		{
			//System.Configuration.ConfigurationManager.AppSettings["LastConfigFilePath"] = path;

			//strip off any trailing '\'
			if (path[path.Length - 1] == Path.DirectorySeparatorChar
				|| path[path.Length - 1] == Path.AltDirectorySeparatorChar)
			{
				path = path.Substring(0, path.Length - 1);
			}

			try
			{
				Project = new WeSayWordsProject();

				//just open the accompanying lift file.
				path = path.Replace(".WeSayConfig", ".lift");

				if (path.Contains(".lift"))
				{
					path = Project.UpdateFileStructure(path);
					if (!Project.LoadFromLiftLexiconPath(path))
					{
						Project = null;
						return;
					}
				}
//                else if (path.Contains(".WeSayConfig"))
//                {
//                    this.Project.LoadFromConfigFilePath(path);
//                }
				else if (Directory.Exists(path))
				{
					Project.LoadFromProjectDirectoryPath(path);
				}
				else
				{
					throw new ApplicationException(path + " is not named as a .lift file or .WeSayConfig file.");
				}
			}
			catch (Exception e)
			{
				ErrorReport.ReportNonFatalMessage("WeSay was not able to open that project. \r\n" + e.Message);
				return;
			}

			SetupProjectControls();
			Settings.Default.LastConfigFilePath = Project.PathToConfigFile;
		}

		private void SetupProjectControls()
		{
			UpdateWindowCaption();
			RemoveExistingControls();
			InstallProjectsControls();
		}

		private void UpdateWindowCaption()
		{
			string projectName = "";
			if (Project != null)
			{
				projectName = Project.Name;
			}
			Text = projectName + " - WeSay Configuration Tool";
			_versionToolStripLabel.Text = ErrorReport.UserFriendlyVersionString;
		}

		private void InstallWelcomePage()
		{
			_welcomePage = new WelcomeControl();
			Controls.Add(_welcomePage);
			_welcomePage.BringToFront();
			_welcomePage.Dock = DockStyle.Fill;
			_welcomePage.NewProjectClicked += new EventHandler(OnCreateProject);
			_welcomePage.OpenPreviousProjectClicked += new EventHandler(OnOpenProject);
			_welcomePage.ChooseProjectClicked += new EventHandler(OnChooseProject);
		}

		private void InstallProjectsControls()
		{
			_projectSettingsControl = new SettingsControl();
			Controls.Add(_projectSettingsControl);
			_projectSettingsControl.Dock = DockStyle.Fill;
			_projectSettingsControl.BringToFront();
			_projectSettingsControl.Focus();
		}

		private void RemoveExistingControls()
		{
			if (_welcomePage != null)
			{
				Controls.Remove(_welcomePage);
				_welcomePage.Dispose();
				_welcomePage = null;
			}
			if (_projectSettingsControl != null)
			{
				Controls.Remove(_projectSettingsControl);
				_projectSettingsControl.Dispose();
				_projectSettingsControl = null;
			}
		}

		private void AdminWindow_FormClosed(object sender, FormClosedEventArgs e)
		{
			if (_projectSettingsControl != null)
			{
				_projectSettingsControl.Dispose();
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
				if (Project != null)
				{
					Project.Save();
				}
				Settings.Default.Save();
			}
			catch (Exception error)
			{
				//would make it impossible to quit. e.Cancel = true;
				ErrorReport.ReportNonFatalMessage(error.Message);
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
				ErrorReport.ReportNonFatalMessage(
						string.Format(
								"Sorry, {0} cannot find a file which is necessary to perform the export on this project ({1})",
								Application.ProductName,
								_project.PathToDb4oLexicalModelDB));
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
			if (Settings.Default.LastLiftExportPath == String.Empty ||
				!Directory.Exists(Settings.Default.LastLiftExportPath))
			{
				saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
			else
			{
				saveDialog.InitialDirectory = Settings.Default.LastLiftExportPath;
			}
			saveDialog.Title = "Save LIFT file as";
			saveDialog.Filter = "LIFT XML (*.xml)|*.xml";
			saveDialog.FileName = _project.Name + ".lift.xml";
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
			_progressState = new ProgressDialogProgressState(_progressHandler);
			_progressState.Log += new EventHandler<ProgressState.LogEvent>(OnProgressState_Log);
			UpdateEnabledStates();
			command.BeginInvoke(_progressState);
		}

		private void OnProgressState_Log(object sender, ProgressState.LogEvent e)
		{
			_progressLog += e.message + "\r\n";
		}

		private void OnProgressHandler_Finished(object sender, EventArgs e)
		{
			_progressHandler = null;
			UpdateEnabledStates();
			if (_progressState.State == ProgressState.StateValue.StoppedWithError)
			{
				ErrorReport.ReportNonFatalMessage("WeSay ran into a problem.\r\n" + _progressLog);
			}
		}

//
//        private void OnImportFromLiftXml(object sender, EventArgs e)
//        {
//            OpenFileDialog openDialog = new OpenFileDialog();
//            openDialog.Title = "Choose the LIFT xml file to convert to a WeSay Words file";
//
//            if (Settings.Default.LastLiftImportPath == String.Empty || !Directory.Exists(Settings.Default.LastLiftImportPath))
//            {
//                openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
//            }
//            else
//            {
//                openDialog.InitialDirectory = Settings.Default.LastLiftImportPath;
//            }
//
//            openDialog.Filter = "LIFT XML (*.xml;*.lift)|*.xml;*.lift";
//            if (openDialog.ShowDialog() != DialogResult.OK)
//            {
//                return;
//            }
//            Settings.Default.LastLiftImportPath = Path.GetDirectoryName(openDialog.FileName);
//
////            SaveFileDialog saveDialog = new SaveFileDialog();
////            saveDialog.Title = "Save WeSay Words file as";
////            saveDialog.Filter = "WeSay Words(*.words)|*.words";
////            saveDialog.InitialDirectory = WeSayWordsProject.Project.PathToLexicalModelDB;
////            if (saveDialog.ShowDialog() != DialogResult.OK)
////            {
////                return;
////            }
////            RunCommand(new ImportLIFTCommand(saveDialog.FileName, openDialog.FileName));
//            RunCommand(new ImportLIFTCommand(openDialog.FileName));
//        }

		private void OnOpenThisProjectInWeSay(object sender, EventArgs e)
		{
			_project.Save(); //want the client to see the latest
			string dir = Directory.GetParent(Application.ExecutablePath).FullName;
			ProcessStartInfo startInfo =
					new ProcessStartInfo(Path.Combine(dir, "WeSay.App.exe"),
										 string.Format("\"{0}\"", _project.PathToLiftFile));
			Process.Start(startInfo);
		}

		private void OnExit_Click(object sender, EventArgs e)
		{
			Close();
		}


	}
}