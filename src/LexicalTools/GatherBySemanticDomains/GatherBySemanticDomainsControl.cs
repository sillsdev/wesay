using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeSay.UI;
using WeSay.UI.TextBoxes;

namespace WeSay.LexicalTools.GatherBySemanticDomains
{
	public partial class GatherBySemanticDomainsControl: UserControl
	{
		private readonly GatherBySemanticDomainTask _presentationModel;
		private bool _animationIsMovingFromList;

		public GatherBySemanticDomainsControl()
		{
			InitializeComponent();
			if (!DesignMode)
			{
				throw new InvalidOperationException("should only be called by Designer");
			}
		}

		public GatherBySemanticDomainsControl(GatherBySemanticDomainTask presentationModel)
		{
			_presentationModel = presentationModel;
			InitializeComponent();

			InitializeDisplaySettings();
			_listViewWords.WritingSystem = _presentationModel.WritingSystemUserIsTypingIn;
			RefreshCurrentWords();
			LoadDomainListCombo();
			RefreshCurrentDomainAndQuestion();
			bool showDescription = false;
			if (!showDescription)
			{
				_listViewWords.Anchor |= AnchorStyles.Top;
				_question.Anchor |= AnchorStyles.Top;
				_question.Anchor &= ~AnchorStyles.Bottom;
				_reminder.Anchor = _question.Anchor;
				_questionIndicator.Anchor |= AnchorStyles.Top;
				_questionIndicator.Anchor &= ~AnchorStyles.Bottom;

				int height = _question.Top - _description.Top;
				_question.Top -= height;
				_question.Height -= 5;
				_reminder.Top = _question.Bottom + 5;
				_questionIndicator.Top -= height;
				_listViewWords.Top -= height;
				_listViewWords.Height += height;

				_description.Visible = false;
			}

			//they have a border in the design view because otherwise they're hard to find
			_vernacularBox.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_vernacularBox.BackColor = Color.White;
			_meaningBox.CellBorderStyle = TableLayoutPanelCellBorderStyle.None;
			_meaningBox.BackColor = Color.White;

			_vernacularBox.WritingSystemsForThisField = new[]
				{
					_presentationModel.WritingSystemUserIsTypingIn
				};

			if( _vernacularBox.WritingSystemsForThisField.Count ==0 ||  _vernacularBox.TextBoxes.Count == 0)
			{
				Palaso.Reporting.ErrorReport.ReportFatalMessageWithStackTrace(String.Format("This task cannot be used with the audio/voice writing system '{0}'. Please use the config tool to specify a non-audio writing system for this task.", _presentationModel.WritingSystemUserIsTypingIn.Abbreviation));
			}

			//bit of a hack here... we make our own meaning box as a less intrusive way to add spell checking to
			//this box, which wasn't really designed to work well with auto-generated designer code.
			//so all this is to be able to turn IsSpellCheckingEnabled before the box is built.

			var meaning = new MultiTextControl(_presentationModel.ViewTemplate.WritingSystems, null)
				{
					IsSpellCheckingEnabled = true,
					ShowAnnotationWidget = false,
					WritingSystemsForThisField = new[] {_presentationModel.DefinitionWritingSystem},
					Visible = _presentationModel.ShowDefinitionField,
					Anchor = _meaningBox.Anchor,
					BackColor = _meaningBox.BackColor,
					AutoSize = _meaningBox.AutoSize,
					AutoSizeMode = _meaningBox.AutoSizeMode,
					Location = _meaningBox.Location,
					Size = _meaningBox.Size,
					TabIndex = _meaningBox.TabIndex
				};
			meaning.KeyDown  += _boxVernacularWord_KeyDown;
			tableLayoutPanel6.Controls.Remove(_meaningBox);
			tableLayoutPanel6.Controls.Add(meaning, 1, 1);
			_meaningBox = meaning;
		   _meaningLabel.Visible = _presentationModel.ShowDefinitionField;


			//  _listViewWords.ItemHeight = (int)Math.Ceiling(_presentationModel.WritingSystemUserIsTypingIn.Font.GetHeight());

			//    _animatedText.Font = _presentationModel.WritingSystemUserIsTypingIn.Font;

			_reminder.Text = _presentationModel.Reminder;

		   _flyingLabel.Font = _vernacularBox.TextBoxes[0].Font;

			_flyingLabel.Finished += _animator_Finished;

			_domainListComboBox.Font = _presentationModel.GetFontOfSemanticDomainField();

			//we'd like to have monospace, but I don't know for sure which languages these fonts will work
			//this is going to override the normal font choice they've made
			var majorRomanWritingSystems = new List<string>(new[] {"en", "id", "fr"});
			if(majorRomanWritingSystems.Contains(presentationModel.SemanticDomainWritingSystemId))
			{
#if MONO
				_domainListComboBox.Font = new Font("monospace", _domainListComboBox.Font.Size, FontStyle.Bold);
#else
				_domainListComboBox.Font = new Font("Lucida Console", _domainListComboBox.Font.Size, FontStyle.Bold);
#endif

			}
		}

		private void LoadDomainListCombo()
		{
			_domainListComboBox.Items.Clear();
			foreach (string domainName in _presentationModel.DomainNames)
			{
				_domainListComboBox.Items.Add(domainName);
			}
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
		}

		private void RefreshCurrentDomainAndQuestion()
		{
			//_domainName.Text = _presentationModel.CurrentDomainName;
			_domainListComboBox.SelectedIndex = _presentationModel.CurrentDomainIndex;
			_description.Text = _presentationModel.CurrentDomainDescription;
			_question.Text = _presentationModel.CurrentQuestion;
			_btnNext.Enabled = _presentationModel.CanGoToNext;
			_btnPrevious.Enabled = _presentationModel.HasPreviousDomainQuestion;
			_questionIndicator.Minimum = 1;
			_questionIndicator.Maximum = _presentationModel.Questions.Count;
			_questionIndicator.Value = _presentationModel.CurrentQuestionIndex + 1;
			_vernacularBox.ClearAllText();
			RefreshCurrentWords();
		}

		private void RefreshCurrentWords()
		{
			_listViewWords.Items.Clear();
			string longestWord = string.Empty;
			foreach (string word in _presentationModel.CurrentWords)
			{
				if (longestWord.Length < word.Length)
				{
					longestWord = word;
				}
				_listViewWords.Items.Add(word);
			}

			Size bounds = TextRenderer.MeasureText(longestWord, _listViewWords.Font);
			_listViewWords.ColumnWidth = bounds.Width + 10;
		}

		private void _btnNext_Click(object sender, EventArgs e)
		{
			//add in any existing word before going on
			if (!string.IsNullOrEmpty(WordToAdd))
			{
				_btnAddWord_Click(this, null);
			}
			_presentationModel.GotoNextDomainQuestion();
			RefreshCurrentDomainAndQuestion();
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		private void _btnPrevious_Click(object sender, EventArgs e)
		{
			_presentationModel.GotoPreviousDomainQuestion();
			RefreshCurrentDomainAndQuestion();
			_vernacularBox.FocusOnFirstWsAlternative();
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
					if (_btnPrevious.Enabled)
					{
						_btnPrevious_Click(this, null);
					}
					break;
				case Keys.PageDown:
					if (_btnNext.Enabled)
					{
						_btnNext_Click(this, null);
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
			//_listViewWords.BackColor = BackColor;
			_domainListComboBox.BackColor = BackColor;
			_description.BackColor = BackColor;
			_question.BackColor = BackColor;
			_reminder.BackColor = BackColor;
			_questionIndicator.BulletColor = ControlPaint.Light(BackColor);
			_questionIndicator.BulletColorEnd = ControlPaint.Dark(BackColor);
		}

		private void _listViewWords_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
			switch (e.KeyChar)
			{
				case '\r':
					OnListViewWords_Click(this, null);
					break;
				default:
					e.Handled = false;
					break;
			}
		}

		private void OnListViewWords_Click(object sender, EventArgs e)
		{
			if (_listViewWords.SelectedItem != null)
			{
				string word = (string) _listViewWords.SelectedItem;
				// NB: don't do this before storing what they clicked on.


				string wordCurrentlyInTheEditBox = WordToAdd;
				if (!String.IsNullOrEmpty(wordCurrentlyInTheEditBox))
				{
					_presentationModel.AddWord(wordCurrentlyInTheEditBox, MeaningToAdd);
					//don't throw away what they were typing
				}


				_presentationModel.PrepareToMoveWordToEditArea(word);

				Point absolutePosition = GetAbsoluteLocationOfControl(_vernacularBox);
				Point destination = absolutePosition;
				Point start = _listViewWords.GetItemRectangle(_listViewWords.SelectedIndex).Location;
				start.Offset(GetAbsoluteLocationOfControl(_listViewWords));

				RefreshCurrentWords();
				_animationIsMovingFromList = true;
				if (_meaningBox.Visible)
				{
					_meaningBox.ClearAllText();
					_meaningBox.SetMultiText(_presentationModel.GetMeaningForWordRecentlyMovedToEditArea());
				}
				_flyingLabel.Go(word, start, destination);

			}
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		private void _animator_Finished(object sender, EventArgs e)
		{
			if (_animationIsMovingFromList)
			{
				_vernacularBox.TextBoxes[0].Text = _flyingLabel.Text;
			}

			_listViewWords.ItemToNotDrawYet = null;
		}

		private static Point GetAbsoluteLocationOfControl(Control controlToLocate)
		{
			Control currentcontrolInHierarchy = controlToLocate;
			Point absolutePosition = currentcontrolInHierarchy.Location;
			while(currentcontrolInHierarchy.Parent != null)
			{
				currentcontrolInHierarchy = currentcontrolInHierarchy.Parent;
				absolutePosition = absolutePosition + (Size) currentcontrolInHierarchy.Location;
			}
			return absolutePosition;
		}

		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			string word = WordToAdd;
			if (String.IsNullOrEmpty(word))
			{
				_vernacularBox.FocusOnFirstWsAlternative();
				return;
			}
			_presentationModel.AddWord(word, MeaningToAdd);
			_vernacularBox.ClearAllText();
			_meaningBox.ClearAllText();

			_listViewWords.ItemToNotDrawYet = word;
			RefreshCurrentWords();

			int index = _listViewWords.FindStringExact(word);

			Point start = GetAbsoluteLocationOfControl(_vernacularBox);
			Point destination = _listViewWords.GetItemRectangle(index).Location;
			destination.Offset(GetAbsoluteLocationOfControl(_listViewWords));

			_flyingLabel.Text = word;
			_animationIsMovingFromList = false;

			_flyingLabel.Go(word, start, destination);
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		private string WordToAdd
		{
			get { return _vernacularBox.TextBoxes[0].Text.Trim(); }
		}

		private string MeaningToAdd
		{
			get { return _meaningBox.TextBoxes[0].Text.Trim(); }
		}


		private void _domainName_DrawItem(object sender, DrawItemEventArgs e)
		{
			if ((e.State & DrawItemState.ComboBoxEdit) == DrawItemState.ComboBoxEdit)
			{
				if (e.Index >= 0)
				{
					TextRenderer.DrawText(e.Graphics,
										  _presentationModel.DomainNames[e.Index],
										  e.Font,
										  e.Bounds,
										  e.ForeColor,
										  e.BackColor,
										  TextFormatFlags.Left);
				}
			}
			else
			{
				TextRenderer.DrawText(e.Graphics,
									  DomainNameAndCount(e.Index),
									  e.Font,
									  e.Bounds,
									  e.ForeColor,
									  e.BackColor,
									  TextFormatFlags.Left);
			}
		}

		private void _domainName_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			Size size = TextRenderer.MeasureText(DomainNameAndCount(e.Index), _domainListComboBox.Font);
			e.ItemHeight = size.Height;
			e.ItemWidth = size.Width;
		}

		private string DomainNameAndCount(int index)
		{
			if (index == -1)
			{
				return string.Empty;
			}

			int count = _presentationModel.WordsInDomain(index);
			string s;
			if (count > 0)
			{
				s = "(" + count + ") ";
			}
			else
			{
				s = "    ";
			}
			return s + _presentationModel.DomainNames[index];
		}

		private void _domainName_SelectedIndexChanged(object sender, EventArgs e)
		{
			_presentationModel.CurrentDomainIndex = _domainListComboBox.SelectedIndex;
			RefreshCurrentDomainAndQuestion();
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		public void Cleanup()
		{
			_btnAddWord_Click(this, null);
		}

	}
}
