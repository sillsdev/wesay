using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using Palaso.IO;
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
		private readonly Color _shyLinkColor = Color.LightGray;

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
				_searchGalleryLink.Visible = ArtOfReadingImageCollection.IsAvailable();
				_chooseImageLink.Visible = true;
				_pictureBox.Visible = false;
				_problemLabel.Visible = false;
				Height = _chooseImageLink.Bottom + 5;
			}
			else if (!File.Exists(GetPathToImage()))
			{
				_pictureBox.Visible = false;
				_problemLabel.Text = _relativePathToImage;
				string s = String.Format("~Cannot find {0}", GetPathToImage());
				toolTip1.SetToolTip(this, s);
				toolTip1.SetToolTip(_problemLabel, s);
				_searchGalleryLink.Visible = ArtOfReadingImageCollection.IsAvailable();
				_chooseImageLink.Visible = true;
				Height = _problemLabel.Bottom + 5;
			}
			else
			{
				_pictureBox.Visible = true;
				_searchGalleryLink.Visible = false;
				_chooseImageLink.Visible = false;
				//_chooseImageLink.Visible = false;
				_problemLabel.Visible = false;
				try
				{
					//inset it a bit, with white border
					_pictureBox.BackColor = Color.White;
					_pictureBox.SizeMode = PictureBoxSizeMode.CenterImage;
					_pictureBox.Image = ImageUtilities.GetThumbNail(GetPathToImage(), _pictureBox.Width - 4, _pictureBox.Height - 4, Color.White);
					// _pictureBox.Load(GetPathToImage());
					Height = _pictureBox.Bottom + 5;
				}
				catch (Exception error)
				{
					_problemLabel.Visible = true;
					_problemLabel.Text = error.Message;
				}
			}

			_removeImageLink.Visible = _pictureBox.Visible;

			_searchGalleryLink.LinkColor = _shyLinkColor;
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
				var dialog = new OpenFileDialog();
				dialog.Filter = "Images|*.jpg;*.png;*.bmp;*.gif;*.tif";
				dialog.Multiselect = false;
				dialog.Title = "Choose image";
				dialog.InitialDirectory =
						Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
				if (dialog.ShowDialog() == DialogResult.OK)
				{
					PictureChosen(dialog.FileName);
				}
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("Something went wrong getting the picture. " +
												  error.Message);
			}
		}

		private void PictureChosen(string fromPath)
		{
			try
			{
				if (File.Exists(GetPathToImage()))
				{
					File.Delete(GetPathToImage());
				}
				var fullDestPath = Path.Combine(_storageFolderPath, Path.GetFileName(fromPath));
				_relativePathToImage = fullDestPath.Replace(_pathToReferingFile, "");
				_relativePathToImage = _relativePathToImage.Trim(Path.DirectorySeparatorChar);

				File.Copy(fromPath, GetPathToImage(), true);
				UpdateDisplay();

				NotifyChanged();
			}
			catch (Exception error)
			{
				ErrorReport.NotifyUserOfProblem("WeSay was not able to copy the picture file.\r\n{0}", error.Message);
			}
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
			set { _relativePathToImage = value; }
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
			_chooseImageLink.LinkColor = Color.Blue;
			_searchGalleryLink.LinkColor = ArtOfReadingImageCollection.IsAvailable() ? Color.Blue : _shyLinkColor;
		}

		private void _chooseImageLink_MouseLeave(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = _shyLinkColor;
			_searchGalleryLink.LinkColor = _shyLinkColor;
		}

		private void _removeImageLink_MouseLeave(object sender, EventArgs e)
		{
			_removeImageLink.LinkColor = _shyLinkColor;
		}

		private void ImageDisplayWidget_MouseHover(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = Color.Blue;
			_searchGalleryLink.LinkColor = ArtOfReadingImageCollection.IsAvailable() ? Color.Blue : _shyLinkColor;
			_removeImageLink.LinkColor = Color.Blue;
		}

		private void ImageDisplayWidget_MouseLeave(object sender, EventArgs e)
		{
			_chooseImageLink.LinkColor = _shyLinkColor;
			_searchGalleryLink.LinkColor = _shyLinkColor;
			_removeImageLink.LinkColor = _shyLinkColor;
		}


		private void OnSearchGalleryLink_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
		{
			if (!ArtOfReadingImageCollection.IsAvailable())
			{
				MessageBox.Show("Could not find the Art Of Reading image collection.");
				return;
			}
			var images = new ArtOfReadingImageCollection();
			string pathToIndexFile = _fileLocator.LocateFile("ArtOfReadingIndexV3_en.txt");
			if (String.IsNullOrEmpty(pathToIndexFile))
			{
				throw new FileNotFoundException("Could not find Art of reading index file.");
			}
			images.LoadIndex(pathToIndexFile);
			images.RootImagePath = ArtOfReadingImageCollection.TryToGetRootImageCatalogPath();
			var searchString = SearchTermProvider == null ? string.Empty : SearchTermProvider.SearchString;
			searchString = images.StripNonMatchingKeywords(searchString);
			using (var chooser = new PictureChooser(images, searchString))
			{
				chooser.ShowInTaskbar = false;
				chooser.ShowIcon = false;
				chooser.MinimizeBox = false;
				chooser.MaximizeBox = false;

				if (DialogResult.OK == chooser.ShowDialog())
				{
					PictureChosen(chooser.ChosenPath);
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
	}

	public interface ISearchTermProvider
	{
		string SearchString { get; }
	}
}