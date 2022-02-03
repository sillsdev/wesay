using SIL.Keyboarding;
using SIL.Reporting;
using SIL.Text;
using SIL.Windows.Forms.Spelling;
using SIL.WritingSystems;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI.TextBoxes
{
	public interface IWeSayTextBox
	{
		Size GetPreferredSize(Size proposedSize);

		[Browsable(false)]
		string Text { set; get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		WritingSystemDefinition WritingSystem { get; set; }

		bool MultiParagraph { get; set; }
		bool IsSpellCheckingEnabled { get; set; }
		int SelectionStart { get; set; }
		int SelectionLength { get; set; }
		void Select(int start, int length);
		void AssignKeyboardFromWritingSystem();
		void ClearKeyboard();
		void Init(WritingSystemDefinition writingSystem, String name);
		Font Font { get; set; }

		Object Tag { get; set; }
		bool Multiline { get; set; }
		bool WordWrap { get; set; }
		bool ReadOnly { get; set; }
		bool Focus();
		void Show();
		Color ForeColor { get; set; }
		bool TabStop { get; set; }
		bool IsDisposed { get; }
		event EventHandler UserLostFocus;
		event EventHandler UserGotFocus;

		/// <summary>
		/// for automated tests
		/// </summary>
		void PretendLostFocus();

		/// <summary>
		/// for automated tests
		/// </summary>
		void PretendSetFocus();

		/// <summary>
		/// Formatting information that we need to preserve.
		/// </summary>
		List<LanguageForm.FormatSpan> Spans { get; set; }
	}


	public partial class WeSayTextBox : TextBox, IControlThatKnowsWritingSystem, IWeSayTextBox
	{
		private WritingSystemDefinition _writingSystem;
		private string _previousText;

		private bool _multiParagraph;
		private string _nameForLogging;
		private bool _haveAlreadyLoggedTextChanged;
		private bool _isSpellCheckingEnabled;

		/// <summary>
		/// Don't use this directly, use the Singleton Property TextBoxSpellChecker
		/// </summary>
		private static TextBoxSpellChecker _textBoxSpellChecker;
		public event EventHandler UserLostFocus;
		public event EventHandler UserGotFocus;

		public WeSayTextBox()
		{
			InitializeComponent();
			if (DesignMode)
			{
				return;
			}
			DoubleBuffered = true;
			GotFocus += OnGotFocus;
			LostFocus += OnLostFocus;
			KeyPress += OnKeyPress;
			TextChanged += OnTextChanged;

			KeyDown += OnKeyDown;

			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
			Name = _nameForLogging;
		}

		public void Init(WritingSystemDefinition writingSystem, String name)
		{
			WritingSystem = writingSystem;
			_nameForLogging = name;
			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
			Name = name;
		}
		private void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F4)
			{
				if (SelectionLength == 0)
				{
					if (Text != null) //grab the whole field
					{
						DoToolboxJump(Text.Trim());
					}
				}
				else if (SelectedText != null)
				{
					DoToolboxJump(SelectedText);
				}
			}

			if (e.KeyCode == Keys.Pause && e.Modifiers == Keys.Shift)
			{
				Process.GetCurrentProcess().Kill();
			}
			if (e.KeyCode == Keys.Pause && (e.Alt))
			{
				throw new ApplicationException("User-invoked test crash.");
			}
			if (e.KeyCode == Keys.PageDown)
			{
				e.Handled = false;
			}
		}

		private static void DoToolboxJump(string word)
		{
			try
			{
				Logger.WriteMinorEvent("Jumping to Toolbox");
				Type toolboxJumperType = Type.GetTypeFromProgID("Toolbox.Jump");
				if (toolboxJumperType != null)
				{
					Object toolboxboxJumper = Activator.CreateInstance(toolboxJumperType);
					if ((toolboxboxJumper != null))
					{
						object[] args = new object[] { word };
						toolboxJumperType.InvokeMember("Jump",
													   BindingFlags.InvokeMethod,
													   null,
													   toolboxboxJumper,
													   args);
					}
				}
			}
			catch (Exception)
			{
				ErrorReport.NotifyUserOfProblem("Could not get a connection to Toolbox.");
				throw;
			}
		}

		private void OnTextChanged(object sender, EventArgs e)
		{
			//only first change per focus session will be logged
			if (!_haveAlreadyLoggedTextChanged && Focused
				/*try not to report when code is changing us*/)
			{
				_haveAlreadyLoggedTextChanged = true;
				Logger.WriteMinorEvent("First_TextChange (could be paste via mouse) {0}:{1}",
									   _nameForLogging,
									   _writingSystem.LanguageTag);
			}
			LanguageForm.AdjustSpansForTextChange(_previousText, Text, Spans);
			_previousText = Text;
		}

		private void OnKeyPress(object sender, KeyPressEventArgs e)
		{
			//only first change per focus session will be logged
			if (!_haveAlreadyLoggedTextChanged)
			{
				_haveAlreadyLoggedTextChanged = true;
				Logger.WriteMinorEvent("First_KeyPress {0}:{1}", _nameForLogging, _writingSystem.LanguageTag);
			}
		}

		private void OnLostFocus(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("LostFocus {0}:{1}", _nameForLogging, _writingSystem.LanguageTag);
			if (UserLostFocus != null)
			{
				UserLostFocus.Invoke(sender, e);
			}
		}

		private void OnGotFocus(object sender, EventArgs e)
		{
			Logger.WriteMinorEvent("Focus {0}:{1}", _nameForLogging, _writingSystem.LanguageTag);
			_haveAlreadyLoggedTextChanged = false;
			if (UserGotFocus != null)
			{
				UserGotFocus.Invoke(sender, e);
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			if (IsDisposed) // are we a zombie still getting events?
			{
				throw new ObjectDisposedException(GetType().ToString());
			}
			base.OnTextChanged(e);
			Height = GetPreferredHeight(Width);
			if (WeSay.UI.Platform.IsLinux)
			{
				// For some fonts that don't render properly in MONO
				Refresh ();
			}
		}

		// we do this in OnLayout instead of OnResize see
		// "Setting the Size/Location of child controls in the Resize event
		// http://blogs.msdn.com/jfoscoding/archive/2005/03/04/385625.aspx
		protected override void OnLayout(LayoutEventArgs levent)
		{
			Height = GetPreferredHeight(Width);
			base.OnLayout(levent);
		}

		// we still need the resize sometimes or ghost fields disappear
		protected override void OnSizeChanged(EventArgs e)
		{
			Height = GetPreferredHeight(Width);
			base.OnSizeChanged(e);
		}

		protected override void OnResize(EventArgs e)
		{
			Height = GetPreferredHeight(Width);
			base.OnResize(e);
		}

		public override Size GetPreferredSize(Size proposedSize)
		{
			Size size = base.GetPreferredSize(proposedSize);
			size.Height = GetPreferredHeight(size.Width);
			return size;
		}

		private int _oldWidth = -1;
		private string _oldText;
		private Font _oldFont;
		private int _oldHeight = -1;
		private int GetPreferredHeight(int width)
		{
			if (width == _oldWidth && Text == _oldText && Font == _oldFont && _oldHeight > 0)
				return _oldHeight;
			using (Graphics g = CreateGraphics())
			{
				TextFormatFlags flags = TextFormatFlags.TextBoxControl | TextFormatFlags.Default |
										TextFormatFlags.NoClipping;
				if (Multiline && WordWrap)
				{
					flags |= TextFormatFlags.WordBreak;
				}
				if (_writingSystem != null && WritingSystem.RightToLeftScript)
				{
					flags |= TextFormatFlags.RightToLeft;
				}
				Size sz = TextRenderer.MeasureText(g,
													Text == String.Empty ? " " : Text + Environment.NewLine,
												   // replace empty string with space, because mono returns zero height for empty string (windows returns one line height)
												   // need extra new line to handle case where ends in new line (since last newline is ignored)
												   Font,
												   new Size(width, int.MaxValue),
												   flags);
				if (WeSay.UI.Platform.IsLinux)
				{
					// For Mono, need to make an additional adjustment if more than one line is displayed
					Size sz2 = TextRenderer.MeasureText(g,
							" ",
							Font,
							new Size(width, int.MaxValue),
							flags);
					int numberOfLines = sz.Height/ sz2.Height;
					if (sz.Height % sz2.Height != 0)
					{
						numberOfLines++;
					}
					if (numberOfLines > 1)
					{
						sz.Height += numberOfLines * 4;
					}
				}
				_oldHeight = sz.Height + 2; // add enough space for spell checking squiggle underneath
				_oldWidth = width;
				_oldText = Text;
				_oldFont = Font;
				return _oldHeight;
			}
		}

		public WeSayTextBox(WritingSystemDefinition ws, string nameForLogging) : this()
		{
			_nameForLogging = nameForLogging;
			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
			Name = _nameForLogging;
			WritingSystem = ws;
		}

		[Browsable(false)]
		public override string Text
		{
			set
			{
				_previousText = value;
				base.Text = value;
			}
			get { return base.Text; }
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystemDefinition WritingSystem
		{
			get
			{
				if (_writingSystem == null)
				{
					throw new InvalidOperationException(
						"Input system must be initialized prior to use.");
				}
				return _writingSystem;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_writingSystem = value;
				Font = WritingSystemInfo.CreateFont(value);

				//hack for testing
				//   this.Height = (int) Math.Ceiling( Font.GetHeight());

				if (value.RightToLeftScript)
				{
					RightToLeft = RightToLeft.Yes;
				}
				else
				{
					RightToLeft = RightToLeft.No;
				}
			}
		}

		public bool MultiParagraph
		{
			get { return _multiParagraph; }
			set { _multiParagraph = value; }
		}

		public bool IsSpellCheckingEnabled
		{
			get { return _isSpellCheckingEnabled; }
			set
			{
				_isSpellCheckingEnabled = value;
				if (_isSpellCheckingEnabled && !(WritingSystem.SpellCheckingId == "none"))
				{
					OnSpellCheckingEnabled();
				}
				else
				{
					OnSpellCheckingDisabled();
				}
			}
		}

		private void OnSpellCheckingDisabled()
		{
			TextBoxSpellChecker.SetLanguageForSpellChecking(this, null);
		}

		private void OnSpellCheckingEnabled()
		{
			TextBoxSpellChecker.SetLanguageForSpellChecking(this, _writingSystem.SpellCheckingId);
		}

		private static TextBoxSpellChecker TextBoxSpellChecker
		{
			get
			{
				if (_textBoxSpellChecker == null)
				{
					_textBoxSpellChecker = new TextBoxSpellChecker();
				}
				return _textBoxSpellChecker;
			}
		}

		protected override void OnKeyPress(KeyPressEventArgs e)
		{
			if (!MultiParagraph && (e.KeyChar == '\r' || e.KeyChar == '\n')) // carriage return
			{
				e.Handled = true;
			}
			base.OnKeyPress(e);
		}

		//public bool IsGhost
		//{
		//    get
		//    {
		//        return _isGhost;
		//    }
		//    set
		//    {
		//        if (_isGhost != value)
		//        {
		//            if (value) // prepare for fade-in
		//            {
		//                this.Text = ""; //ready for the next one
		//                this.BackColor = System.Drawing.SystemColors.Control;
		//                this.BorderStyle = BorderStyle.None;
		//            }
		//            else  // show as "real"
		//            {
		//                //no change currently
		//            }
		//        }
		//        _isGhost = value;
		//    }
		//}

		//public void FadeInSomeMore(Label label)
		//{
		//    int interval = 2;
		//    if (BackColor.R < SystemColors.Window.R)
		//    {
		//        interval = Math.Min(interval, 255 - BackColor.R);

		//        BackColor = Color.FromArgb(BackColor.R + interval,
		//                                                     BackColor.G + interval,
		//                                                     BackColor.B + interval);
		//    }
		//    else if( BackColor != SystemColors.Window)
		//    {
		//        BackColor = SystemColors.Window;
		//    }
		//}

		//public void PrepareForFadeIn()
		//{
		//        Text = ""; //ready for the next one
		//        BackColor = SystemColors.Control;
		//}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			AssignKeyboardFromWritingSystem();
		}

		public void AssignKeyboardFromWritingSystem()
		{
			if (_writingSystem == null)
			{
				throw new InvalidOperationException(
					"Input system must be initialized prior to use.");
			}

			_writingSystem.LocalKeyboard.Activate();
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);

			// this.BackColor = System.Drawing.Color.White;
			ClearKeyboard();
		}

		public void ClearKeyboard()
		{
			if (_writingSystem == null)
			{
				throw new InvalidOperationException(
					"Input system must be initialized prior to use.");
			}

			Keyboard.Controller.ActivateDefaultKeyboard();
		}

		/// <summary>
		/// for automated tests
		/// </summary>
		public void PretendLostFocus()
		{
			OnLostFocus(new EventArgs());
		}

		/// <summary>
		/// for automated tests
		/// </summary>
		public void PretendSetFocus()
		{
			Focus();
		}

		public List<LanguageForm.FormatSpan> Spans { get; set; }
	}
}