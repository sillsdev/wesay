using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.Language;
using WeSay.LexicalModel;
using WeSay.Project;
using WeSay.UI;


namespace WeSay.LexicalTools
{
	public partial class GatherWordListControl : UserControl
	{
		private readonly GatherWordListTask _task;

		//private System.Windows.Forms.Label _animatedText= new Label();
		private bool _animationIsMovingFromList;
		public GatherWordListControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public GatherWordListControl(GatherWordListTask task,
									 ViewTemplate viewTemplate)
		{
			_task = task;

			InitializeComponent();
			InitializeDisplaySettings();
			this._vernacularBox.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;

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
			_vernacularBox.TextChanged += new EventHandler(_vernacularBox_TextChanged);
			_vernacularBox.KeyDown += new KeyEventHandler(_boxVernacularWord_KeyDown);
			_vernacularBox.MinimumSize = this._boxForeignWord.Size;

			_listViewOfWordsMatchingCurrentItem.WritingSystem = _task.WordWritingSystem;
		  //  _listViewOfWordsMatchingCurrentItem.ItemHeight = (int)Math.Ceiling(_task.WordWritingSystem.Font.GetHeight());

			UpdateStuff();

			_movingLabel.Font = _vernacularBox.TextBoxes[0].Font;
			_movingLabel.Finished += new EventHandler(OnAnimator_Finished);
		}

		private void InitializeDisplaySettings()
		{
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
		}


		private void OnAnimator_Finished(object sender, EventArgs e)
		{
			if (_animationIsMovingFromList)
			{
				_vernacularBox.TextBoxes[0].Text = _movingLabel.Text;
			}
			_vernacularBox.TextBoxes[0].Focus();
			_vernacularBox.TextBoxes[0].SelectionStart = 1000; //go to end
		}

		void UpdateSourceWord()
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
			UpdateSourceWord();
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
		/// wordlist item.
		/// </summary>
		private void PopulateWordsMatchingCurrentItem()
		{
			_listViewOfWordsMatchingCurrentItem.Items.Clear();

			foreach (LexEntry entry in _task.GetMatchingRecords(_task.CurrentWordAsMultiText))
			{
				//string alternative = entry.LexicalForm.GetFirstAlternative();
			   // ListViewItem item = new ListViewItem(alternative);
				//item.Tag = entry;
				_listViewOfWordsMatchingCurrentItem.Items.Add(new EntryDisplayProxy(entry, _task.WordWritingSystem.Id));
			}
		}

		private void _btnNextWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord();//don't throw away what they were typing
			_task.NavigateNext();
			UpdateSourceWord();
		}
		private void _btnPreviousWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord();//don't throw away what they were typing
			_task.NavigatePrevious();
			UpdateSourceWord();
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
			_vernacularBox.TextBoxes[0].Focus();

		}

		private void _boxVernacularWord_KeyDown(object sender, KeyEventArgs e)
		{
			e.Handled = true;
			e.SuppressKeyPress = true;
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
					e.SuppressKeyPress = false;
					break;
			}
		}

		private void GatherWordListControl_BackColorChanged(object sender, EventArgs e)
		{
			_listViewOfWordsMatchingCurrentItem.BackColor = BackColor;
			_boxForeignWord.BackColor = BackColor;
		}

		private void OnListViewOfWordsMatchingCurrentItem_Click(object sender, EventArgs e)
		{
			if (_listViewOfWordsMatchingCurrentItem.SelectedItems.Count > 0)
			{
				int selectedListIndex = _listViewOfWordsMatchingCurrentItem.SelectedIndices[0];
				string word = _listViewOfWordsMatchingCurrentItem.SelectedItem.ToString();
				LexEntry entry = ((EntryDisplayProxy)_listViewOfWordsMatchingCurrentItem.SelectedItem).Entry;
				Debug.Assert(entry!=null);
				if(entry==null)
				{
					return;
				}
				Point start = _listViewOfWordsMatchingCurrentItem.GetItemRectangle(selectedListIndex).Location;
				start.Offset(_listViewOfWordsMatchingCurrentItem.Location);
				Point destination = _vernacularBox.Location;
				destination.Offset(_vernacularBox.TextBoxes[0].Location);

								 // NB: don't do this before storing what they clicked on.
			   AddCurrentWord();//don't throw away what they were typing

				_task.TryToRemoveAssociationWithListWordFromEntry(entry);

			   // _movingLabel.Go(word,_listViewOfWordsMatchingCurrentItem.GetItemRect(selectedListIndex).Location, _vernacularBox.Location)

				 UpdateStuff();
				// _vernacularBox.TextBoxes[0].Text = word;

				_animationIsMovingFromList = true;
				_movingLabel.Go(word,
								start,
								destination);
			}
		}

	}
}
