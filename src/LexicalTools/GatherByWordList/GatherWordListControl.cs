using System;
using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms;
using Palaso.Data;
using Palaso.DictionaryServices.Model;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.GatherByWordList
{
	public partial class GatherWordListControl: UserControl
	{
		private readonly GatherWordListTask _task;

		//private System.Windows.Forms.Label _animatedText= new Label();
		private bool _animationIsMovingFromList;
		private bool _settingIndexInCode;

		public GatherWordListControl()
		{
			Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public GatherWordListControl(GatherWordListTask task, IWritingSystemDefinition lexicalUnitWritingSystem)
		{
			_task = task;

			InitializeComponent();
			InitializeDisplaySettings();
			_vernacularBox.AutoSizeMode = AutoSizeMode.GrowAndShrink;
			_vernacularBox.WritingSystemsForThisField = new IWritingSystemDefinition[]
															{lexicalUnitWritingSystem};
			_vernacularBox.TextChanged += _vernacularBox_TextChanged;
			_vernacularBox.KeyDown += _boxVernacularWord_KeyDown;
			_vernacularBox.MinimumSize = new Size(_boxForeignWord.Size.Width - 25, _boxForeignWord.Size.Height);

			_listViewOfWordsMatchingCurrentItem.UserClick += new System.EventHandler(this.OnListViewOfWordsMatchingCurrentItem_Click);
			_listViewOfWordsMatchingCurrentItem.Clear();
			_listViewOfWordsMatchingCurrentItem.FormWritingSystem = lexicalUnitWritingSystem;

			//  _listViewOfWordsMatchingCurrentItem.ItemHeight = (int)Math.Ceiling(_task.FormWritingSystem.Font.GetHeight());



			_verticalWordListView.WritingSystem = task.PromptingWritingSystem;
			_verticalWordListView.MaxLength = 18;
			_verticalWordListView.MinLength = 10;  // Space fill to this length
			_verticalWordListView.BackColor = Color.White;
			_verticalWordListView.DataSource = task.Words;
			UpdateStuff();
			_verticalWordListView.ItemSelectionChanged += OnWordsList_SelectedIndexChanged;

			_flyingLabel.Font = _vernacularBox.TextBoxes[0].Font;
			_flyingLabel.Finished += OnAnimator_Finished;
		}

		private void OnWordsList_SelectedIndexChanged(object sender, ListViewItemSelectionChangedEventArgs e)
		{
			if (_settingIndexInCode)
				return;

			if(_verticalWordListView.SelectedIndex==-1)
				return;

			AddCurrentWord(); //don't throw away what they were typing
			_task.NavigateToIndex(_verticalWordListView.SelectedIndex);
			UpdateSourceWord();
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
		}


		private void OnAnimator_Finished(object sender, EventArgs e)
		{
			if (_animationIsMovingFromList)
			{
				_vernacularBox.TextBoxes[0].Text = _flyingLabel.Text;
			}
			var box = _vernacularBox.TextBoxes[0];
			box.Focus();
			if(box is IWeSayTextBox)
					((IWeSayTextBox) box).SelectionStart = 1000; //go to end
		}

		private void UpdateSourceWord()
		{
			UpdateStuff();
			_vernacularBox.ClearAllText();
			_vernacularBox.TextBoxes[0].Focus();
		}

		private void _vernacularBox_TextChanged(object sender, EventArgs e)
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
			if (!string.IsNullOrEmpty(_task.LoadFailureMessage))
			{
				_congratulationsControl.Show(_task.LoadFailureMessage);
			}
			else if (_task.IsTaskComplete)
			{
				_congratulationsControl.Show("Congratulations. You have completed this task.");
			}
			else
			{
				_congratulationsControl.Hide();
				Debug.Assert(_vernacularBox.TextBoxes.Count == 1,
							 "other code here (for now), assumes exactly one ws/text box");
				_boxForeignWord.Text = _task.CurrentPromptingForm;

				PopulateWordsMatchingCurrentItem();

				try //this is a late-added feature (2012), so we don't want to destabilize things
				{
					_settingIndexInCode = true;
					_verticalWordListView.SelectedIndex = _task.CurrentIndexIntoWordlist;
					_settingIndexInCode = false;
				}
				catch (Exception)
				{
#if DEBUG
					throw;
#endif
				}

			}
			UpdateEnabledStates();
		}

		private void UpdateEnabledStates()
		{
			_btnAddWord.Enabled = !_task.IsTaskComplete &&
								  _vernacularBox.TextBoxes[0].Text.Trim() != "";
			_btnNextWord.Enabled = _task.CanNavigateNext;
			//_btnPreviousWord.Enabled = _task.CanNavigatePrevious;
		}

		/// <summary>
		/// We want to show all words in the lexicon which match the current
		/// wordlist item.
		/// </summary>
		private void PopulateWordsMatchingCurrentItem()
		{
			_listViewOfWordsMatchingCurrentItem.Clear();
			string longestWord = string.Empty;
			foreach (RecordToken<LexEntry> recordToken in _task.GetRecordsWithMatchingGloss())
			{
				var recordTokenToStringAdapter = new RecordTokenToStringAdapter<LexEntry>("Form", recordToken);
				string word = recordTokenToStringAdapter.ToString();
				if(!string.IsNullOrEmpty(word))
				{
					if (longestWord.Length < word.Length)
					{
						longestWord = word;
					}
					_listViewOfWordsMatchingCurrentItem.AddItem(recordTokenToStringAdapter);
				}
				else
				{
					//The matching gloss/def is there, but the lexeme form is empty. So just don't put it in the list
				}
			}
			Size wordMax = TextRenderer.MeasureText(longestWord, _listViewOfWordsMatchingCurrentItem.Font);
			_listViewOfWordsMatchingCurrentItem.ColumnWidth = wordMax.Width + 10;
			_listViewOfWordsMatchingCurrentItem.ListCompleted();
		}

		private void _btnNextWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord(); //don't throw away what they were typing
			_task.NavigateNext();
			UpdateSourceWord();
		}

		private void _btnPreviousWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord(); //don't throw away what they were typing
			_task.NavigatePrevious();
			UpdateSourceWord();
		}

		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			AddCurrentWord();
		}

		private void AddCurrentWord()
		{
			Debug.Assert(_vernacularBox.TextBoxes.Count == 1,
						 "other code here (for now), assumes exactly one ws/text box");
			string s = _vernacularBox.TextBoxes[0].Text.Trim();
			if (s == "")
			{
				return;
			}
			_task.WordCollected(_vernacularBox.GetMultiText());

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
					if (_btnAddWord.Enabled)
					{
						_btnAddWord_Click(this, null);
					}
					break;
				case Keys.PageUp:
					if ( _task.CanNavigatePrevious)
					{
						_btnPreviousWord_Click(this, null);
					}
					break;
				case Keys.PageDown:
					if (_btnNextWord.Enabled)
					{
						_btnNextWord_Click(this, null);
					}
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
			if (_listViewOfWordsMatchingCurrentItem.SelectedItem != null)
			{
				int selectedListIndex = _listViewOfWordsMatchingCurrentItem.SelectedIndex;
				string word = _listViewOfWordsMatchingCurrentItem.SelectedItem.ToString();

				RecordToken<LexEntry> recordToken =
					((RecordTokenToStringAdapter<LexEntry>) (_listViewOfWordsMatchingCurrentItem.SelectedItem)).AdaptedRecordToken;
				Point start =
						_listViewOfWordsMatchingCurrentItem.GetItemRectangle(selectedListIndex).
								Location;
				start.Offset(_listViewOfWordsMatchingCurrentItem.Location);
				Point destination = _vernacularBox.Location;
				destination.Offset(_vernacularBox.TextBoxes[0].Location);

				// NB: don't do this before storing what they clicked on.
				AddCurrentWord(); //don't throw away what they were typing

				_task.TryToRemoveAssociationWithListWordFromEntry(recordToken);

				// _movingLabel.Go(word,_listViewOfWordsMatchingCurrentItem.GetItemRect(selectedListIndex).Location, _vernacularBox.Location)

				UpdateStuff();
				// _vernacularBox.TextBoxes[0].Text = word;

				_animationIsMovingFromList = true;
				_flyingLabel.Go(word, start, destination);
			}
		}

		public void Cleanup()
		{
			AddCurrentWord();
		}

		public void SelectInitialControl()
		{
			_vernacularBox.FocusOnFirstWsAlternative();
		}
	}
}
