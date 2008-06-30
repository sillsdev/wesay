using System;
using System.ComponentModel;
using System.Windows.Forms;
using Palaso.Reporting;
using WeSay.Foundation;

namespace WeSay.ConfigTool
{
	public partial class FontControl: UserControl
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
			get { return _writingSystem; }
			set
			{
				_writingSystem = value;

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
				ErrorReport.ReportNonFatalMessage(
						"The Microsoft Font Dialog had a problem. We have seen this happen when you add a font to the system.  Try quitting this application and running it again." +
						Environment.NewLine + "The exception was: " + error.Message);
				return;
			}
			_writingSystem.Font = _fontDialog.Font;
			UpdateFontDisplay();
		}

		private void UpdateFontDisplay()
		{
			_fontInfoDisplay.Text =
					string.Format("{0}, {1}",
								  WritingSystem.Font.Name,
								  Math.Round(WritingSystem.Font.Size));
			_sampleTextBox.WritingSystem = WritingSystem;
			_sampleTextBox.Text = string.Empty;
			Invalidate();
		}
	}
}