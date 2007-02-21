using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
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
		}

		private void _textProjectName_TextChanged(object sender, EventArgs e)
		{
			btnOK.Enabled = NameLooksOk;
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