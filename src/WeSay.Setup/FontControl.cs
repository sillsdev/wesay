using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.Setup
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

				UpdateFontDisplay();
			}
		}

		private void _btnFont_Click(object sender, EventArgs e)
		{
			_fontDialog.Font = _writingSystem.Font;
			_fontDialog.ShowColor = false;
			_fontDialog.ShowEffects = false;
			try
			{
				if (DialogResult.OK != _fontDialog.ShowDialog())
				{
					return;
				}
			}
			catch (Exception error)
			{
				Palaso.Reporting.ErrorReport.ReportNonFatalMessage("The Microsoft Font Dialog had a problem. We have seen this happen when you add a font to the system.  Try quitting this application and running it again."+System.Environment.NewLine+"The exception was: "+error.Message);
				return;
			}
			_writingSystem.Font = _fontDialog.Font;
			UpdateFontDisplay();
		  }

		private void UpdateFontDisplay()
		{
			_fontInfoDisplay.Text =
				string.Format("{0}, {1}", this.WritingSystem.Font.Name, Math.Round(this.WritingSystem.Font.Size));
			_sampleTextBox.WritingSystem =WritingSystem;
			_sampleTextBox.Text = string.Empty;
			this.Invalidate();
		}
	}
}
