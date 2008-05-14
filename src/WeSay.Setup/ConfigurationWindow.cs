using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Project;
using WeSay.Setup.Properties;

namespace WeSay.Setup
{
	public partial class ConfigurationWindow : Form
	{
		private WelcomeControl _welcomePage;
		private SettingsControl _projectSettingsControl;
		private WeSayWordsProject _project;

		public ConfigurationWindow(string[] args)
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
				UpdateStuffCallback d = UpdateEnabledStates;
				Invoke(d, new object[] {});
			}
			else
			{
				openProjectInWeSayToolStripMenuItem.Enabled = (_project != null);
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
				ErrorReport.ReportNonFatalMessage(
					"WeSay could not find the file at {0} anymore.  Maybe it was moved or renamed?", configFilePath);
				return;
			}

			OpenProject(configFilePath);

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
			_welcomePage.NewProjectClicked += OnCreateProject;
			_welcomePage.OpenPreviousProjectClicked += OnOpenProject;
			_welcomePage.ChooseProjectClicked += OnChooseProject;
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