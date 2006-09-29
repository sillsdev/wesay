using System.Drawing;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class PictureControl : UserControl, ITask
	{
		private string _label;
		private string _description;

		public PictureControl(string label, string description, string pictureFilePath)
		{
			_label = label;
			_description = description;
			InitializeComponent(new Bitmap(pictureFilePath));
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

		#endregion
	}
}
