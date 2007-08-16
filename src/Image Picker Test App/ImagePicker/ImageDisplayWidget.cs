using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace ImagePicker
{
	public partial class ImageDisplayWidget : UserControl
	{
		private string _fileName;
		private string _storageFolderPath;

		public ImageDisplayWidget()
		{
			InitializeComponent();
		}

		/// <summary>
		/// The name of the file which must be in the StorageFolder
		/// </summary>
		public string FileName
		{
			get { return _fileName; }
			set { _fileName = value; }
		}

		public string StorageFolderPath
		{
			get { return _storageFolderPath; }
			set
			{
				_storageFolderPath = value;
				Directory.CreateDirectory(value);
			}
		}

		private void UpdateDisplay()
		{

			if(string.IsNullOrEmpty(_fileName ))
			{
				_chooseImageLink.Visible = true;
				_pictureBox.Visible = false;
				_problemLabel.Visible = false;
				Height = _chooseImageLink.Bottom + 5;
			}
			else if (!File.Exists(GetPathToImage()))
			{
				_pictureBox.Visible = false;
				_problemLabel.Text = String.Format("~Cannot find {0}", GetPathToImage());
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
				catch(Exception error)
				{
					_problemLabel.Visible = true;
					_problemLabel.Text = error.Message;
				}
			}

			_removeImageLink.Visible = _pictureBox.Visible;

		}

		private void ImageDisplayWidget_Load(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private void _chooseImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			OpenFileDialog dialog = new OpenFileDialog();
			dialog.Filter = "Images|*.jpg;*.png;*.bmp;*.gif";
			dialog.Multiselect = false;
			dialog.Title = "Choose image";
			if(dialog.ShowDialog() == DialogResult.OK)
			{
				_fileName = System.IO.Path.GetFileName(dialog.FileName);
				File.Copy(dialog.FileName, GetPathToImage());
				UpdateDisplay();
			}
		}

		private string GetPathToImage()
		{
			return System.IO.Path.Combine(_storageFolderPath, _fileName);
		}

		private void _removeImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			_fileName = string.Empty;
			UpdateDisplay();
		}
	}
}
