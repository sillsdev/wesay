using System;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;

namespace WeSay.CommonTools
{
	public partial class DictionaryStatusControl: UserControl
	{
		public DictionaryStatusControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
			ShowLogo = false;
		}

		public DictionaryStatusControl(int count)
		{
			InitializeComponent();
			_dictionarySizeLabel.Text =
					String.Format(StringCatalog.Get(_dictionarySizeLabel.Text), count);
		}

		public bool ShowLogo
		{
			set { _logoImage.Visible = value; }
		}

		private void _dictionarySizeLabel_Click(object sender, EventArgs e) {}

		private void DictionaryStatusControl_FontChanged(object sender, EventArgs e)
		{
			_dictionarySizeLabel.Font = Font;
		}

		private void _dictionarySizeLabel_FontChanged(object sender, EventArgs e) {}
	}
}