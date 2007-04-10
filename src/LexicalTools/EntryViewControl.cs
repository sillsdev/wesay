using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Project;
using WeSay.UI;
using WeSay.LexicalModel;

namespace WeSay.LexicalTools
{
	public partial class EntryViewControl : UserControl
	{
		private ViewTemplate _viewTemplate;
		private LexEntry _record;

		public EntryViewControl()
		{
			_viewTemplate = null;
			InitializeComponent();
			//_detailListControl.Size = new Size((this.Right-10)-_detailListControl.Left, (this.Bottom-10) - _detailListControl.Top);
			//_detailListControl.Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top | AnchorStyles.Bottom;
			_detailListControl.ChangeOfWhichItemIsInFocus += new EventHandler<CurrentItemEventArgs>(OnChangeOfWhichItemIsInFocus);
			_detailListControl.KeyDown += new KeyEventHandler(_detailListControl_KeyDown);
		}


//        public EntryViewControl(viewTemplate viewTemplate)
//        {
//            if (viewTemplate == null)
//            {
//                throw new ArgumentNullException();
//            }
//            viewTemplate = viewTemplate;
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
		public ViewTemplate  ViewTemplate
		{
			get
			{
				return _viewTemplate;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_viewTemplate = value;
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

		public DetailList ControlEntryDetail
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
						_record.CleanUpAfterEditting();
						_record.PropertyChanged -= OnRecordPropertyChanged;
						_record.EmptyObjectsRemoved -= OnEmptyObjectsRemoved;
					}
					_record = value;
					_currentItemInFocus = null;
					if (_record != null)
					{
						_record.PropertyChanged += new PropertyChangedEventHandler(OnRecordPropertyChanged);
						_record.EmptyObjectsRemoved +=new EventHandler(OnEmptyObjectsRemoved);
					}
					RefreshLexicalEntryPreview();
					RefreshEntryDetail();
				}
			}
		}

		private void OnEmptyObjectsRemoved(object sender, EventArgs e)
		{
			//find out where our current focus is and attempt to return to that place
			int? row = null;
			if (this._detailListControl.ContainsFocus)
			{
				row = this._detailListControl.GetRow(this._detailListControl.FocussedImmediateChild);
			}
			RefreshEntryDetail();
			Application.DoEvents();
			if (row != null)
			{
				row = Math.Min((int)row, this._detailListControl.Count-1);
				this._detailListControl.MoveInsertionPoint((int)row);
			}
		}

		private void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			RefreshLexicalEntryPreview();
		}

		private void RefreshLexicalEntryPreview()
		{
			_lexicalEntryPreview.Rtf = RtfRenderer.ToRtf(_record, _currentItemInFocus);
			_lexicalEntryPreview.Refresh();
		}

		private void RefreshEntryDetail()
		{
		  this._panelEntry.SuspendLayout();
			this._detailListControl.SuspendLayout();

			this._detailListControl.Clear();
		  //this._panelEntry.Controls.Remove(this._detailListControl);
		  //  this._detailListControl = new DetailList();
		  //  this._detailListControl.AutoScroll = true;
		  //  this._detailListControl.BackColor = BackColor;
		  //  this._detailListControl.Dock = System.Windows.Forms.DockStyle.Fill;
		  //  this._detailListControl.Location = new System.Drawing.Point(0, 0);
		  //  this._detailListControl.Margin = new System.Windows.Forms.Padding(0);
		  //  this._detailListControl.Name = "_detailListControl";
		  //  this._detailListControl.Size = new System.Drawing.Size(474, 277);
		  //  this._detailListControl.TabIndex = 1;
		  //this._detailListControl.AutoSize = true;
		  //this._detailListControl.AutoSizeMode = AutoSizeMode.GrowAndShrink;

		  this._panelEntry.Controls.Add(_detailListControl);
			if (this._record != null)
			{
				LexEntryLayouter layout = new LexEntryLayouter(this._detailListControl, ViewTemplate);
				layout.AddWidgets(this._record);
			}
			this._detailListControl.ResumeLayout(true);
		  this._panelEntry.ResumeLayout(true);
		}

		private void OnChangeOfWhichItemIsInFocus(object sender, CurrentItemEventArgs e)
		{
			_currentItemInFocus = e;
			RefreshLexicalEntryPreview();
		}

		private CurrentItemEventArgs _currentItemInFocus;

		private void LexPreviewWithEntryControl_BackColorChanged(object sender, EventArgs e)
		{
			_detailListControl.BackColor = BackColor;
			_lexicalEntryPreview.BackColor = BackColor;
		}
	}
}
