using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class PictureControl : UserControl, ITask
	{
		private string _label;

		public PictureControl(string label, string pictureFilePath)
		{
			_label = label;
			InitializeComponent(new Bitmap(pictureFilePath));
		}

		#region ITask Members

		void ITask.Activate()
		{
		}

		void ITask.Deactivate()
		{
		}

		string ITask.Label
		{
			get
			{
				return _label;
			}
		}

		Control ITask.Control
		{
			get
			{
				return this;
			}
		}

		#endregion
	}
}
