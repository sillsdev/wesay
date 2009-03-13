using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using Palaso.IO;
using Palaso.Reporting;
using Palaso.UI.WindowsForms.ImageGallery;
using WeSay.Foundation;
using WeSay.LexicalModel;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools.AddPictures
{
	public class AddPicturesTask : TaskBase
	{
		private readonly AddPicturesConfig _config;
		private readonly IFileLocator _fileLocator;
		private Control _view;
		private ArtOfReadingImageCollection _imageCollection;

		public AddPicturesTask( AddPicturesConfig config,
									LexEntryRepository lexEntryRepository,
								 TaskMemoryRepository taskMemoryRepository,
								IFileLocator fileLocator)
			: base(config, lexEntryRepository, taskMemoryRepository)
		{
			_config = config;
			_fileLocator = fileLocator;
		}

		public override void Activate()
		{
			base.Activate();
			_imageCollection = new ArtOfReadingImageCollection();
			var path = _fileLocator.LocateFile("artofreadingindexv3_en.txt");
			if(string.IsNullOrEmpty(path) || !File.Exists(path))
			{
				//NonFatalErrorDialog.Show(string.Format("Could not locate image index at {0}'",path));
				throw new ConfigurationException("Could not locate image index.");
			}
			_imageCollection.LoadIndex(path);



		}




		public override Control Control
		{
			get
			{
				if (_view == null)
				{
					_view = new AddPicturesControl(this);
				}
				return _view;
			}
		}

		public IImageCollection ImageCollection
		{
			get
			{
				return _imageCollection;
			}
		}

		protected override int ComputeCount(bool returnResultEvenIfExpensive)
		{
			return 0;
		}

		protected override int ComputeReferenceCount()
		{
			return 0;
		}
	}
}
