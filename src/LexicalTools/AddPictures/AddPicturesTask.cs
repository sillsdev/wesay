using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using SIL.IO;
using SIL.Reporting;
using SIL.Windows.Forms.ImageGallery;
using WeSay.LexicalModel;
using WeSay.Project;
using System.Linq;

namespace WeSay.LexicalTools.AddPictures
{
	public class AddPicturesTask : TaskBase
	{
		private readonly AddPicturesConfig _config;
		private Control _view;
		private IImageCollection _imageCollection;

		public AddPicturesTask( AddPicturesConfig config,
									LexEntryRepository lexEntryRepository,
								 TaskMemoryRepository taskMemoryRepository,
								IFileLocator fileLocator)
			: base(config, lexEntryRepository, taskMemoryRepository)
		{
			_config = config;
		}

		public override void Activate()
		{
			base.Activate();
			_imageCollection = ArtOfReadingImageCollection.FromStandardLocations();
			if(_imageCollection == null)
			{
				throw new ConfigurationException("Could not locate image index.");
			}
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

		public override void FocusDesiredControl()
		{
			// This is the place to implement how the AddPictureTask selects its desired child control
			return;
		}
	}
}
