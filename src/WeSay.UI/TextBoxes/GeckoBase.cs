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
		public IWritingSystemDefinition WritingSystem
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
				else if ((e.KeyCode == (uint)Keys.Tab) && !e.CtrlKey && !e.AltKey)
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
				OnKeyDown(new KeyEventArgs((Keys)e.KeyCode));
			}
		}

		public virtual bool Bold { get; set; }

		public virtual int SelectionStart { get; set; }
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
