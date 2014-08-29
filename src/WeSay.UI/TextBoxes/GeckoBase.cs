using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gecko;
using Gecko.DOM;
using Gecko.Events;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.Progress;

namespace WeSay.UI.TextBoxes
{
	public class GeckoBase : UserControl
	{
		protected GeckoWebBrowser _browser;
		protected IWritingSystemDefinition _writingSystem;
		protected bool _browserIsReadyToNavigate;
		protected bool _browserDocumentLoaded;
		protected EventHandler _loadHandler;
		protected EventHandler<DomKeyEventArgs> _domKeyUpHandler;
		protected EventHandler<DomMouseEventArgs> _domClickHandler;
		protected EventHandler<DomKeyEventArgs> _domKeyDownHandler;
		protected EventHandler<DomEventArgs> _domFocusHandler;
		protected EventHandler<DomEventArgs> _domBlurHandler;
		protected EventHandler<GeckoDocumentCompletedEventArgs> _domDocumentCompletedHandler;
		protected EventHandler _backColorChangedHandler;
		protected EventHandler _foreColorChangedHandler;
		public event EventHandler UserLostFocus;
		public event EventHandler UserGotFocus;
		protected bool _inFocus;
		protected string _nameForLogging;
		protected bool _handleEnter;
		private Timer _timer;
		protected GeckoHtmlElement _focusElement;
		/// <summary>
		/// Set to true when OnEnter has been called, but OnLeave hasn't yet been called.
		/// </summary>
		bool _entered = false;

		public GeckoBase()
		{
			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
			Name = _nameForLogging;
			ReadOnly = false;

			_inFocus = false;
			_handleEnter = true;
			_browser = new GeckoWebBrowser();
			_browser.Dock = DockStyle.Fill;
			_browser.Parent = this;
			_browser.NoDefaultContextMenu = true;

			SelectionStart = 0;  // Initialize value;

			_loadHandler = new EventHandler(OnGeckoBox_Load);
			this.Load += _loadHandler;
			Controls.Add(_browser);

			_domDocumentCompletedHandler = new EventHandler<GeckoDocumentCompletedEventArgs>(OnDomDocumentCompleted);
			_browser.DocumentCompleted += _domDocumentCompletedHandler;
			_domFocusHandler = new EventHandler<DomEventArgs>(OnDomFocus);
			_browser.DomFocus += _domFocusHandler;
			_domKeyUpHandler = new EventHandler<DomKeyEventArgs>(OnDomKeyUp);
			_browser.DomKeyUp += _domKeyUpHandler;
			_domKeyDownHandler = new EventHandler<DomKeyEventArgs>(OnDomKeyDown);
			_browser.DomKeyDown += _domKeyDownHandler;
			_domBlurHandler = new EventHandler<DomEventArgs>(OnDomBlur);
			_browser.DomBlur += _domBlurHandler;
			_backColorChangedHandler = new EventHandler(OnBackColorChanged);
			this.BackColorChanged += _backColorChangedHandler;
			_foreColorChangedHandler = new EventHandler(OnForeColorChanged);
			this.ForeColorChanged += _foreColorChangedHandler;
#if __MonoCS__
			_domClickHandler = new EventHandler<DomMouseEventArgs>(OnDomClick);
			_browser.DomClick += _domClickHandler;
#endif
		}
		public void Init(IWritingSystemDefinition writingSystem, String name)
		{
			WritingSystem = writingSystem;
			_nameForLogging = name;
			Name = name;
		}
		protected virtual void Closing()
		{
			this.Load -= _loadHandler;
			this.BackColorChanged -= _backColorChangedHandler;
			this.ForeColorChanged -= _foreColorChangedHandler;
			_focusElement = null;
			if (_timer != null)
			{
				_timer.Stop();
				_timer = null;
			}
			if (_browser != null)
			{
				_browser.DomKeyDown -= _domKeyDownHandler;
				_browser.DomKeyUp -= _domKeyUpHandler;
				_browser.DomFocus -= _domFocusHandler;
				_browser.DomBlur -= _domBlurHandler;
				_browser.DocumentCompleted -= _domDocumentCompletedHandler;
#if __MonoCS__
				_browser.DomClick -= _domClickHandler;
				_browser.Dispose();
				_browser = null;
#else
				if (Xpcom.IsInitialized)
				{
					_browser.Dispose();
					_browser = null;
				}
#endif
			}
			_loadHandler = null;
			_domKeyDownHandler = null;
			_domKeyUpHandler = null;
			_domFocusHandler = null;
			_domDocumentCompletedHandler = null;
#if __MonoCS__
			_domClickHandler = null;
#endif
		}
		protected virtual void AdjustHeight()
		{
			if (_browser.Document == null)
			{
				return;
			}
			var content = (GeckoBodyElement)_browser.Document.GetElementById("mainbody");
			if (content != null)
			{
				Height = content.Parent.ScrollHeight;
			}
		}
		public virtual IWritingSystemDefinition WritingSystem
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
				Font = WritingSystemInfo.CreateFont(_writingSystem);
			}
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
			Debug.Assert(_browser != null, "_browser != null");
			_browser.Focus();
		}

		protected virtual void OnDomKeyDown(object sender, DomKeyEventArgs e)
		{
			if (_inFocus)
			{

				if (_handleEnter && !MultiParagraph && e.KeyCode == (uint)Keys.Enter) // carriage return
				{
					e.Handled = true;
				}
#if __MonoCS__
				SendKey(e);
#else
				if ((e.KeyCode == (uint)Keys.Tab) && !e.CtrlKey && !e.AltKey)
				{
					e.Handled = true;
					if (e.ShiftKey)
					{
						if (!ParentForm.SelectNextControl(this, false, true, true, true))
						{
#if DEBUG
							Debug.WriteLine("Failed to advance");
#endif
						}
					}
					else
					{
						if (!ParentForm.SelectNextControl(this, true, true, true, true))
						{
#if DEBUG
							Debug.WriteLine("Failed to advance");
#endif
						}
					}
					e.Handled = true;
					return;
				}

#endif
				OnKeyDown(new KeyEventArgs((Keys)e.KeyCode));
			}
		}

		protected virtual void SendKey(DomKeyEventArgs e)
		{
			var builder = new StringBuilder();
			switch (e.KeyCode)
			{
				case (uint)Keys.Tab:
					if (e.CtrlKey)
					{
						builder.Append("^");
					}
					if (e.AltKey)
					{
						builder.Append("%");
					}
					if (e.ShiftKey)
					{
						builder.Append("+");
					}
					builder.Append("{TAB}");
					break;
				case (uint)Keys.Up:
					builder.Append("{UP}");
					break;
				case (uint)Keys.Down:
					builder.Append("{DOWN}");
					break;
				case (uint)Keys.Left:
					builder.Append("{LEFT}");
					break;
				case (uint)Keys.Right:
					builder.Append("{RIGHT}");
					break;
				case (uint)Keys.Escape:
					builder.Append("{ESC}");
					break;
				case (uint)Keys.N:
					if (e.CtrlKey)
					{
						builder.Append("^n");
					}
					break;
				case (uint)Keys.F:
					if (e.CtrlKey)
					{
						builder.Append("^f");
					}
					break;
				case (uint)Keys.Delete:
					builder.Append("{DEL}");
					break;
				case (uint)Keys.Enter:
					builder.Append("{ENTER}");
					break;
			}
			string result = builder.ToString();
			if (! String.IsNullOrEmpty(result))
			{
				SendKeys.Send(result);
			}
		}
		public virtual bool InFocus
		{
			get { return _inFocus; }
			set { _inFocus = value; }
		}

		public virtual bool Bold { get; set; }

		public virtual int SelectionStart { get; set; }
		public int SelectionLength { get; set; }
		public virtual bool Multiline { get; set; }
		public virtual bool MultiParagraph { get; set; }
		public virtual bool ReadOnly { get; set; }
		public virtual bool WordWrap { get; set; }
		public virtual View View { get; set; }
		public virtual DrawMode DrawMode { get; set; }
		public virtual FlatStyle FlatStyle { get; set; }

		protected virtual void OnDomBlur(object sender, DomEventArgs e)
		{
			if (_inFocus)
			{
				_inFocus = false;
#if DEBUG
				//Debug.WriteLine("OnDomBlur");
#endif
				if (UserLostFocus != null)
				{
					UserLostFocus.Invoke(this, null);
				}
			}
		}
		protected virtual void OnDomFocus(object sender, DomEventArgs e)
		{
			var content = _browser.Document.GetElementById("main");
			if (content != null)
			{
				if ((content is GeckoHtmlElement) && (!_inFocus))
				{
					// The following is required because we get two in focus events every time this
					// is entered.  This is normal for Gecko.  But I don't want to be constantly
					// refocussing.
					_inFocus = true;
#if DEBUG
					//Debug.WriteLine("OnDomFocus");
#endif
					EnsureFocusedGeckoControlHasInputFocus();
					if (_browser != null)
					{
						_browser.SetInputFocus();
					}
					_focusElement = (GeckoHtmlElement)content;
					ChangeFocus();
				}
			}
		}
		protected virtual void OnDomDocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
		{
			_browserDocumentLoaded = true;  // Document loaded once
			AdjustHeight();
			if (_entered)
			{
				this.Focus();

				IContainerControl containerControl = GetContainerControl();

				if ((containerControl != null) && (containerControl != this) && (containerControl.ActiveControl != this))
					containerControl.ActiveControl = this;

				_browser.WebBrowserFocus.Activate();
				var element = (GeckoHtmlElement)_browser.Document.GetElementById("main");
				element.Focus();

				EnsureFocusedGeckoControlHasInputFocus();
			}
		}

		protected override void OnLeave(EventArgs e)
		{
			if ((_browser != null) && (_browser.WebBrowserFocus != null))
			{
				_browser.WebBrowserFocus.Deactivate();
			}

			_entered = false;

			base.OnLeave(e);
#if __MonoCS__
			Action EnsureXInputFocusIsRemovedFromReceivedWinFormsControl = () =>
			{
				var control = Control.FromHandle(NativeReplacements.MonoGetFocus());
				if (control is GeckoWebBrowser)
					return;

				MoveInputFocusBacktoAWinFormsControl();

				// Setting the ActiveControl ensure a Focus event occurs on the control focus is moving to.
				// And this allows us to call RemoveinputFocus at the neccessary time.
				// This prevents keypress still going to the gecko controls when a winform TextBox has focus
				// and the mouse is over a gecko control.
				Form.ActiveForm.ActiveControl = control;
				EventHandler focusEvent = null;
				// Attach a execute once only focus handler to the Non GeckoWebBrowser control focus is moving too...
				focusEvent = (object sender, EventArgs eventArg) =>
				{
					control.GotFocus -= focusEvent;
					MoveInputFocusBacktoAWinFormsControl();
				};

				control.GotFocus += focusEvent;
			};

			ProgressUtils.InvokeLaterOnUIThread(() => EnsureXInputFocusIsRemovedFromReceivedWinFormsControl());
#endif
		}

		protected void ChangeFocus()
		{
			_focusElement.Focus();
			if (UserGotFocus != null)
			{
				UserGotFocus.Invoke(this, null);
			}
		}

		protected override void OnEnter(EventArgs e)
		{
			_entered = true;

			// Only do this is the controls html has loaded.
			if (_browserDocumentLoaded)
			{
				_browser.WebBrowserFocus.Activate();
				EnsureFocusedGeckoControlHasInputFocus();
			}
			base.OnEnter(e);
		}
		/// <summary>
		/// This gives a move sensible result than the default winform implemenentation.
		/// </summary>
		/// <value><c>true</c> if focused; otherwise, <c>false</c>.</value>
		public override bool Focused
		{
			get
			{
				return base.Focused || ContainsFocus;
			}
		}
		/// <summary>
		/// If Browser control dosesn't have X11 input focus.
		/// then Ensure that it does..
		/// </summary>
		protected void EnsureFocusedGeckoControlHasInputFocus()
		{
			if ((_browser == null) || (_browser.HasInputFocus()))
				return;

			// Attempt ot do it right away.
			this._browser.SetInputFocus();

			// Othewise do it on the first idle event.
			Action setInputFocus = null;
			setInputFocus = () =>
			{
				ProgressUtils.InvokeLaterOnIdle(() =>
				{
					if (_browser == null)
					{
						return;
					}
					if (Form.ActiveForm == null || _browser.ContainsFocus == false)
					{
						MoveInputFocusBacktoAWinFormsControl();
						return;
					}

					if (!_browser.HasInputFocus())
						this._browser.SetInputFocus();
					if (!_browser.HasInputFocus())
						setInputFocus();
				}, null);
			};

			setInputFocus();
		}

		public string GetLanguageHtml(IWritingSystemDefinition ws)
		{
			String langName = "";
			// Add in the ISO language code in case font supports multiple regions
			if (ws != null)
			{
				String lang = ws.Bcp47Tag.IndexOf('-') == -1 ? ws.Bcp47Tag : ws.Bcp47Tag.Substring(0, ws.Bcp47Tag.IndexOf('-'));
				langName = "lang='" + lang + "' ";
			}
			return langName;
		}

		/// <summary>
		/// Set InputFocus to a WinForm controls using Mono winforms connection to the X11 server.
		/// GeckoWebBrowser.RemoveInputFocus uses the Gecko/Gtk connection to the X11 server.
		/// The undoes the call to _browser.SetInputFocus.
		/// Call this method when a winform control has gained focus, and X11 Input focus is still on the Gecko control.
		/// </summary>
		protected static void MoveInputFocusBacktoAWinFormsControl()
		{
#if __MonoCS__
			IntPtr newTargetHandle = NativeReplacements.MonoGetFocus();
			IntPtr displayHandle = NativeReplacements.MonoGetDisplayHandle();

			// Remove the Focus from a Gtk window back to a mono winform X11 window.
			NativeX11Methods.XSetInputFocus(displayHandle, NativeReplacements.MonoGetX11Window(newTargetHandle), NativeX11Methods.RevertTo.None, IntPtr.Zero);
#endif
		}
		// Making these empty handlers rather than abstract so the class only
		// needs to implement the ones they need.
		protected virtual void OnDomKeyUp(object sender, DomKeyEventArgs e)
		{
		}
		protected virtual void OnDomClick(object sender, DomMouseEventArgs e)
		{
		}
		protected virtual void OnGeckoBox_Load(object sender, EventArgs e)
		{
		}
		protected virtual void OnBackColorChanged(object sender, EventArgs e)
		{
		}
		protected virtual void OnForeColorChanged(object sender, EventArgs e)
		{
		}
	}
}
