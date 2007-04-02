using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.LexicalTools
{
	public partial class GatherBySemanticDomainsControl : UserControl
	{
		private GatherBySemanticDomainTask _presentationModel;

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
			_vernacularBox.WritingSystems = new WritingSystem[] {_presentationModel.WordWritingSystem};
		}

		private void InitializeDisplaySettings() {
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
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
			_questionIndicator.Value = _presentationModel.CurrentQuestionIndex+1;
			RefreshCurrentWords();
		}

		private void RefreshCurrentWords() {
			this._listViewWords.Items.Clear();

			foreach (string word in this._presentationModel.CurrentWords)
			{
				this._listViewWords.Items.Add(word);
			}
		}

		private void _btnNext_Click(object sender, EventArgs e)
		{
			_presentationModel.GotoNextDomainQuestion();
			RefreshCurrentDomainAndQuestion();
		}

		private void _btnPrevious_Click(object sender, EventArgs e)
		{
			_presentationModel.GotoPreviousDomainQuestion();
			RefreshCurrentDomainAndQuestion();
		}

		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			_presentationModel.AddWord(_vernacularBox.TextBoxes[0].Text);
			_vernacularBox.ClearAllText();
			RefreshCurrentWords();
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
					if(_btnPrevious.Enabled)
						_btnPrevious_Click(this, null);
					break;
				case Keys.PageDown:
					if(_btnNext.Enabled)
					_btnNext_Click(this, null);
					break;

				default:
					e.Handled = false;
					if (Environment.OSVersion.Platform != PlatformID.Unix)
					{
						SetSuppressKeyPress(e, false);
					}
					break;
			}
		}

		private static void SetSuppressKeyPress(KeyEventArgs e, bool suppress)
		{
			e.SuppressKeyPress = suppress;
		}

		private void GatherWordListControl_BackColorChanged(object sender, EventArgs e)
		{
			//_listViewWords.BackColor = BackColor;
			_domainName.BackColor = BackColor;
			_description.BackColor = BackColor;
			_question.BackColor = BackColor;
		}

		void _listViewWords_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
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

		void _listViewWords_Click(object sender, System.EventArgs e)
		{
			if(_listViewWords.SelectedItem != null)
			{
				string word = (string) _listViewWords.SelectedItem;
				_presentationModel.RemoveWord(word);
				_vernacularBox.TextBoxes[0].Text = word;
				RefreshCurrentWords();
			}
		}

		void _domainName_DrawItem(object sender, DrawItemEventArgs e)
		{
			if (e.State ==  DrawItemState.ComboBoxEdit)
			{
				if (e.Index >= 0)
				{
					TextRenderer.DrawText(e.Graphics,
										  this._presentationModel.DomainNames[e.Index],
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
		void _domainName_MeasureItem(object sender, MeasureItemEventArgs e)
		{
			Size size = TextRenderer.MeasureText(DomainNameAndCount(e.Index), _domainName.Font);
			e.ItemHeight = size.Height;
			e.ItemWidth = size.Width;
		}

		private string DomainNameAndCount(int index)
		{
			if(index == -1)
			{
				return string.Empty;
			}
			return "(" + this._presentationModel.WordsInDomain(index) + ") " +
				this._presentationModel.DomainNames[index];
		}

		void _domainName_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			_presentationModel.CurrentDomainIndex = _domainName.SelectedIndex;
			RefreshCurrentDomainAndQuestion();
		}

	}
}
