using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.UI
{
	public partial class PictureControl : UserControl, IBindableControl<string>
	{
		private string _nameForLogging;
		public event EventHandler ValueChanged;
		public event EventHandler GoingAway;


		private string _fileName;
		private string _storageFolderPath;
		private Color _shyLinkColor = Color.LightGray;

		public PictureControl(string nameForLogging, string storageFolderPath)
		{
			InitializeComponent();
			_nameForLogging = nameForLogging;
			_storageFolderPath = storageFolderPath;
			if (!Directory.Exists(storageFolderPath))
			{
				Directory.CreateDirectory(storageFolderPath);
			}
		}

		/// <summary>
		/// The name of the file which must be in the StorageFolder
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		private void UpdateDisplay()
		{
			toolTip1.SetToolTip(this, "");
			toolTip1.SetToolTip(_problemLabel, "");

			if (string.IsNullOrEmpty(_fileName))
			{
				_chooseImageLink.Visible = true;
				_pictureBox.Visible = false;
				_problemLabel.Visible = false;
				Height = _chooseImageLink.Bottom + 5;
			}
			else if (!File.Exists(GetPathToImage()))
			{
				_pictureBox.Visible = false;
				_problemLabel.Text = _fileName;
				string s = String.Format("~Cannot find {0}", GetPathToImage());
				toolTip1.SetToolTip(this, s);
				toolTip1.SetToolTip(_problemLabel, s);
				_chooseImageLink.Visible = true;
				Height = _problemLabel.Bottom + 5;
			}
			else
			{
				_pictureBox.Visible = true;
				_chooseImageLink.Visible = false;
				//_chooseImageLink.Visible = false;
				_problemLabel.Visible = false;
				try
				{
					_pictureBox.ImageLocation = GetPathToImage();
					_pictureBox.Invalidate();
					_pictureBox.Show();
					Height = _pictureBox.Bottom + 5;

				}
				catch (Exception error)
				{
					_problemLabel.Visible = true;
					_problemLabel.Text = error.Message;
				}
			}

			_removeImageLink.Visible = _pictureBox.Visible;

			_chooseImageLink.LinkColor = _shyLinkColor;
			_removeImageLink.LinkColor = _shyLinkColor;

		}

		private void ImageDisplayWidget_Load(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private void _chooseImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				OpenFileDialog dialog = new OpenFileDialog();
				dialog.Filter = "Images|*.jpg;*.png;*.bmp;*.gif";
				dialog.Multiselect = false;
				dialog.Title = "Choose image";
				dialog.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					_fileName = System.IO.Path.GetFileName(dialog.FileName);
					if (File.Exists(GetPathToImage()))
					{
						File.Delete(GetPathToImage());
					}
					File.Copy(dialog.FileName, GetPathToImage());
					UpdateDisplay();

					Palaso.Reporting.Logger.WriteMinorEvent("Picture Control Changed ({0})", this._nameForLogging);
					if (ValueChanged != null)
					{
						ValueChanged.Invoke(this, null);
					}
				}
			}
			catch (Exception error)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage("Something went wrong getting the picture. " +
																   error.Message);
			}
		}


		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null);//shake any bindings to us loose
			}
			GoingAway = null;
			base.OnHandleDestroyed(e);
		}

		public string Value
		{
			get
			{
				return _fileName;
			}
			set
			{
			   _fileName = value;
			}
		}

		private string GetPathToImage()
		{
			return System.IO.Path.Combine(_storageFolderPath, _fileName);
		}

		private void _removeImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			try
			{
				if (File.Exists(this.GetPathToImage()))
				{
					string old = this.GetPathToImage();
					_fileName = "Unused_" + _fileName;
					File.Move(old, GetPathToImage());
				}
			}
			catch(Exception error)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage(error.Message);
			}

			_fileName = string.Empty;

			UpdateDisplay();
		}

		private void _removeImageLink_MouseEnter(object sender, EventArgs e)
		{
			_removeImageLink.LinkColor = Color.Blue;
		}

		private void _chooseImageLink_MouseEnter(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = Color.Blue;

		}

		private void _chooseImageLink_MouseLeave(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = _shyLinkColor;
		}

		private void _removeImageLink_MouseLeave(object sender, EventArgs e)
		{
			_removeImageLink.LinkColor = _shyLinkColor;
		}

		private void ImageDisplayWidget_MouseHover(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = Color.Blue;
			_removeImageLink.LinkColor = Color.Blue;

		}

		private void ImageDisplayWidget_MouseLeave(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = _shyLinkColor;
			_removeImageLink.LinkColor = _shyLinkColor;

		}
	}
}