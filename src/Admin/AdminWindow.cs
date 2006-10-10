using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using WeSay.Admin.Properties;
using WeSay.Data;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.Admin
{
	public partial class AdminWindow : Form
	{
		private WelcomeControl _welcomePage = new WelcomeControl();
		private ProjectTabs _projectTabs;
		private WeSayWordsProject _project;

		/// <summary>
		/// This is probably temporary while we transition to the tasks xml being
		/// driven by some class model rather than just XML.
		/// </summary>
		public static FieldInventory SharedFieldInventory;

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
				exportToLIFTXmlToolStripMenuItem.Enabled = (value != null);
				importFromLIFTXMLToolStripMenuItem.Enabled = (value != null);
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

			ConvertWordsFileToLIFT(saveDialog.FileName, openDialog.FileName);
		}

		private static void ConvertWordsFileToLIFT(string destinationLIFTPath, string sourceWordsPath)
		{
			LiftExporter exporter=null;
			try
			{
				exporter = new LiftExporter(destinationLIFTPath);

				using (Db4oDataSource ds = new Db4oDataSource(sourceWordsPath))
				{
					using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
					{
						exporter.Add(entries);
					}
				}
			}
			finally
			{
				exporter.End();
			}
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
			string sourceWordsPath = openDialog.FileName;
			string destPath = saveDialog.FileName;
			if (File.Exists(destPath)) // make backup of the file we're about to over-write
			{
				File.Move(destPath, destPath+".bak");
			}

			using (Db4oDataSource ds = new WeSay.Data.Db4oDataSource(destPath))
			{
				using (Db4oRecordList<LexEntry> entries = new Db4oRecordList<LexEntry>(ds))
				{
					LiftImporter importer = new LiftImporter(entries);
					importer.ReadFile(sourceWordsPath);
				}
			}

		}

	}
}
