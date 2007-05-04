using System;
using System.ComponentModel;
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
		private System.Windows.Forms.Timer _cleanupTimer;

		public EntryViewControl()
		{
			_viewTemplate = null;
			InitializeComponent();
			_detailListControl.ChangeOfWhichItemIsInFocus += new EventHandler<CurrentItemEventArgs>(OnChangeOfWhichItemIsInFocus);
			_detailListControl.KeyDown += new KeyEventHandler(_detailListControl_KeyDown);
		}

		protected override void OnHandleDestroyed(EventArgs e)
		{
			if (_cleanupTimer != null)
			{
				_cleanupTimer.Dispose();
			}
			base.OnHandleDestroyed(e);
		}

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
				if(_viewTemplate == null)
				{
					throw new InvalidOperationException("ViewTemplate must be initialized");
				}
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
				Reporting.Logger.WriteMinorEvent("In DataSource Set");

				if (_record != value)
				{
					if (_record != null)
					{
						Reporting.Logger.WriteMinorEvent("Datasource set calling _record.CleanUpAfterEditting()");

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
					Reporting.Logger.WriteMinorEvent("Datasource set calling RefreshLexicalEntryPreview()");
					RefreshLexicalEntryPreview();
					Reporting.Logger.WriteMinorEvent("Datasource set calling RefreshEntryDetail()");
					RefreshEntryDetail();
				}

				Reporting.Logger.WriteMinorEvent("Exit DataSource Set");

			}
		}

		private void OnEmptyObjectsRemoved(object sender, EventArgs e)
		{
			//find out where our current focus is and attempt to return to that place
			int? row = null;
			if (this._detailListControl.ContainsFocus)
			{
				Control focussedControl = this._detailListControl.FocussedImmediateChild;
				if (focussedControl != null)
				{
					row = this._detailListControl.GetRow(focussedControl);
				}
			}
			Reporting.Logger.WriteMinorEvent("OnEmptyObjectsRemoved: b4 RefreshEntryDetial");
			RefreshEntryDetail();
			Reporting.Logger.WriteMinorEvent("OnEmptyObjectsRemoved: b4  Application.DoEvents()");
			for(int i=0; i< 1000;i++)
				Application.DoEvents(); //TODO: We need to remove this.  It's a dangerous thing in a historically buggy spot
			Reporting.Logger.WriteMinorEvent("OnEmptyObjectsRemoved: b4 MoveInsertionPoint");
			if (row != null)
			{
				row = Math.Min((int)row, this._detailListControl.Count-1);
				this._detailListControl.MoveInsertionPoint((int)row);
			}
			Reporting.Logger.WriteMinorEvent("OnEmptyObjectsRemoved end");
		}

		private void OnRecordPropertyChanged(object sender, PropertyChangedEventArgs e)
		{
			LexEntry entry = (LexEntry) sender;
			switch (e.PropertyName)
			{
				// these are changes to the list not a change that needs to clean up
				// and can actually have very detrimental effect if we do.
				case "exampleSentences":
				case "senses":
					break;
				default:
					//this is to fix WS-238. The scenario is this:
					//a paste operation starts by erasing the target... this fired off a cleanup
					//and then the new text came in so fast that we got various crashes.
					//With this, we just schedule a cleanup, as a ui event handler, for
					//a moment in the future.  The interval is chosen to allow even a quick
					//backspace followed by typing, without wiping out the sense/example.
					if (_cleanupTimer == null)
					{
						_cleanupTimer = new Timer();
						_cleanupTimer.Tick += new EventHandler(OnCleanupTimer_Tick);
					 _cleanupTimer.Interval = 500;
					 }
					_cleanupTimer.Tag = entry;
					_cleanupTimer.Start();
					break;
			}
			RefreshLexicalEntryPreview();
		}

		void OnCleanupTimer_Tick(object sender, EventArgs e)
		{
			Reporting.Logger.WriteMinorEvent("OnCleanupTimer_Tick");
			LexEntry entry = (LexEntry)_cleanupTimer.Tag;
			_cleanupTimer.Stop();
			entry.CleanUpEmptyObjects();
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
		  this._detailListControl.VerticalScroll.Value = this._detailListControl.VerticalScroll.Minimum;
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
