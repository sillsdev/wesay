using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using MultithreadProgress;
using WeSay.Admin.Properties;
using WeSay.Data;
using WeSay.Foundation.Progress;
using WeSay.LexicalModel;
using WeSay.Project;
using ErrorEventHandler=System.IO.ErrorEventHandler;

namespace WeSay.Admin
{
	public partial class AdminWindow : Form
	{
		private WelcomeControl _welcomePage = new WelcomeControl();
		private ProjectTabs _projectTabs;
		private WeSayWordsProject _project;
		ProgressDialogHandler _progressHandler;

		/// <summary>
		/// This is probably temporary while we transition to the tasks xml being
		/// driven by some class model rather than just XML.
		/// </summary>
		//public static ViewTemplate SharedviewTemplate;

		public AdminWindow()
		{

			InitializeComponent();

			this.Project = null;


//            if (this.DesignMode)
//                return;
//
			InstallWelcomePage();
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
				exportToLIFTXmlToolStripMenuItem.Enabled = (_project != null) && (_progressHandler == null);
				importFromLIFTXMLToolStripMenuItem.Enabled = (_project != null) && (_progressHandler == null);
			}

		}

		void OnOpenProject(object sender, EventArgs e)
		{
			string selectedPath = sender as string;
			if (selectedPath == null)
			{
				string s = Settings.Default.LastProjectPath;

				if (s == null || s == "")
				{
					s = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
				}
				this._chooseProjectLocationDialog.SelectedPath = s;

				if (DialogResult.OK != this._chooseProjectLocationDialog.ShowDialog())
					return;

				selectedPath = this._chooseProjectLocationDialog.SelectedPath;
			}

			if (WeSayWordsProject.IsValidProjectDirectory(selectedPath))
			{
				OpenProject(selectedPath);
			}
			else
			{
				MessageBox.Show("That directory does not appear to be a valid WeSay Project directory.");
			}
		}




		private void OnCreateProject(object sender, EventArgs e)
		{
			if (DialogResult.OK != this._chooseProjectLocationDialog.ShowDialog())
				return;
			CreateNewProject(this._chooseProjectLocationDialog.SelectedPath);
			OpenProject(this._chooseProjectLocationDialog.SelectedPath);
		}

		public void CreateNewProject(string path)
		{
			WeSayWordsProject p;

			try
			{
				p = new WeSayWordsProject();
				p.Create(path);
			}
			catch (Exception e)
			{
				MessageBox.Show("WeSay was not able to create a project there. \r\n"+e.Message);
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
			Settings.Default.LastProjectPath = path;

			try
			{
				this.Project = new WeSayWordsProject();
				this.Project.LoadFromProjectDirectoryPath(path);
			}
			catch (Exception e)
			{
				MessageBox.Show("WeSay was not able to open that project. \r\n"+e.Message);
				return;
			}

			SetupProjectControls();
		}

		private void SetupProjectControls()
		{
			try
			{
				this.Text = "WeSay Admin: " + this.Project.Name;
				RemoveExistingControls();
				InstallProjectsControls();
			}
			catch (Exception e)
			{
				MessageBox.Show("WeSay was not able to display that project. \r\n"+e.Message);
			}
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
				e.Cancel = true;
				MessageBox.Show(error.Message);
			}


		}

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
		{
			new AboutBox().ShowDialog();
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void ExportToLiftXmlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Title="Choose the Words file to convert to LIFT";
			openDialog.FileName = WeSayWordsProject.Project.PathToLexicalModelDB;
			openDialog.Filter = "WeSay Words(*.words)|*.words";
			if (openDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.Title="Save LIFT file as";
			saveDialog.Filter = "LIFT XML (*.xml)|*.xml";
			saveDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			saveDialog.FileName=_project.Name+".lift.xml";
			if (saveDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}

			RunCommand(new ExportLIFTCommand(saveDialog.FileName, openDialog.FileName));
		}


		private void RunCommand(BasicCommand command)
		{
			_progressHandler = new ProgressDialogHandler(this, command);
			_progressHandler.Finished += new EventHandler(_progressHandler_Finished);
			ProgressState progress = new ProgressState(_progressHandler);
			UpdateEnabledStates();
			command.BeginInvoke(progress);
		}

		void _progressHandler_Finished(object sender, EventArgs e)
		{
			_progressHandler = null;
			UpdateEnabledStates();
		}


		private void ImportFromLiftXmlToolStripMenuItem_Click(object sender, EventArgs e)
		{
			OpenFileDialog openDialog = new OpenFileDialog();
			openDialog.Title = "Choose the LIFT xml file to convert to a WeSay Words file";
			openDialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			openDialog.Filter = "LIFT XML (*.xml)|*.xml";
			if (openDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			SaveFileDialog saveDialog = new SaveFileDialog();
			saveDialog.Title = "Save WeSay Words file as";
			saveDialog.Filter = "WeSay Words(*.words)|*.words";
			saveDialog.InitialDirectory = WeSayWordsProject.Project.PathToLexicalModelDB;
			if (saveDialog.ShowDialog() != DialogResult.OK)
			{
				return;
			}
			RunCommand(new ImportLIFTCommand(saveDialog.FileName, openDialog.FileName));
		}
	}
}
