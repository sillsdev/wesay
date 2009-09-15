using System;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.UI;

namespace WeSay.LexicalTools.Review.AdvancedHistory
{
	public partial class AdvancedHistoryControl: UserControl
	{

		public AdvancedHistoryControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public AdvancedHistoryControl(Chorus.UI.Review.ReviewPage view)
		{

			InitializeComponent();
			view.Dock = DockStyle.Fill;
			Controls.Add(view);

			InitializeDisplaySettings();
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
		}

		private void OnBackColorChanged(object sender, EventArgs e)
		{
		}
	}
}