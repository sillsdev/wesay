using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Gecko;
using Gecko.DOM;
using Gecko.Events;
using Palaso.WritingSystems;

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
		protected bool _inFocus;
		protected string _nameForLogging;
		protected bool _handleEnter;
		private Timer _timer;
		protected GeckoHtmlElement _focusElement;

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
#if __MonoCS__
			_domClickHandler = new EventHandler<DomMouseEventArgs>(OnDomClick);
			_browser.DomClick += _domClickHandler;
#endif
		}
		public void Init(IWritingSystemDefinition writingSystem, String name)
		{
			_writingSystem = writingSystem;
			_nameForLogging = name;
			Name = name;
		}
		protected virtual void Closing()
		{
			this.Load -= _loadHandler;
			this.BackColorChanged -= _backColorChangedHandler;
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
			_inFocus = false;
		}
		protected virtual void OnDomDocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
		{
			_browserDocumentLoaded = true;  // Document loaded once
			AdjustHeight();
		}

		protected void Delay(int ms, EventHandler action)
		{
			if (_timer == null)
			{
				_timer = new Timer {Interval = ms};
				_timer.Tick += action;
				_timer.Start();
			}
		}

		protected void ChangeFocus(object sender, EventArgs e)
		{
			// Kill the timer if set and release the timer and HTML Element
			if (_timer != null)
			{
				_timer.Stop();
				_timer = null;
			}
			nsIDOMElement focusedElement = _browser.WebBrowserFocus.GetFocusedElementAttribute();
			if (focusedElement==_focusElement.DOMElement)
			{
				return;
			}
			nsIDOMElement start = focusedElement;
			while (start != null && start != _focusElement.DomObject)
			{
				nsIDOMNode parentNode = start.GetParentNodeAttribute();
				if (!(parentNode is nsIDOMElement))
				{
					start = null;
					break;
				}
				start = (nsIDOMElement)parentNode;
			}
			if ((start == null) && (_focusElement != null))
			{
				_browser.WebBrowserFocus.SetFocusedElementAttribute((nsIDOMElement)_focusElement.DomObject);
			}
		}
		// Making these empty handlers rather than abstract so the class only
		// needs to implement the ones they need.
		protected virtual void OnDomFocus(object sender, DomEventArgs e)
		{
		}
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
	}
}
