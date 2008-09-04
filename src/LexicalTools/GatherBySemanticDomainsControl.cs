using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;
using WeSay.UI;

namespace WeSay.LexicalTools
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
			RefreshCurrentWords();
			_domainName.Items.Clear();
			foreach (string domainName in _presentationModel.DomainNames)
			{
				_domainName.Items.Add(domainName);
			}
			RefreshCurrentDomainAndQuestion();
			bool showDescription = false;
			if (!showDescription)
			{
				_listViewWords.Anchor |= AnchorStyles.Top;
				_question.Anchor |= AnchorStyles.Top;
				_question.Anchor &= ~AnchorStyles.Bottom;
				_questionIndicator.Anchor |= AnchorStyles.Top;
				_questionIndicator.Anchor &= ~AnchorStyles.Bottom;

				int height = _question.Top - _description.Top;
				_question.Top -= height;
				_question.Height -= 5;
				_questionIndicator.Top -= height;
				_listViewWords.Top -= height;
				_listViewWords.Height += height;

				_description.Visible = false;
			}
			_vernacularBox.WritingSystemsForThisField = new WritingSystem[]
															{_presentationModel.WordWritingSystem};
			_listViewWords.WritingSystem = _presentationModel.WordWritingSystem;
			//  _listViewWords.ItemHeight = (int)Math.Ceiling(_presentationModel.WordWritingSystem.Font.GetHeight());

			//    _animatedText.Font = _presentationModel.WordWritingSystem.Font;

			_movingLabel.Font = _vernacularBox.TextBoxes[0].Font;
			_movingLabel.Finished += _animator_Finished;
		}

		private void InitializeDisplaySettings()
		{
			BackColor = DisplaySettings.Default.BackgroundColor;
		}

		private void RefreshCurrentDomainAndQuestion()
		{
			//_domainName.Text = _presentationModel.CurrentDomainName;
			_domainName.SelectedIndex = _presentationModel.CurrentDomainIndex;
			_description.Text = _presentationModel.CurrentDomainDescription;
			_question.Text = _presentationModel.CurrentQuestion;
			_btnNext.Enabled = _presentationModel.HasNextDomainQuestion;
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
			_domainName.BackColor = BackColor;
			_description.BackColor = BackColor;
			_question.BackColor = BackColor;
			_questionIndicator.BulletColor = ControlPaint.Light(BackColor);
			_questionIndicator.BulletColorEnd = ControlPaint.Dark(BackColor);
		}

		private void _listViewWords_KeyPress(object sender, KeyPressEventArgs e)
		{
			e.Handled = true;
			switch (e.KeyChar)
			{
				case '\r':
					_listViewWords_Click(this, null);
					break;
				default:
					e.Handled = false;
					break;
			}
		}

		private void _listViewWords_Click(object sender, EventArgs e)
		{
			if (_listViewWords.SelectedItem != null)
			{
				string word = (string) _listViewWords.SelectedItem;
				// NB: don't do this before storing what they clicked on.

				string wordCurrentlyInTheEditBox = WordToAdd;
				if (!String.IsNullOrEmpty(wordCurrentlyInTheEditBox))
				{
					_presentationModel.AddWord(wordCurrentlyInTheEditBox);
					//don't throw away what they were typing
				}

				_presentationModel.DetachFromMatchingEntries(word);

				Point destination = _vernacularBox.Location;
				destination.Offset(_vernacularBox.TextBoxes[0].Location);
				Point start = _listViewWords.GetItemRectangle(_listViewWords.SelectedIndex).Location;
				start.Offset(_listViewWords.Location);

				RefreshCurrentWords();
				_animationIsMovingFromList = false;

				_movingLabel.Go(word, start, destination);
			}
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			string word = WordToAdd;
			if (String.IsNullOrEmpty(word))
			{
				_vernacularBox.FocusOnFirstWsAlternative();
				return;
			}
			_presentationModel.AddWord(word);
			_vernacularBox.ClearAllText();

			_listViewWords.ItemToNotDrawYet = word;
			RefreshCurrentWords();

			int index = _listViewWords.FindStringExact(word);

			Point start = _vernacularBox.Location;
			start.Offset(_vernacularBox.TextBoxes[0].Location);
			Point destination = _listViewWords.GetItemRectangle(index).Location;
			destination.Offset(_listViewWords.Location);

			_movingLabel.Text = word;
			_animationIsMovingFromList = true;

			_movingLabel.Go(word, start, destination);
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		private string WordToAdd
		{
			get { return _vernacularBox.TextBoxes[0].Text.Trim(); }
		}

		private void _animator_Finished(object sender, EventArgs e)
		{
			if (!_animationIsMovingFromList)
			{
				_vernacularBox.TextBoxes[0].Text = _movingLabel.Text;
			}

			_listViewWords.ItemToNotDrawYet = null;
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
			Size size = TextRenderer.MeasureText(DomainNameAndCount(e.Index), _domainName.Font);
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
			_presentationModel.CurrentDomainIndex = _domainName.SelectedIndex;
			RefreshCurrentDomainAndQuestion();
			_vernacularBox.FocusOnFirstWsAlternative();
		}

		public void Cleanup()
		{
			_btnAddWord_Click(this, null);
		}
	}
}