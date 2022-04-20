using System;
using System.Windows.Forms;

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
				var results = collection.GetMatchingImages(_searchWords.Text);
				_thumbnailViewer.LoadItems(results);
			}
		}
	}
}
