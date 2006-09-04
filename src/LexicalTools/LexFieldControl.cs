using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.UI;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		LexEntry _record;
		public LexFieldControl()
		{
			InitializeComponent();
		}

		public LexFieldControl(Predicate<string> filter)
		{
			InitializeComponent();

			_entryDetailControl.ShowField = filter;
		}

		public Predicate<string>  ShowField
		{
			get
			{
				return _entryDetailControl.ShowField;
			}
			set
			{
				_entryDetailControl.ShowField = value;
			}
		}


		public string Control_DataView
		{
			get
			{
				return _lexicalEntryView.Text;
			}
			set
			{
				_lexicalEntryView.Text = value; // this could go to rtf depending on the value
			}
		}

		public EntryDetailControl Control_EntryDetail
		{
			get
			{
				return _entryDetailControl;
			}
		}

		public LexEntry DataSource
		{
			get
			{
				return _record;
			}
			set
			{
				_record = value;
				_entryDetailControl.DataSource = value;
				if (_record == null)
				{
					_lexicalEntryView.Text = String.Empty;
				}
				else
				{
					_lexicalEntryView.Rtf = _record.ToRtf();
				}
			}
		}

	}
}
