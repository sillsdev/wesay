using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.ImageGallery;

namespace WeSay.LexicalTools.AddPictures
{
	public partial class AddPicturesControl : UserControl
	{
		private readonly AddPicturesTask _presentationModel;


		public AddPicturesControl()
		{
			InitializeComponent();
		}

		public AddPicturesControl(AddPicturesTask presentationModel)
		{
			_presentationModel = presentationModel;

		}

		private void _searchButton_Click(object sender, EventArgs e)
		{
			var collection = _presentationModel.ImageCollection;
			if (!string.IsNullOrEmpty(_searchWords.Text))
			{
				IEnumerable<object> results = collection.GetMatchingPictures(_searchWords.Text);

				_thumbnailViewer.LoadItems(collection.GetPathsFromResults(results, true));
			}
		}
	}
}
