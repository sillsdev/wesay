using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using Palaso.UI.WindowsForms.i8n;
using WeSay.Foundation;
using WeSay.UI;

namespace WeSay.CommonTools
{
	public partial class DictionaryStatusControl : UserControl
	{
		private IBindingList _records;

		public DictionaryStatusControl()
		{
			Debug.Assert(this.DesignMode);
			InitializeComponent();
			ShowLogo = false;
		}


		public DictionaryStatusControl(IBindingList records)
		{
			_records = records;
			InitializeComponent();
			this._dictionarySizeLabel.Text = String.Format(StringCatalog.Get(this._dictionarySizeLabel.Text), records.Count);
		}

		public bool ShowLogo
		{
			set
			{
				_logoImage.Visible = value;
			}
		}
		private void _dictionarySizeLabel_Click(object sender, EventArgs e)
		{

		}

		private void DictionaryStatusControl_FontChanged(object sender, EventArgs e)
		{
			this._dictionarySizeLabel.Font = this.Font;
		}

		private void _dictionarySizeLabel_FontChanged(object sender, EventArgs e)
		{

		}
	}
}
