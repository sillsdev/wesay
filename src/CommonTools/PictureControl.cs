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
		private WeSay.UI.IProject _project;
		private string _label;

		public PictureControl(WeSay.UI.IProject project, string label, System.Drawing.Image image)
		{
			_project = project;
			_label = label;
			InitializeComponent(image);
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
