using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Text;
using System.Windows.Forms;
using WeSay.UI;
using WeSay.LexicalModel;
using WeSay.Language;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		LexEntry _record;

		private void Initialize()
		{
			_entryDetailControl.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(OnCurrentItemChanged);
		}

	   public LexFieldControl()
		{
			InitializeComponent();
			Initialize();
		}

		public LexFieldControl(Predicate<string> filter)
		{
			InitializeComponent();
			Initialize();
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

		public RichTextBox Control_FormattedView
		{
			get
			{
				return _lexicalEntryView;
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
				_currentItem = null;
				if (_record == null)
				{
					_lexicalEntryView.Text = String.Empty;
				}
				else
				{
					_record.PropertyChanged +=new PropertyChangedEventHandler(OnRecordPropertyChanged);

					RefreshLexicalEntryView();
				}
			}
		}

		private void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshLexicalEntryView();
		}

		private void RefreshLexicalEntryView()
		{
			_lexicalEntryView.Rtf = RtfRenderer.ToRtf(_record, _currentItem);
		}

		private void OnCurrentItemChanged(object sender, CurrentItemEventArgs e)
		{
			_currentItem = e;
			RefreshLexicalEntryView();
		}

		private CurrentItemEventArgs _currentItem;
	}
}
