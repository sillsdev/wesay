using System;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace WeSay.Setup
{
	public partial class NewProject : Form
	{
		public NewProject()
		{
			InitializeComponent();
			btnOK.Enabled = false;
			_pathLabel.Text = "";
		}

		private void _textProjectName_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = NameLooksOk;
			if (btnOK.Enabled)
			{
				string[] dirs = SelectedPath.Split(Path.DirectorySeparatorChar);
				if (dirs.Length > 1)
				{
					string root = Path.Combine(dirs[dirs.Length - 3], dirs[dirs.Length - 2]);
					_pathLabel.Text = String.Format("Project will be created at: {0}", Path.Combine(root, dirs[dirs.Length - 1]));
				}

				_pathLabel.Invalidate();
				Debug.WriteLine(_pathLabel.Text);
			}
			else
			{
				if (_textProjectName.Text.Length > 0)
				{
					_pathLabel.Text = "Unable to create a new project there.";
				}
				else
				{
					_pathLabel.Text = "";
				}
			}
		}

		private bool NameLooksOk
		{
			get
			{
				//http://regexlib.com/Search.aspx?k=file+name
				//Regex legalFilePattern = new Regex(@"(.*?)");
//               if (!(legalFilePattern.IsMatch(_textProjectName.Text)))
//               {
//                   return false;
//               }

				if (_textProjectName.Text.Trim().Length < 1)
				{
					return false;
				}


				if (_textProjectName.Text.IndexOfAny(Path.GetInvalidFileNameChars()) > -1)
			   {
				   return false;
			   }

				if (Directory.Exists(SelectedPath) || File.Exists(SelectedPath))
				{
					return false;
				}
				return true;
			}
		}

		private string DestinationDirectory
		{
			get
			{
				return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "WeSay");

			}
		}

		public string SelectedPath
		{
			get
			{
				return Path.Combine(DestinationDirectory, _textProjectName.Text);
			}
		}

		private void btnOK_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.OK;
			this.Close();
		}

		private void btnCancel_Click(object sender, EventArgs e)
		{
			this.DialogResult = DialogResult.Cancel;
			this.Close();
		}
	}
}