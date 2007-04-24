using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
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
					_pathLabel.Text = String.Format("Project will be created at: {0}", Path.Combine(dirs[dirs.Length - 2], dirs[dirs.Length - 1]));
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

				if (System.IO.Directory.Exists(SelectedPath) || System.IO.File.Exists(SelectedPath))
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
				return System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

			}
		}

		public string SelectedPath
		{
			get
			{
				return System.IO.Path.Combine(DestinationDirectory, _textProjectName.Text);
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