using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Forms;
using WeSay.Data;
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
			System.Diagnostics.Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public GatherWordListControl(GatherWordListTask task)
		{
			_task = task;
			_task.UpdateSourceWord += new EventHandler(OnUpdateSourceWord);

			InitializeComponent();
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_listViewOfWordsMatchingCurrentItem.Items.Clear();
			//TODO: this limits us to a single writing system, and relies on the deprecated "default"
			_vernacularBox.WritingSystems = new WritingSystem[] { BasilProject.Project.WritingSystems.VernacularWritingSystemDefault };
			_vernacularBox.TextChanged += new EventHandler(_vernacularBox_TextChanged);
			_vernacularBox.KeyDown += new KeyEventHandler(_boxVernacularWord_KeyDown);
			UpdateStuff();
		}

		void OnUpdateSourceWord(object sender, EventArgs e)
		{
			UpdateStuff();
			_vernacularBox.ClearAllText();
			_vernacularBox.FlagIsOn = false;
			_vernacularBox.TextBoxes[0].Focus();
		}

		void _vernacularBox_TextChanged(object sender, EventArgs e)
		{
			UpdateStuff();
		}

		private void GatherWordListControl_Load(object sender, EventArgs e)
		{
			_task.NavigateFirst();
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
				_btnAddWord.Enabled = false;
			}
			else
			{
				_congratulationsControl.Hide();
				Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
				_boxForeignWord.Text = _task.CurrentWord;
				PopulateWordsMatchingCurrentItem();
				_btnAddWord.Enabled = _vernacularBox.TextBoxes[0].Text.Trim() != "";
			}
			_btnNextWord.Enabled = _task.NavigateNextEnabled;
			_btnPreviousWord.Enabled = _task.NavigatePreviousEnabled;

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
				_listViewOfWordsMatchingCurrentItem.Items.Add(entry.LexicalForm.GetFirstAlternative());
			}
		}

		private void _btnNextWord_Click(object sender, EventArgs e)
		{
			_task.NavigateNext();
		}
		private void _btnPreviousWord_Click(object sender, EventArgs e)
		{
			_task.NavigatePrevious();
		}



		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
			string s = _vernacularBox.TextBoxes[0].Text.Trim();
			if(s == "")
			{
				return;
			}
			_task.WordCollected(_vernacularBox.MultiText, _vernacularBox.FlagIsOn);

			//_listViewOfWordsMatchingCurrentItem.Items.Add(s);
			_vernacularBox.TextBoxes[0].Text = "";
			_vernacularBox.FlagIsOn = false;
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
			_listViewOfWordsMatchingCurrentItem.BackColor = BackColor;
			_boxForeignWord.BackColor = BackColor;
		}
	}
}
