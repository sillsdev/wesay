using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
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
	}
}
