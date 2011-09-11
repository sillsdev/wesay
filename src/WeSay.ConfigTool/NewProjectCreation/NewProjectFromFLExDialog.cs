using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.ConfigTool.Properties;

namespace WeSay.ConfigTool.NewProjectCreation
{
	public partial class NewProjectFromFLExDialog:Form
	{
		public NewProjectFromFLExDialog()
		{
			InitializeComponent();
			_btnOk.Enabled = false;
			_liftPathTextBox.TextChanged += OnLiftPathTextBox_TextChanged;
			_browseForLiftPathButton.Click += OnBrowseForLiftPathButton_Click;
			_projectNameTextBox.TextChanged += OnProjectNameTextBox_TextChanged;
			_btnOk.Click += OnBtnOK_Click;
			_btnCancel.Click += OnBtnCancel_Click;
			_linkLabel.Click += OnLinkLabel_Clicked;
			_pathtoProjectLabel.Text = String.Empty;
			Icon = Resources.WeSaySetupApplicationIcon;
		}

		public string PathToLift
		{
			get { return _liftPathTextBox.Text; }
		}

		private bool EnableOk
		{
			get { return NameLooksOk && !string.IsNullOrEmpty(PathToLift) && File.Exists(PathToLift); }
		}

		private bool NameLooksOk
		{
			get
			{
				if (_projectNameTextBox.Text.Trim().Length < 1)
				{
					return false;
				}

				if (_projectNameTextBox.Text.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
				{
					return false;
				}

				if (Directory.Exists(PathToNewProjectDirectory) || File.Exists(PathToNewProjectDirectory))
				{
					return false;
				}
				return true;
			}
		}

		public string PathToNewProjectDirectory
		{
			get { return Path.Combine(Project.WeSayWordsProject.NewProjectDirectory, _projectNameTextBox.Text); }
		}

		private static void OnLinkLabel_Clicked(object sender, EventArgs e1)
		{
			Process.Start("http://wesay.org/index.php?title=ShareWithFLEx");
		}

		private void OnBrowseForLiftPathButton_Click(object sender, EventArgs e)
		{
			using(var dlg = new OpenFileDialog())
			{
				dlg.Title = "Locate LIFT file";
				dlg.AutoUpgradeEnabled = true;
				dlg.RestoreDirectory = true;
				dlg.DefaultExt = ".lift";
				dlg.Filter = "LIFT Lexicon File (*.lift)|*.lift";

				if (DialogResult.OK != dlg.ShowDialog())
					return;

				_liftPathTextBox.Text = dlg.FileName;
			}
		}

		private void OnLiftPathTextBox_TextChanged(object sender, EventArgs e)
		{
			if (_projectNameTextBox.Text == String.Empty && _liftPathTextBox.Text.Contains(".lift"))
			{
				try
				{
					_projectNameTextBox.Text = Path.GetFileNameWithoutExtension(_liftPathTextBox.Text);
				}
				catch (Exception)
				{
				}
			}
			_btnOk.Enabled = EnableOk;
		}

		protected void OnProjectNameTextBox_TextChanged(object sender, EventArgs e)
		{
			_btnOk.Enabled = EnableOk;
			if (_btnOk.Enabled)
			{
				string[] dirs = PathToNewProjectDirectory.Split(Path.DirectorySeparatorChar);
				if (dirs.Length > 1)
				{
					string root = Path.Combine(dirs[dirs.Length - 3], dirs[dirs.Length - 2]);
					_pathtoProjectLabel.Text = String.Format("Project will be created at: {0}",
													Path.Combine(root, dirs[dirs.Length - 1]));
				}

				_pathtoProjectLabel.Invalidate();
				Debug.WriteLine(_pathtoProjectLabel.Text);
			}
			else
			{
				_pathtoProjectLabel.Text = _projectNameTextBox.Text.Length > 0 ? "Unable to create a new project there." : "";
			}
		}

		private void OnBtnOK_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void OnBtnCancel_Click(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}