using System;
using System.Collections.Generic;
using System.ComponentModel;
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

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
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


		public string Control_FormattedView
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
				if (_record != null)
				{
					_record.PropertyChanged -= OnRecordPropertyChanged;
				}
				_record = value;
				_entryDetailControl.DataSource = value;
				if (_record == null)
				{
					_lexicalEntryView.Text = String.Empty;
				}
				else
				{
					_record.PropertyChanged +=new PropertyChangedEventHandler(OnRecordPropertyChanged);

					_lexicalEntryView.Rtf = _record.ToRtf();
				}
			}
		}

		private void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			_lexicalEntryView.Rtf = _record.ToRtf();
		}

	}
}
