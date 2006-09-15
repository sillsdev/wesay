using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;
using WeSay.Admin.Properties;
using WeSay.UI;

namespace WeSay.Admin
{
	public partial class AdminWindow : Form
	{
		private WelcomeControl _welcomePage = new WelcomeControl();
		private ProjectTabs _projectTabs;
		private WeSayWordsProject _project;

		public AdminWindow()
		{

			InitializeComponent();

			if (this.DesignMode)
				return;

			InstallWelcomePage();
		}



		void OnOpenProject(object sender, EventArgs e)
		{
			string s = WeSay.Admin.Properties.Settings.Default.LastProjectPath;

			if (s == null || s =="")
			{
				s= Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
			}
			_newProjectLocationDialog.SelectedPath = s;

		   if (DialogResult.OK != _newProjectLocationDialog.ShowDialog())
				return;
		   OpenProject(_newProjectLocationDialog.SelectedPath);
		}



		private void OnCreateProject(object sender, EventArgs e)
		{
			if (DialogResult.OK != _newProjectLocationDialog.ShowDialog())
				return;
			CreateNewProject(_newProjectLocationDialog.SelectedPath);
			OpenProject(_newProjectLocationDialog.SelectedPath);
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

			if (_project != null)
			{
				_project.Dispose();
			}
			_project = p;
		}

		public void OpenProject(string path)
		{
			//System.Configuration.ConfigurationManager.AppSettings["LastProjectPath"] = path;
			WeSay.Admin.Properties.Settings.Default.LastProjectPath = path;

			try
			{
				this._project = new WeSayWordsProject();
				_project.Load(path);
			}
			catch (Exception e)
			{
				MessageBox.Show("WeSay was not able to open that project. \r\n"+e.Message);
				return;
			}

			try
			{
				this.Text = "WeSay Admin: " + _project.Name;
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
//            System.Configuration.Configuration config =
//                     ConfigurationManager.OpenExeConfiguration(
//                     ConfigurationUserLevel.None);
//
//            config.Save();//(ConfigurationSaveMode.Modified);
			WeSay.Admin.Properties.Settings.Default.Save();
			if (_projectTabs != null)
			{
				_projectTabs.Dispose();
			}
		}
	}
}
