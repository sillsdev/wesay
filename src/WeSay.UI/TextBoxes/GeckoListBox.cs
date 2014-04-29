using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gecko;
using Gecko.DOM;
using Palaso.Reporting;
using Palaso.WritingSystems;

namespace WeSay.UI.TextBoxes
{
	public partial class GeckoListBox : UserControl, IControlThatKnowsWritingSystem
	{
		private GeckoWebBrowser _browser;
		private bool _browserIsReadyToNavigate;
		private bool _browserDocumentLoaded;
		private bool _initialSelectLoad;
		private int _pendingInitialIndex;
		private string _pendingHtmlLoad;
		private IWritingSystemDefinition _writingSystem;
		private bool _keyPressed;
		private GeckoSelectElement _selectElement;
		private GeckoBodyElement _bodyElement;
		private EventHandler _loadHandler;
		private EventHandler<GeckoDomKeyEventArgs> _domKeyDownHandler;
		private EventHandler<GeckoDomEventArgs> _domFocusHandler;
		private EventHandler<GeckoDomEventArgs> _domBlurHandler;
		private EventHandler _domDocumentChangedHandler;
		private EventHandler _backColorChangedHandler;
		private readonly string _nameForLogging;
		private bool _inFocus;
		private List<Object> _items;
		private readonly StringBuilder _itemHtml;
		public event EventHandler SelectedValueChanged;

		public GeckoListBox()
		{
			InitializeComponent();

			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
			Name = _nameForLogging;
			_keyPressed = false;
			ReadOnly = false;
			_inFocus = false;
			_initialSelectLoad = false;
			_pendingInitialIndex = -1;
			_items = new List<object>();
			_itemHtml = new StringBuilder();

			var designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			if (designMode)
				return;

			Debug.WriteLine("New GeckoListBox");
			_browser = new GeckoWebBrowser();
			_browser.Dock = DockStyle.Fill;
			_browser.Parent = this;
			_loadHandler = new EventHandler(GeckoBox_Load);
			this.Load += _loadHandler;
			Controls.Add(_browser);

			_domKeyDownHandler = new EventHandler<GeckoDomKeyEventArgs>(OnDomKeyDown);
			_browser.DomKeyDown += _domKeyDownHandler;
			_domFocusHandler = new EventHandler<GeckoDomEventArgs>(_browser_DomFocus);
			_browser.DomFocus += _domFocusHandler;
			_domBlurHandler = new EventHandler<GeckoDomEventArgs>(_browser_DomBlur);
			_browser.DomBlur += _domBlurHandler;
			_domDocumentChangedHandler = new EventHandler(_browser_DomDocumentChanged);
			_browser.DocumentCompleted += _domDocumentChangedHandler;
			_backColorChangedHandler = new EventHandler(OnBackColorChanged);
			this.BackColorChanged += _backColorChangedHandler;

		}

		public GeckoListBox(IWritingSystemDefinition ws, string nameForLogging)
			: this()
		{
			_nameForLogging = nameForLogging;
			if (_nameForLogging == null)
			{
				_nameForLogging = "??";
			}
			Name = _nameForLogging;
			WritingSystem = ws;
		}

		public void Clear()
		{
			_items.Clear();
			_itemHtml.Clear();
		}

		public void Closing()
		{
			Clear();
			this.Load -= _loadHandler;
			_browser.DomKeyDown -= _domKeyDownHandler;
			_browser.DomFocus -= _domFocusHandler;
			_browser.DomBlur -= _domBlurHandler;
			_browser.DocumentCompleted -= _domDocumentChangedHandler;
			this.BackColorChanged -= _backColorChangedHandler;
			_items = null;
			_loadHandler = null;
			_domKeyDownHandler = null;
			_domFocusHandler = null;
			_domDocumentChangedHandler = null;
			_backColorChangedHandler = null;
			_browser.Stop();
			_browser.Dispose();
			_browser = null;
		}

		public void AddItem(Object item)
		{
			_items.Add(item);
			var paddedItem = Regex.Replace(item.ToString(), @"(?<=^\s*)\s", "&nbsp;");
			_itemHtml.AppendFormat("<option value=\"{0}\">{1}</option>", item.ToString().Trim(), paddedItem);
		}

		public Object SelectedItem
		{
			get
			{
				if (_browser.Document == null)
				{
					return null;
				}
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				if (content != null)
				{
					return (_items[content.SelectedIndex]);
				}
				return null;
			}

		}

		public int SelectedIndex
		{
			get
			{
				if (_browser.Document == null)
				{
					return -1;
				}
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				if (content != null)
				{
					return (content.SelectedIndex);
				}
				return -1;
			}
			set
			{
				if (!_browserDocumentLoaded)
				{
					_pendingInitialIndex = value;
				}
				if (_browser.Document == null)
				{
					return;
				}
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				if (content != null)
				{
					content.SelectedIndex = value;
				}
			}
		}
		public int Length
		{
			get
			{
				return _items.Count;
			}
		}

		public List<Object> Items
		{
			get
			{
				return _items;
			}
		}

		public String SelectedText
		{
			get
			{
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				if (content != null)
				{
					return (content.Value);
				}
				return null;
			}
		}

		private String SelectStyle()
		{
			String justification = "left";
			if (_writingSystem != null && WritingSystem.RightToLeftScript)
			{
				justification = "right";
			}

			return String.Format("min-height:15px; font-family:{0}; font-size:{1}pt; text-align:{2}; font-weight:{3}; background:{4}; width:{5}",
				Font.Name,
				Font.Size, justification,
				Font.Bold ? "bold" : "normal",
				System.Drawing.ColorTranslator.ToHtml(BackColor),
				this.Width);
		}
		public void ListCompleted()
		{
			_initialSelectLoad = false;

			var html = new StringBuilder();
			html.Append("<!DOCTYPE html>");
			html.Append("<html><header><meta charset=\"UTF-8\">");
			html.Append("<script type='text/javascript'>");
			html.Append(" function fireEvent(name, data)");
			html.Append(" {");
			html.Append("   event = document.createEvent('MessageEvent');");
			html.Append("   event.initMessageEvent(name, false, false, data, null, null, null, null);");
			html.Append("   document.dispatchEvent(event);");
			html.Append(" }");
			html.Append("</script>");
			html.Append("</head>");
			html.AppendFormat("<body style='background:{0}; width:{1}; overflow-x:hidden' id='mainbody'>",
				System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(255,203,255,185)),
				this.Width);
			html.Append("<select size='10' id='itemList' style='" + SelectStyle() + "' onchange=\"fireEvent('selectChanged','changed');\">");
			// The following line is removed at this point and done later as a change to the inner
			// html because otherwise the browser blows up because of the length of the
			// navigation line.  Leaving this and this comment in as a warning to anyone who
			// may be tempted to try the same thing.
			// html.Append(_itemHtml);
			html.Append("</select></body></html>");
			SetHtml(html.ToString());
		}
		private void OnSelectedValueChanged(String s)
		{
			if (SelectedValueChanged != null)
			{
				SelectedValueChanged.Invoke(this, null);
			}
		}
		private void OnBackColorChanged(object sender, EventArgs e)
		{
			// if it's already loaded, change it
			if (_initialSelectLoad)
			{
				var content = (GeckoSelectElement) _browser.Document.GetElementById("itemList");
				if (content != null)
				{
					content.SetAttribute("style", SelectStyle());
				}
			}
		}
		private void _browser_DomDocumentChanged(object sender, EventArgs e)
		{
			_browserDocumentLoaded = true;  // Document loaded once
			if (!_initialSelectLoad)
			{
				_initialSelectLoad = true;
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				content.InnerHtml = _itemHtml.ToString();
			}
			if (_pendingInitialIndex > -1)
			{
				SelectedIndex = _pendingInitialIndex;
				_pendingInitialIndex = -1;
			}
			AdjustHeight();
		}

		void AdjustHeight()
		{
			if (_browser.Document == null)
			{
				return;
			}
			var content = _browser.Document.GetElementById("mainbody");
			if (content != null)
			{
				if (content is GeckoBodyElement)
				{
					_bodyElement = (GeckoBodyElement)content;
					Height = _bodyElement.Parent.ScrollHeight;
				}
			}
		}

		private delegate void ChangeFocusDelegate(GeckoSelectElement ctl);
		private void _browser_DomFocus(object sender, GeckoDomEventArgs e)
		{
#if DEBUG
			Debug.WriteLine("Got Focus: " + Text);
#endif
			var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
			if (content != null)
			{
				// The following is required because we get two in focus events every time this
				// is entered.  This is normal for Gecko.  But I don't want to be constantly
				// refocussing.
				if (!_inFocus)
				{
					_inFocus = true;
#if DEBUG
					Debug.WriteLine("Got Focus2: " + Text);
#endif
					_selectElement = (GeckoSelectElement)content;
					this.BeginInvoke(new ChangeFocusDelegate(changeFocus), _selectElement);
				}
			}
		}
		private void _browser_DomBlur(object sender, GeckoDomEventArgs e)
		{
			_inFocus = false;
#if DEBUG
			Debug.WriteLine("Got Blur: " + Text);
#endif
		}

		private void changeFocus(GeckoSelectElement ctl)
		{
#if DEBUG
			Debug.WriteLine("Change Focus: " + Text);
#endif
			ctl.Focus();
		}


		private void OnDomKeyDown(object sender, GeckoDomKeyEventArgs e)
		{
			if (_inFocus)
			{
				if ((e.KeyCode == 9) && !e.CtrlKey && !e.AltKey)
				{
					int a = ParentForm.Controls.Count;
#if DEBUG
					Debug.WriteLine ("Got a Tab Key " );
#endif
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
				}
				else
				{
					this.RaiseKeyEvent(Keys.A, new KeyEventArgs(Keys.A));
				}
			}
		}

		private void GeckoBox_Load(object sender, EventArgs e)
		{
			_browserIsReadyToNavigate = true;
			_browser.AddMessageEventListener("selectChanged", ((string s) => this.OnSelectedValueChanged(s)));
			if (_pendingHtmlLoad != null)
			{
#if DEBUG
				Debug.WriteLine("Load: " + _pendingHtmlLoad);
#endif
				SetHtml(_pendingHtmlLoad);
				_pendingHtmlLoad = null;
			}
		}

		public void SetHtml(string html)
		{
			if (!_browserIsReadyToNavigate)
			{
				_pendingHtmlLoad = html;
			}
			else
			{
				Debug.WriteLine("SetHTML: " + html);
				const string type = "text/html";
				var bytes = System.Text.Encoding.UTF8.GetBytes(html);
				_browser.Navigate(string.Format("data:{0};base64,{1}", type, Convert.ToBase64String(bytes)),
					GeckoLoadFlags.BypassHistory);

//				_browser.LoadHtml(html);
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

		public bool MultiParagraph { get; set; }

		public bool IsSpellCheckingEnabled { get; set; }


		public int SelectionStart
		{
			get
			{
				//TODO
				return 0;
			}
			set
			{
				//TODO
			}
		}


		public bool ReadOnly { get; set; }



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

	}
}
