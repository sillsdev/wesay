using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using WeSay.ConfigTool.Properties;

namespace WeSay.ConfigTool.NewProjectCreation
{
	public partial class NewProjectFromRawLiftDialog : Form
	{
		public NewProjectFromRawLiftDialog()
		{
			InitializeComponent();
			_btnOk.Enabled = false;
			_liftPathTextBox.TextChanged += OnLiftPathTextBoxTextChanged;
			_browseForLiftPathButton.Click += OnBrowseForLiftPathButtonClick;
			_projectNameTextBox.TextChanged += OnProjectNameTextBoxTextChanged;
			_btnOk.Click += OnBtnOKClick;
			_btnCancel.Click += OnBtnCancelClick;
			_linkLabel.Click += OnLinkLabelClicked;
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

		private static void OnLinkLabelClicked(object sender, EventArgs e1)
		{
			Process.Start("http://software.sil.org/fieldworks/wp-content/uploads/sites/38/2017/07/Technical-Notes-on-FieldWorks-Send-Receive.pdf");
		}

		private void OnBrowseForLiftPathButtonClick(object sender, EventArgs e)
		{
			using (var dlg = new OpenFileDialog())
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

		private void OnLiftPathTextBoxTextChanged(object sender, EventArgs e)
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

		protected void OnProjectNameTextBoxTextChanged(object sender, EventArgs e)
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

		private void OnBtnOKClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void OnBtnCancelClick(object sender, EventArgs e)
		{
			DialogResult = DialogResult.Cancel;
			Close();
		}
	}
}
