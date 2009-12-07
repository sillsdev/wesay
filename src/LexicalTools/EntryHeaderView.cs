using System;
using System.Drawing;
using System.Windows.Forms;
using Chorus;
using Chorus.UI.Notes;
using Palaso.DictionaryServices.Model;
using WeSay.LexicalModel;
using WeSay.UI;

namespace WeSay.LexicalTools
{
	public partial class EntryHeaderView : UserControl
	{
		private const int kNotesBarHeight = 30;//we want 16pixel icons
		//autofac generates a factory which comes up with all the other needed parameters from its container
		public delegate EntryHeaderView Factory();

		private NotesBarView _notesBar;
		private LexEntry _previousRecord=null;

		/// <summary>
		/// designer only
		/// </summary>
		public EntryHeaderView()
		{
			InitializeComponent();
		}

		public EntryHeaderView(ChorusNotesSystem notesSystem)
		{
			InitializeComponent();

			_notesBar = notesSystem.CreateNotesBarView();
			_notesBar.BorderStyle = System.Windows.Forms.BorderStyle.None;
			_notesBar.Dock = System.Windows.Forms.DockStyle.Top;
			_notesBar.Location = new System.Drawing.Point(0, 0);
			_notesBar.Name = "notesBar";
			_notesBar.BackColor = this.BackColor;
			//notesBar.Size = new System.Drawing.Size(474, 85);
			_notesBar.TabIndex = 1;
			_notesBar.TabStop = false;
			//_notesBar.Visible = false;//wait until we have a record to show
			_notesBar.Height = kNotesBarHeight;
			this.Controls.Add(_notesBar);
			Controls.SetChildIndex(_notesBar, 0);
			_notesBar.SizeChanged += new EventHandler(_notesBar_SizeChanged);
			DoLayout();
		}

		void _notesBar_SizeChanged(object sender, EventArgs e)
		{
			_notesBar.Height = kNotesBarHeight;
		}

		public string RtfForTests
		{
			get { return this._entryPreview.Rtf; }
		}

		public string TextForTests
		{
			get { return this._entryPreview.Text; }
		}

		private void EntryHeaderView_Load(object sender, EventArgs e)
		{

		}

		public void UpdateContents(LexEntry record, CurrentItemEventArgs currentItemInFocus, LexEntryRepository lexEntryRepository)
		{
			if (record == null)
			{
				_entryPreview.Rtf = string.Empty;
			}
			else
			{
				_entryPreview.Rtf = RtfRenderer.ToRtf(record,
													  currentItemInFocus,
													  lexEntryRepository);
			}

			if (record != _previousRecord)
			{
				_previousRecord = record;
				if (_notesBar == null)
					return;
				if (record == null)
				{
					_notesBar.SetIdOfCurrentAnnotatedObject(string.Empty);
				}
				else
				{
					_notesBar.SetIdOfCurrentAnnotatedObject(record.Guid.ToString());
				}
			}

			//_notesBar.Visible = true;
		}

		private void EntryHeaderView_BackColorChanged(object sender, EventArgs e)
		{
			_entryPreview.BackColor = BackColor;
			if (_notesBar == null)
				return;
			_notesBar.BackColor = BackColor;
		}

		private void EntryHeaderView_SizeChanged(object sender, EventArgs e)
		{
			if (_notesBar == null)
				return;
			DoLayout();
		}

		private void DoLayout()
		{
			if (_notesBar == null)
				return;
			_notesBar.Location=new Point(0, Height-_notesBar.Height);
			int height = Height - _notesBar.Height;
			_entryPreview.Visible = (height > 20);
			_entryPreview.Height = height;
		}
	}
}
