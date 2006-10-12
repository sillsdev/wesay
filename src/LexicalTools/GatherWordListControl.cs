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
		private readonly List<string> _words;
		private readonly IRecordList<LexEntry> _records;
		private int _currentWordIndex=0;

		public GatherWordListControl()
		{
			System.Diagnostics.Debug.Assert(DesignMode);
			InitializeComponent();
		}

		public GatherWordListControl(List<string> words, IRecordList<LexEntry> records)
		{
			_words = words;
			_records = records;
			InitializeComponent();
			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;

			//TODO: this limits us to a single writing system, and relies on the deprecated "default"
			_vernacularBox.WritingSystems = new WritingSystem[] { BasilProject.Project.WritingSystems.VernacularWritingSystemDefault };
			_vernacularBox.TextChanged += new EventHandler(_vernacularBox_TextChanged);
			_vernacularBox.KeyDown += new KeyEventHandler(_boxVernacularWord_KeyDown);
			UpdateStuff();
		}

		void _vernacularBox_TextChanged(object sender, EventArgs e)
		{
			UpdateStuff();
		}

		private void GatherWordListControl_Load(object sender, EventArgs e)
		{
			SourceWordChanged();
		}

		private void UpdateStuff()
		{
			if (DesignMode)
			{
				return;
			}

			Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
			 _boxForeignWord.Text = _words[_currentWordIndex];
			 _btnNextWord.Enabled = _words.Count > (_currentWordIndex-1);
			 _btnPreviousWord.Enabled = _currentWordIndex > 0;
			 _btnAddWord.Enabled = _vernacularBox.TextBoxes[0].Text.Trim() != "";
	   }

		private void _btnNextWord_Click(object sender, EventArgs e)
		{
			_currentWordIndex++;
			SourceWordChanged();
		}

		private void SourceWordChanged()
		{
			_listViewWords.Items.Clear();
			UpdateStuff();
			_vernacularBox.ClearAllText();
			_vernacularBox.TextBoxes[0].Focus();
		}

		private void _btnPreviousWord_Click(object sender, EventArgs e)
		{
			_currentWordIndex--;
			SourceWordChanged();
		}



		private void _btnAddWord_Click(object sender, EventArgs e)
		{
			Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
			string s = _vernacularBox.TextBoxes[0].Text.Trim();
			if(s == "")
			{
				return;
			}

			LexEntry entry = new LexEntry();
			entry.LexicalForm.SetAlternative(BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId, s);
			LexSense sense = (LexSense) entry.Senses.AddNew();
			sense.Gloss.SetAlternative(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefaultId, _words[_currentWordIndex]);
			_records.Add(entry);

			_listViewWords.Items.Add(s);
			_vernacularBox.TextBoxes[0].Text = "";

			UpdateStuff();
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
			_listViewWords.BackColor = BackColor;
			_boxForeignWord.BackColor = BackColor;
		}


//        private void GatherWordListControl_KeyUp(object sender, KeyEventArgs e)
//        {
//        }
////
////        protected override bool IsInputKey(Keys keyData)
////        {
////            return false;
////        }
//
//        private void GatherWordListControl_KeyPress(object sender, KeyPressEventArgs e)
//        {
//
//        }
//
//        private void _boxVernacularWord_KeyUp(object sender, KeyEventArgs e)
//        {
//
//        }
//
//        private void _boxVernacularWord_KeyPress(object sender, KeyPressEventArgs e)
//        {
//
//        }



	}
}
