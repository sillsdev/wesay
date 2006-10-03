using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class PictureControl : UserControl, ITask
	{
		int _i;
		private string _label;
		private string _description;

		public PictureControl(string label, string description, string pictureFilePath)
		{
			_label = label;
			_description = description;
			InitializeComponent(new Bitmap(pictureFilePath));
			_i = new Random().Next(20, 100);
		}

		#region ITask Members

		public void Activate()
		{
		}

		public void Deactivate()
		{
		}

		public string Label
		{
			get
			{
				return _label;
			}
		}

		public Control Control
		{
			get
			{
				return this;
			}
		}

		public string Description
		{
			get
			{
				return _description;
			}
		}

		public bool IsPinned
		{
			get
			{
				return false;
			}
		}

		public string Status
		{
			get
			{
				return _i.ToString();
			}
		}
		#endregion
	}
}
