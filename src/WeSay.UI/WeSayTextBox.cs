using System;
using System.ComponentModel;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
using WeSay.Language;

namespace WeSay.UI
{
	public partial class WeSayTextBox : TextBox
	{
		private WritingSystem _writingSystem;
		private KeymanLink.KeymanLink _keymanLink;
		private bool _multiParagraph;
		private string _nameForLogging;
		private bool _haveAlreadyLoggedTextChanged = false;

		public WeSayTextBox()
		{
			InitializeComponent();
			GotFocus += new EventHandler(OnGotFocus);
			LostFocus += new EventHandler(OnLostFocus);
			KeyPress += new KeyPressEventHandler(WeSayTextBox_KeyPress);
			TextChanged += new EventHandler(WeSayTextBox_TextChanged);

			KeyDown += new KeyEventHandler(OnKeyDown);

		  //  Debug.Assert(DesignMode);
			if (Environment.OSVersion.Platform != PlatformID.Unix)
			{
				_keymanLink = new KeymanLink.KeymanLink();
				if (!_keymanLink.Initialize(false))
				{
					_keymanLink = null;
				}
			}

			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
		}

		void OnKeyDown(object sender, KeyEventArgs e)
		{
			if (e.KeyCode == Keys.F4)
			{
				if (SelectionLength == 0)
				{
					if (Text !=null)   //grab the whole field
					{
						DoToolboxJump(Text.Trim());
					}
				}
				else if (SelectedText != null)
				{
					DoToolboxJump(SelectedText);
				}
			}
		}


		static private void DoToolboxJump(string word)
		{
			try
			{
				Reporting.Logger.WriteMinorEvent("Jumping to Toolbox");
				Type toolboxJumperType = Type.GetTypeFromProgID("Toolbox.Jump");
				if (toolboxJumperType != null)
				{
					Object toolboxboxJumper = Activator.CreateInstance(toolboxJumperType);
					if ((toolboxboxJumper != null))
					{
						object[] args = new object[] { word };
						toolboxJumperType.InvokeMember("Jump", BindingFlags.InvokeMethod, null, toolboxboxJumper, args);
					}
				}
			}
			catch (Exception)
			{
				Reporting.ErrorReporter.ReportNonFatalMessage("Could not get a connection to Toolbox.");
				throw;
			}
		}

		void WeSayTextBox_TextChanged(object sender, EventArgs e)
		{
			//only first change per focus session will be logged
			if (!_haveAlreadyLoggedTextChanged &&
				Focused/*try not to report when code is changing us*/)
			{
				_haveAlreadyLoggedTextChanged = true;
				Reporting.Logger.WriteMinorEvent("First_TextChange (could be paste via mouse) {0}:{1}", _nameForLogging, _writingSystem.Id);
			}
		}

		void WeSayTextBox_KeyPress(object sender, KeyPressEventArgs e)
		{
			//only first change per focus session will be logged
			if (!_haveAlreadyLoggedTextChanged)
			{
				_haveAlreadyLoggedTextChanged = true;
				Reporting.Logger.WriteMinorEvent("First_KeyPress {0}:{1}", _nameForLogging, _writingSystem.Id);
			}
		}

		void OnLostFocus(object sender, EventArgs e)
		{
			Reporting.Logger.WriteMinorEvent("LostFocus {0}:{1}", _nameForLogging, _writingSystem.Id);
		}

		void OnGotFocus(object sender, EventArgs e)
		{
			Reporting.Logger.WriteMinorEvent("Focus {0}:{1}", _nameForLogging, _writingSystem.Id);
			_haveAlreadyLoggedTextChanged = false;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged(e);
			UpdateHeight();
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			UpdateHeight();
		}

		private void UpdateHeight()
		{
			SuspendLayout();
			if (Text.Length == 0)
			{
				Height = PreferredHeight;
				return;
			}


			using (Graphics g = CreateGraphics())
			{
				Size sz = TextRenderer.MeasureText(g,
												   Text + "\n",
												   // need extra new line to handle case where ends in new line (since last newline is ignored)
									Font,
									new Size(Width, int.MinValue),
									TextFormatFlags.TextBoxControl |
										TextFormatFlags.Default |
										TextFormatFlags.NoClipping |
										TextFormatFlags.WordBreak);

				Height = Math.Max(MinimumSize.Height, sz.Height);
			}
			ResumeLayout(false);
		}

		public WeSayTextBox(WritingSystem ws, string nameForLogging):this()
		{
			_nameForLogging = nameForLogging;
			 WritingSystem = ws;
		}

		[Browsable(false)]
		public override string Text
		{
			set
			{
				base.Text = value;
			}
			get
			{
				return base.Text;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystem WritingSystem
		{
			get
			{
				if(_writingSystem == null)
				{
					throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
				}
				return _writingSystem;
			}
			set
			{
				if(value == null)
				{
					throw new ArgumentNullException();
				}
				_writingSystem = value;
				Font = value.Font;
				if (value.RightToLeft)
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
			get { return this._multiParagraph; }
			set { this._multiParagraph = value; }
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

		public void FadeInSomeMore(Label label)
		{
			int interval = 2;
			if (BackColor.R < SystemColors.Window.R)
			{
				interval = Math.Min(interval, 255 - BackColor.R);

				BackColor = Color.FromArgb(BackColor.R + interval,
															 BackColor.G + interval,
															 BackColor.B + interval);
			}
			else if( BackColor != SystemColors.Window)
			{
				BackColor = SystemColors.Window;
			}
		}

		public void PrepareForFadeIn()
		{
				Text = ""; //ready for the next one
				BackColor = SystemColors.Control;
		}

		protected override void OnEnter(EventArgs e)
		{
			base.OnEnter(e);
			AssignKeyboardFromWritingSystem();
		}

		public void AssignKeyboardFromWritingSystem()
		{
			if (_writingSystem == null)
			{
				throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
			}
//
//            if (_writingSystem.KeyboardName == null || _writingSystem.KeyboardName == string.Empty)
//            {
//                InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;
//                return;
//            }

			InputLanguage inputLanguage = FindInputLanguage(this._writingSystem.KeyboardName);
			if (inputLanguage != null)
			{
				InputLanguage.CurrentInputLanguage = inputLanguage;
			}
			else
			{
				//set the windows back to default so it doesn't interfere
				//nice idea but is unneeded... perhaps keyman is calling this too
				//InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;

				if (_keymanLink != null && !string.IsNullOrEmpty(_writingSystem.KeyboardName))
				{
					_keymanLink.SelectKeymanKeyboard(_writingSystem.KeyboardName, true);
				}
			}
		}

		static private InputLanguage FindInputLanguage(string name)
		{
			if(InputLanguage.InstalledInputLanguages != null) // as is the case on Linux
			{
				foreach (InputLanguage  l in InputLanguage.InstalledInputLanguages )
				{
					if (l.LayoutName == name)
					{
						return l;
					}
				}
			}
			return null;
		}

		protected override void OnLeave(EventArgs e)
		{
			base.OnLeave(e);

			// this.BackColor = System.Drawing.Color.White;
			ClearKeyboard();
		}

		public void ClearKeyboard() {
			if (_writingSystem == null)
			{
				throw new InvalidOperationException("WritingSystem must be initialized prior to use.");
			}
			if (FindInputLanguage(this._writingSystem.KeyboardName) != null)//just a weird way to know if we changed the keyboard when we came in
			{
				InputLanguage.CurrentInputLanguage = InputLanguage.DefaultInputLanguage;
			}
			else if (this._keymanLink != null)
			{
				this._keymanLink.SelectKeymanKeyboard(null, false);
			}
		}

		/// <summary>
		/// for automated tests
		/// </summary>
		public void PretendLostFocus()
		{
			OnLostFocus(new EventArgs());
		}
	}
}
