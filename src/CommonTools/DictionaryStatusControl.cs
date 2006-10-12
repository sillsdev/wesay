using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Language;
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
		}


		public DictionaryStatusControl(IBindingList records)
		{
			_records = records;
			InitializeComponent();
			this._dictionarySizeLabel.Text = String.Format(StringCatalog.Get(this._dictionarySizeLabel.Text), records.Count);
		}
	}
}
