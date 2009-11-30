using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel.Foundation;
using WeSay.LexicalTools.Properties;
using WeSay.UI;
using WeSay.UI.AutoCompleteTextBox;

namespace WeSay.LexicalTools.DictionaryBrowseAndEdit
{
	public partial class SearchBoxControl : UserControl
	{
		private readonly ContextMenu _searchModeMenu;

		public SearchBoxControl()
		{
			InitializeComponent();
			BackColor = Color.White;
			_searchModeMenu = new ContextMenu();
			_selectedWritingSystemLabel.ForeColor = DisplaySettings.Default.WritingSystemLabelColor;
			_writingSystemChooser.Image = Resources.Expand.GetThumbnailImage(6,
																			 6,
																			 ReturnFalse,
																			 IntPtr.Zero);
		}

		private static bool ReturnFalse()
		{
			return false;
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WeSayAutoCompleteTextBox TextBox
		{
			get { return _textToSearchForBox; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ContextMenu SearchModeMenu
		{
			get { return _searchModeMenu; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public Button FindButton
		{
			get { return _findButton; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem ListWritingSystem
		{
			get
			{
				return _textToSearchForBox.WritingSystem;
			}
			set
			{
				if(value !=null)//the designer generated code leads to this being null initially
				_textToSearchForBox.WritingSystem = value;
			}

		}

		public void SetWritingSystem(WritingSystem writingSystem)
		{
			ListWritingSystem = writingSystem;
			_selectedWritingSystemLabel.Text = writingSystem.Abbreviation;

			_selectedWritingSystemLabel.AutoSize = false;
			this.Height = 10 + Math.Max(GetAbbreviationSize().Height, (Math.Max(_textToSearchForBox.Height, _findButton.Height)));

			ConfigSizesOfChildren();
		}

		private void ConfigSizesOfChildren()
		{
			Size abbreviationSize = GetAbbreviationSize();


			_selectedWritingSystemLabel.Width = abbreviationSize.Width + 5;
			_selectedWritingSystemLabel.Height = abbreviationSize.Height + 0;

			//         _textToSearchForBox.Height = Math.Max(_selectedWritingSystemLabel.Height, SearchTextBox.Height);

			//        SearchTextBox.Size = new Size(SearchTextBox.Width, 100);


			//          _findButton.Height = SearchTextBox.Height;
			//          _writingSystemChooser.Height = SearchTextBox.Height;

			_writingSystemChooser.Left = Width - _writingSystemChooser.Width;
			_findButton.Left = _writingSystemChooser.Left - _findButton.Width;
			_textToSearchForBox.Left = _selectedWritingSystemLabel.Right ;
			_textToSearchForBox.Width = _findButton.Left - _textToSearchForBox.Left;

		 //   this.MinimumSize = new Size(_writingSystemChooser.Right + 10, this.Height);
		}

		private Size GetAbbreviationSize()
		{
			return TextRenderer.MeasureText(_selectedWritingSystemLabel.Text,
											_selectedWritingSystemLabel.Font);
		}

		private void OnWritingSystemChooser_Click(object sender, EventArgs e)
		{
			foreach (MenuItem menuItem in _searchModeMenu.MenuItems)
			{
				menuItem.Checked = (ListWritingSystem == menuItem.Tag);
			}
			_searchModeMenu.Show(_writingSystemChooser,
								   new Point(_writingSystemChooser.Width,
											 _writingSystemChooser.Height));
		}

		private void SearchBoxControl_SizeChanged(object sender, EventArgs e)
		{
			ConfigSizesOfChildren();
		}


		private void SearchBoxControl_Load(object sender, EventArgs e)
		{
			_desperationDisplayTimer.Enabled=true;
		}

		private void _desperationDisplayTimer_Tick(object sender, EventArgs e)
		{
			//jh sept 2009 After 2 hours of trying to get the find button and the
			//chooser to show initially (before you move the resize control), I
			//gave up and added this hack.
			_desperationDisplayTimer.Enabled = false;
			ConfigSizesOfChildren();
		}
	}
}
