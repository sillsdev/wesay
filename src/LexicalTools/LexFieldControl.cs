using System;
using System.ComponentModel;
using System.Windows.Forms;
using WeSay.UI;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools
{
	public partial class LexFieldControl : UserControl
	{
		private FieldInventory _fieldInventory;
		LexEntry _record;

		private void Initialize()
		{
			_entryDetailControl.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(OnCurrentItemChanged);
		}

		public LexFieldControl(FieldInventory fieldInventory)
		{
			_fieldInventory = fieldInventory;
			InitializeComponent();
			Initialize();
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FieldInventory  FieldInventory
		{
			get
			{
				if(_fieldInventory == null)
				{
					throw new InvalidOperationException("FieldInventory must be initialized prior to being used.");
				}
				return _fieldInventory;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_fieldInventory = value;
				Refresh();
			}
		}

		public RichTextBox ControlFormattedView
		{
			get
			{
				return _lexicalEntryView;
			}
		}

		public EntryDetailControl ControlEntryDetail
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
