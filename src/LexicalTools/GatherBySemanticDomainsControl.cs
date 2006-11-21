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
	public partial class GatherBySemanticDomainsControl : UserControl
	{
		private  List<string> _domains;
		private  List<string> _questions;
		private  IRecordList<LexEntry> _records;
		private int _currentDomainIndex=0;


		public GatherBySemanticDomainsControl()
		{
			InitializeComponent();
			if (DesignMode)
			{
				return;
			}

			BackColor = WeSay.UI.DisplaySettings.Default.BackgroundColor;
			_listViewWords.Items.Clear();
			//TODO: this limits us to a single writing system, and relies on the deprecated "default"
			_vernacularBox.WritingSystems = new WritingSystem[] { BasilProject.Project.WritingSystems.VernacularWritingSystemDefault };
			_vernacularBox.TextChanged += new EventHandler(_vernacularBox_TextChanged);
			_vernacularBox.KeyDown += new KeyEventHandler(_boxVernacularWord_KeyDown);
		}

		public List<string> Domains
		{
			get { return _domains; }
			set { _domains = value;
			UpdateStuff();
				}
		}

		public IRecordList<LexEntry> Records
		{
			get { return _records; }
			set { _records = value; UpdateStuff(); }
		}

		public List<string> Questions
		{
			get { return _questions; }
			set { _questions = value;
				UpdateStuff(); }
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
			if (DesignMode || _domains == null || _questions == null || _records ==null)
			{
				return;
			}
			if (_currentDomainIndex >= _domains.Count)
			{
				_congratulationsControl.Show("Congratulations. You have completed this task.");
			}
			else
			{
				_congratulationsControl.Hide();
				Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
				_domainName.Text = _domains[_currentDomainIndex];
				_question.Text = _questions[_currentDomainIndex];
				_btnNext.Enabled = _domains.Count > (_currentDomainIndex - 1);
				_btnPrevious.Enabled = _currentDomainIndex > 0;
				_btnAddWord.Enabled = _vernacularBox.TextBoxes[0].Text.Trim() != "";
			}
	   }

		private void _btnNext_Click(object sender, EventArgs e)
		{
		   _currentDomainIndex++;
			   SourceWordChanged();
		}


		private void SourceWordChanged()
		{
			_listViewWords.Items.Clear();

			UpdateStuff();
			_vernacularBox.ClearAllText();
			_vernacularBox.TextBoxes[0].Focus();
		}

		private void _btnPrevious_Click(object sender, EventArgs e)
		{
			_currentDomainIndex--;
			SourceWordChanged();
		}



		private void _btnAddWord_Click(object sender, EventArgs e)
		{
//            Debug.Assert(_vernacularBox.TextBoxes.Count == 1, "other code here (for now), assumes exactly one ws/text box");
//            string s = _vernacularBox.TextBoxes[0].Text.Trim();
//            if(s == "")
//            {
//                return;
//            }
//
//            LexEntry entry = new LexEntry();
//            entry.LexicalForm.SetAlternative(BasilProject.Project.WritingSystems.VernacularWritingSystemDefaultId, s);
//            LexSense sense = (LexSense) entry.Senses.AddNew();
//            sense.Gloss.SetAlternative(BasilProject.Project.WritingSystems.AnalysisWritingSystemDefaultId, _words[_currentWordIndex]);
//            _records.Add(entry);
//
//            _listViewWords.Items.Add(s);
			_vernacularBox.TextBoxes[0].Text = "";
			_vernacularBox.FlagIsOn = false;
//            if (WordAdded != null)
//            {
//                WordAdded.Invoke(this, null);
//            }
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
			_listViewWords.BackColor = BackColor;
			_domainName.BackColor = BackColor;
			_question.BackColor = BackColor;
		}
	}
}
