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
	public partial class FontControl : UserControl
	{
		private WritingSystem _writingSystem;

		public FontControl()
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
				_fontProperties.SelectedObject = _writingSystem.Font;
				this.Refresh();
			}
		}

		private void _btnFont_Click(object sender, EventArgs e)
		{
			_fontDialog.Font = _writingSystem.Font;
			if (DialogResult.OK != _fontDialog.ShowDialog())
			{
				return;
			}
			_writingSystem.Font = _fontDialog.Font;
			_fontProperties.SelectedObject = _writingSystem.Font;
		}
	}
}
