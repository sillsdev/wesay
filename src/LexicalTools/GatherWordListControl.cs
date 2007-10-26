using System;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;

namespace WeSay.LexicalTools
{
	public partial class GatherWordListControl : UserControl
	{
		private readonly GatherWordListTask _task;

		public GatherWordListControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public GatherWordListControl(GatherWordListTask task,
									 ViewTemplate viewTemplate)
		{
			_task = task;
			_task.UpdateSourceWord += new EventHandler(OnUpdateSourceWord);

			InitializeComponent();
			InitializeDisplaySettings();
			if (Type.GetType("Mono.Runtime") == null) // Work around not yet implemented in Mono
			{
				SetAutoSizeToGrowAndShrink();
			}

			_listViewOfWordsMatchingCurrentItem.Items.Clear();

			Field lexicalFormField = viewTemplate.GetField(Field.FieldNames.EntryLexicalForm.ToString());
			if (lexicalFormField == null || lexicalFormField.WritingSystems.Count < 1)
			{
				_vernacularBox.WritingSystemsForThisField = new WritingSystem[] { BasilProject.Project.WritingSystems.UnknownVernacularWritingSystem };
			}
			else
			{
				_vernacularBox.WritingSystemsForThisField = new WritingSystem[] { lexicalFormField.WritingSystems[0] };
			}
//            _vernacularBox.BackColor = System.Drawing.Color.Red;
			_vernacularBox.TextChanged += new EventHandler(_vernacularBox_TextChanged);
			_vernacularBox.KeyDown += new KeyEventHandler(_boxVernacularWord_KeyDown);
			_vernacularBox.MinimumSize = this._boxForeignWord.Size;
			UpdateStuff();
		}

		private void InitializeDisplaySettings()
		{
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
		}


		void OnUpdateSourceWord(object sender, EventArgs e)
		{
			UpdateStuff();
			_vernacularBox.ClearAllText();
			_vernacularBox.TextBoxes[0].Focus();
		}

		void _vernacularBox_TextChanged(object sender, EventArgs e)
		{
			UpdateEnabledStates();
		}

		private void GatherWordListControl_Load(object sender, EventArgs e)
		{
			_task.NavigateFirstToShow();
		}

		private void UpdateStuff()
		{
			if (DesignMode)
			{
				return;
			}
			if (_task.IsTaskComplete)
			{
				_congratulationsControl.Show("Congratulations. You have completed this task.");
			}
			else
			{
				_congratulationsControl.Hide();
				Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
				_boxForeignWord.Text = _task.CurrentWord;
				PopulateWordsMatchingCurrentItem();
			}
			UpdateEnabledStates();

	   }

		private void UpdateEnabledStates()
		{
			_btnAddWord.Enabled = !_task.IsTaskComplete && _vernacularBox.TextBoxes[0].Text.Trim() != "";
			_btnNextWord.Enabled = _task.CanNavigateNext;
			_btnPreviousWord.Enabled = _task.CanNavigatePrevious;
		}

		/// <summary>
		/// We want to show all words in the lexicon which match the current
		/// gloss.
		/// </summary>
		private void PopulateWordsMatchingCurrentItem()
		{
			_listViewOfWordsMatchingCurrentItem.Items.Clear();

			foreach (LexEntry entry in _task.GetMatchingRecords(_task.CurrentWordAsMultiText))
			{
				string alternative = entry.LexicalForm.GetFirstAlternative();
				ListViewItem item = new ListViewItem(alternative);
				item.Tag = entry;
				_listViewOfWordsMatchingCurrentItem.Items.Add(item);
			}
		}

		private void _btnNextWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord();//don't throw away what they were typing
			_task.NavigateNext();
		}
		private void _btnPreviousWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord();//don't throw away what they were typing
			_task.NavigatePrevious();
		}



		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord();
		}

		private void AddCurrentWord()
		{
			Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
			string s = _vernacularBox.TextBoxes[0].Text.Trim();
			if(s == "")
			{
				return;
			}
			_task.WordCollected(_vernacularBox.MultiText);

			//_listViewOfWordsMatchingCurrentItem.Items.Add(s);
			_vernacularBox.TextBoxes[0].Text = "";
			UpdateStuff();
		}

		private void _boxVernacularWord_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				SetSuppressKeyPress(e, true);
			}
			switch (e.KeyCode)
			{
				case Keys.Return:
					if(_btnAddWord.Enabled)
						_btnAddWord_Click(this, null);
					break;
				case Keys.PageUp:
					if(_btnPreviousWord.Enabled)
						_btnPreviousWord_Click(this, null);
					break;
				case Keys.PageDown:
					if(_btnNextWord.Enabled)
					_btnNextWord_Click(this, null);
					break;

				default:
					e.Handled = false;
					if (Type.GetType("Mono.Runtime") == null) // Work around not yet implemented in Mono
					{
						SetSuppressKeyPress(e, false);
					}
					break;
			}
		}
		private static void SetSuppressKeyPress(KeyEventArgs e, bool suppress)
		{
#if !MONO
			e.SuppressKeyPress = suppress;
#endif
		}

		private void GatherWordListControl_BackColorChanged(object sender, EventArgs e)
		{
			_listViewOfWordsMatchingCurrentItem.BackColor = BackColor;
			_boxForeignWord.BackColor = BackColor;
		}

		private void SetAutoSizeToGrowAndShrink()
		{
#if !MONO
			this._vernacularBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
#endif
		}

		private void _listViewOfWordsMatchingCurrentItem_Click(object sender, EventArgs e)
		{
			if (_listViewOfWordsMatchingCurrentItem.SelectedItems.Count > 0)
			{

				string word = _listViewOfWordsMatchingCurrentItem.SelectedItems[0].Text;
				LexEntry entry = _listViewOfWordsMatchingCurrentItem.SelectedItems[0].Tag as LexEntry;
				Debug.Assert(entry!=null);
				if(entry==null)
				{
					return;
				}

								 // NB: don't do this before storing what they clicked on.
			   AddCurrentWord();//don't throw away what they were typing

				_task.TryToRemoveAssociationWithListWordFromEntry(entry);

//                this.destination = this._vernacularBox.Location;
//                this.destination.X += this._vernacularBox.TextBoxes[0].Location.X;
//                this.destination.Y += this._vernacularBox.TextBoxes[0].Location.Y;
//                this.start = this._listViewOfWordsMatchingCurrentItem.GetItemRectangle(_listViewOfWordsMatchingCurrentItem.SelectedIndex).Location;
//                this.start.X += this._listViewOfWordsMatchingCurrentItem.Location.X;
//                this.start.Y += this._listViewOfWordsMatchingCurrentItem.Location.Y;
//
//                _animatedText.Text = (string)this._listViewWords.SelectedItem;
//                _animatedText.Location = start;
//                _animatedText.Visible = true;
//
				 UpdateStuff();
				 _vernacularBox.TextBoxes[0].Text = word;

//                _addingWordAnimation = false;
//                this._animator.Start();
			}
		}

	}
}
