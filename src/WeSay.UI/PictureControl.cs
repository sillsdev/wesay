using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Palaso.IO;
using Palaso.UI.WindowsForms.ImageToolbox;
using Palaso.UiBindings;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.ImageGallery;

namespace WeSay.UI
{
	public partial class PictureControl : UserControl, IBindableControl<string>
	{
		public event EventHandler ValueChanged;
		public event EventHandler GoingAway;

		private string _relativePathToImage;
		private readonly string _pathToReferingFile;
		private readonly string _storageFolderPath;
		private readonly IFileLocator _fileLocator;
		private readonly Color _shyLinkColor = Color.Black;

		public PictureControl(string pathToReferingFile, string storageFolderPath, IFileLocator fileLocator)
		{
			InitializeComponent();
			_pathToReferingFile = pathToReferingFile;
			_storageFolderPath = storageFolderPath;
			_fileLocator = fileLocator;
			if (!Directory.Exists(storageFolderPath))
			{
				Directory.CreateDirectory(storageFolderPath);
			}
		}
		public ISearchTermProvider SearchTermProvider { get; set; }

		public string RelativePathToImage
		{
			get { return _relativePathToImage; }
			set { _relativePathToImage = value; }
		}

		private void UpdateDisplay()
		{
			toolTip1.SetToolTip(this, "");
			toolTip1.SetToolTip(_problemLabel, "");

			if (string.IsNullOrEmpty(_relativePathToImage))
			{
				_imageToolboxLink.Visible = true;
				_pictureBox.Visible = false;
				_removeImageLink.Visible = false;
				_problemLabel.Visible = false;
				Height = _imageToolboxLink.Bottom + 5;
			}
			else if (!File.Exists(GetPathToImage()))
			{
				_pictureBox.Visible = false;
				_removeImageLink.Visible = false;
				_problemLabel.Text = _relativePathToImage;
				string s = String.Format("~Cannot find {0}", GetPathToImage());
				toolTip1.SetToolTip(this, s);
				toolTip1.SetToolTip(_problemLabel, s);
				_imageToolboxLink.Visible = true;
				Height = _problemLabel.Bottom + 5;
			}
			else
			{
				_pictureBox.Visible = true;
				_removeImageLink.Visible = true;	// need explicit setting for Mono
				_imageToolboxLink.Visible = false;
				_problemLabel.Visible = false;
				try
				{
					//inset it a bit, with white border
					_pictureBox.BackColor = Color.White;
					_pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
					_pictureBox.Image = ImageUtilities.GetThumbNail(GetPathToImage(), _pictureBox.Width - 4, _pictureBox.Height - 4, Color.White);
					Height = _pictureBox.Bottom + 5;
				}
				catch (Exception error)
				{
					_problemLabel.Visible = true;
					_problemLabel.Text = error.Message;
				}
			}

			_imageToolboxLink.LinkColor = _shyLinkColor;
			_removeImageLink.LinkColor = _shyLinkColor;
		}

		private void ImageDisplayWidget_Load(object sender, EventArgs e)
		{
			UpdateDisplay();
		}

		private void NotifyChanged()
		{
			Logger.WriteMinorEvent("Picture Control Changed");
			if (ValueChanged != null)
			{
				ValueChanged.Invoke(this, null);
			}
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (GoingAway != null)
			{
				GoingAway.Invoke(this, null); //shake any bindings to us loose
			}
			GoingAway = null;
			base.OnHandleDestroyed(e);
		}

		public string Value
		{
			get { return _relativePathToImage; }
			set
			{
				// Normalize the relative path for the local platform.  Ie, convert \ characters
				// from Windows to / characters for the benefit of Linux and Mac.  (Windows can
				// read / characters as well, so keep the code simple and always do this.)
				// This block fixes https://jira.sil.org/browse/WS-87.
				if (!String.IsNullOrEmpty(value))
					value = FileUtils.NormalizePath(value);
				_relativePathToImage = value;
			}
		}

		private string GetPathToImage()
		{
			if (string.IsNullOrEmpty(RelativePathToImage))
				return string.Empty;

			var p = Path.Combine(_pathToReferingFile, _relativePathToImage);
			if (!File.Exists(p))
			{
				//the old style was to just give the file name
				var alternatePath = Path.Combine(_storageFolderPath, _relativePathToImage);
				if (File.Exists(alternatePath))
				{
					return alternatePath;
				}
				if (!_relativePathToImage.Contains(Path.DirectorySeparatorChar.ToString()))
				{
					return alternatePath; // show where we expected it to be
				}
			}
			return p; // show where we expected it to be
		}

		private void _removeImageLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			//    Why did I think we should rename the photo... makes it hard to change your mind...
			//   I think we can better add a function some day to trim the photos to ones you're really using
			//            try
			//            {
			//                if (File.Exists(this.GetPathToImage()))
			//                {
			//                    string old = this.GetPathToImage();
			//                    _relativePathToImage = "Unused_" + _relativePathToImage;
			//                    if(!File.Exists(GetPathToImage()))
			//                    {
			//                        File.Move(old, GetPathToImage());
			//                    }
			//                }
			//            }
			//            catch(Exception error)
			//            {
			//                Palaso.Reporting.ErrorReport.NotifyUserOfProblem(error.Message);
			//            }

			_relativePathToImage = string.Empty;
			NotifyChanged();
			UpdateDisplay();
		}

		private void _removeImageLink_MouseEnter(object sender, EventArgs e)
		{
			_removeImageLink.LinkColor = Color.Blue;
		}

		private void _chooseImageLink_MouseEnter(object sender, EventArgs e)
		{
			_imageToolboxLink.LinkColor = Color.Blue;
		}

		private void _chooseImageLink_MouseLeave(object sender, EventArgs e)
		{
			_imageToolboxLink.LinkColor = _shyLinkColor;
		}

		private void _removeImageLink_MouseLeave(object sender, EventArgs e)
		{
			_removeImageLink.LinkColor = _shyLinkColor;
		}

		private void ImageDisplayWidget_MouseHover(object sender, EventArgs e)
		{
			_imageToolboxLink.LinkColor = Color.Blue;
			_removeImageLink.LinkColor = Color.Blue;
		}

		private void ImageDisplayWidget_MouseLeave(object sender, EventArgs e)
		{
			_imageToolboxLink.LinkColor = _shyLinkColor;
			_removeImageLink.LinkColor = _shyLinkColor;
		}


		private void OnSearchGalleryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			Cursor = Cursors.WaitCursor;
		   var searchString = SearchTermProvider == null ? string.Empty : SearchTermProvider.SearchString;
			PalasoImage currentImage=null;

			try
			{
				if (!string.IsNullOrEmpty(_relativePathToImage) && File.Exists(GetPathToImage()))
				{
					currentImage = PalasoImage.FromFile(GetPathToImage());
				}
			}
			catch(Exception)
			{
				//if we couldn't load it (like if it's missing), best to carry on and let them pick a new one
			}

			using(var dlg = new Palaso.UI.WindowsForms.ImageToolbox.ImageToolboxDialog(currentImage ?? new PalasoImage(), searchString))
			{
				if(DialogResult.OK == dlg.ShowDialog(this.ParentForm))
				{
					try
					{
						if (File.Exists(GetPathToImage()))
						{
							File.Delete(GetPathToImage());
						}
						string fileName = searchString;

						if(string.IsNullOrEmpty(fileName))
							fileName = DateTime.UtcNow.ToFileTimeUtc().ToString();

						string fileExt;
						if ((!String.IsNullOrEmpty(dlg.ImageInfo.FileName)) && (!String.IsNullOrEmpty(dlg.ImageInfo.FileName.Split('.').Last())))
						{
							fileExt = "." + dlg.ImageInfo.FileName.Split('.').Last();
						}
						else
						{
							// If no file name or extension, default to png
							fileExt = ".png";
						}
						//NB: we have very possible collision if use a real word "bird".
						//Less so with a time "3409343839", which this only uses if we don't have a file name (e.g. if it came from a scanner)
						//so this will add to the name if what we have is not unique.
						if (File.Exists(Path.Combine(_storageFolderPath, fileName + fileExt)))
						{
							fileName += "-"+DateTime.UtcNow.ToFileTimeUtc();
						}

						fileName += fileExt;
						var fullDestPath = Path.Combine(_storageFolderPath, fileName);
						_relativePathToImage = fullDestPath.Replace(_pathToReferingFile, "");
						_relativePathToImage = _relativePathToImage.Trim(Path.DirectorySeparatorChar);

						dlg.ImageInfo.Save(GetPathToImage());
						UpdateDisplay();
						NotifyChanged();
					}
					catch (Exception error)
					{
						ErrorReport.NotifyUserOfProblem("WeSay was not able to save the picture file.\r\n{0}", error.Message);
					}
				}
			}
		}

		/// <summary>
		/// See WS-1214 (hatton) pressing 'n' or 'u' or 'd' with focus on picture control is like alt+n, alt+u, etc.
		/// </summary>
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if (Keys.None != (keyData & Keys.Modifiers) ||
				   Keys.Tab == (keyData & Keys.Tab) ||
				   Keys.Up == (keyData & Keys.Up) ||
				   Keys.Down == (keyData & Keys.Down))
			{
				return base.ProcessCmdKey(ref msg, keyData);
			}
			return true;
		}

		private void _pictureBox_Click(object sender, EventArgs e)
		{
			OnSearchGalleryLink_LinkClicked(sender, null);
		}
	}

	public interface ISearchTermProvider
	{
		string SearchString { get; }
	}
}