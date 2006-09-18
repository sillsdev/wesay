using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.Admin
{
	public partial class WritingSystemBasic : UserControl
	{
		private WritingSystem _writingSystem;


		public WritingSystemBasic()
		{
			InitializeComponent();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get { return this._writingSystem; }
			set
			{
				this._writingSystem = value;
				_fontProperties.SelectedObject = _writingSystem;
				this.Refresh();
			}
		}




	}
}
