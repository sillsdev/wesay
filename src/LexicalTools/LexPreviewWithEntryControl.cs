using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Project;
using WeSay.UI;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools
{
	public partial class LexPreviewWithEntryControl : UserControl
	{
		private FieldInventory _fieldInventory;
		private LexEntry _record;

		public LexPreviewWithEntryControl()
		{
			_fieldInventory = null;
			InitializeComponent();
			_detailListControl.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(OnCurrentItemChanged);
			_detailListControl.KeyDown += new KeyEventHandler(_detailListControl_KeyDown);
		}


//        public LexPreviewWithEntryControl(FieldInventory fieldInventory)
//        {
//            if (fieldInventory == null)
//            {
//                throw new ArgumentNullException();
//            }
//            FieldInventory = fieldInventory;
//            InitializeComponent();
//            _detailListControl.CurrentItemChanged += new EventHandler<CurrentItemEventArgs>(OnCurrentItemChanged);
//            _detailListControl.KeyDown += new KeyEventHandler(_detailListControl_KeyDown);
//        }

		void _detailListControl_KeyDown(object sender, KeyEventArgs e)
		{
			OnKeyDown(e);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public FieldInventory  FieldInventory
		{
			get
			{
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
				return _lexicalEntryPreview;
			}
		}

		public WeSay.UI.DetailList ControlEntryDetail
		{
			get
			{
				return _detailListControl;
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
				if (_record != value)
				{
					if (_record != null)
					{
						_record.PropertyChanged -= OnRecordPropertyChanged;
					}
					_record = value;
					_currentItem = null;
					if (_record != null)
					{
						_record.PropertyChanged += new PropertyChangedEventHandler(OnRecordPropertyChanged);
					}
					RefreshLexicalEntryPreview();
					RefreshEntryDetail();
				}
			}
		}


		private void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshLexicalEntryPreview();
		}

		private void RefreshLexicalEntryPreview()
		{
			_lexicalEntryPreview.Rtf = RtfRenderer.ToRtf(_record, _currentItem);
			_lexicalEntryPreview.Refresh();
		}

		private void RefreshEntryDetail()
		{
			this._detailListControl.SuspendLayout();
			this._detailListControl.Clear();
			if (this._record != null)
			{
				LexEntryLayouter layout = new LexEntryLayouter(this._detailListControl, this.FieldInventory);
				layout.AddWidgets(this._record);
			}

			this._detailListControl.ResumeLayout(true);
			this._detailListControl.Refresh();
		}

		private void OnCurrentItemChanged(object sender, CurrentItemEventArgs e)
		{
			_currentItem = e;
			RefreshLexicalEntryPreview();
		}

		private CurrentItemEventArgs _currentItem;

		private void LexPreviewWithEntryControl_BackColorChanged(object sender, EventArgs e)
		{
			_detailListControl.BackColor = BackColor;
			_lexicalEntryPreview.BackColor = BackColor;
		}
	}
}
