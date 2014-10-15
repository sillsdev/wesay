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
using Gecko.Events;
using Palaso.Reporting;
using Palaso.WritingSystems;

namespace WeSay.UI.TextBoxes
{
	public partial class GeckoComboBox : GeckoBase, IControlThatKnowsWritingSystem, IWeSayComboBox
	{
		private bool _initialSelectLoad;
		private int _pendingInitialIndex;
		private string _pendingHtmlLoad;
		private bool _keyPressed;

		private List<Object> _items;
		private readonly StringBuilder _itemHtml;
		public event EventHandler SelectedValueChanged;
		public event DrawItemEventHandler DrawItem;
		public event MeasureItemEventHandler MeasureItem;

		public GeckoComboBox()
		{
			InitializeComponent();

			_keyPressed = false;
			_initialSelectLoad = false;
			_pendingInitialIndex = -1;
			_items = new List<object>();
			_itemHtml = new StringBuilder();

			var designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			if (designMode)
				return;

			Debug.WriteLine("New GeckoComboBox");
		}

		public GeckoComboBox(IWritingSystemDefinition ws, string nameForLogging)
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
			if (_items != null)
			{
				_items.Clear();
			}
			_itemHtml.Clear();
		}

		protected override void Closing()
		{
			Clear();

			_items = null;
			base.Closing();
		}

		public void AddItem(Object item)
		{
			_items.Add(item);
			var paddedItem = System.Security.SecurityElement.Escape(item.ToString());
			paddedItem = Regex.Replace(paddedItem, @"(?<=^\s*)\s", "&nbsp;");
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
				var content = (GeckoSelectElement)_browser.Document.GetElementById("main");
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
				var content = (GeckoSelectElement)_browser.Document.GetElementById("main");
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
				var content = (GeckoSelectElement)_browser.Document.GetElementById("main");
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

		public Object GetItem(int i)
		{
			return this._items[i];
		}

		public String SelectedText
		{
			get
			{
				var content = (GeckoSelectElement)_browser.Document.GetElementById("main");
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
			return String.Format("min-height:15px; height:inherit; font-family:{0}; font-size:{1}pt; text-align:{2}; font-weight:{3}; background:{4}; width:{5}",
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
			html.Append("<html><head><meta charset=\"UTF-8\">");
			html.Append("<style>");
			html.AppendLine("@font-face {");
			html.AppendFormat("    font-family: \"{0}\";\n", Font.Name);
			html.AppendFormat("    src: local(\"{0}\");\n", Font.Name);
			html.AppendLine("}");
			html.Append("</style>");
			html.Append("<script type='text/javascript'>");
			html.Append(" function fireEvent(name, data)");
			html.Append(" {");
			html.Append("   var event = new MessageEvent(name, {'data' : data});");
			html.Append("   document.dispatchEvent(event);");
			html.Append(" }");
			html.Append("</script>");
			html.Append("</head>");
			html.AppendFormat("<body {2} style='background:{0}; width:{1}; overflow-x:hidden' id='mainbody'>",
				System.Drawing.ColorTranslator.ToHtml(Color.FromArgb(255,203,255,185)),
				this.Width,
				GetLanguageHtml(_writingSystem));
			html.Append("<select id='main' style='" + SelectStyle() + "' onchange=\"fireEvent('selectChanged','changed');\">");
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

		protected override void OnDomClick(object sender, DomMouseEventArgs e)
		{
			_browser.Focus ();
		}

		protected override void OnDomDocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
		{
			base.OnDomDocumentCompleted(sender, e);
			if (!_initialSelectLoad)
			{
				_initialSelectLoad = true;
				var content = (GeckoSelectElement)_browser.Document.GetElementById("main");
				content.InnerHtml = _itemHtml.ToString();
			}
			if (_pendingInitialIndex > -1)
			{
				SelectedIndex = _pendingInitialIndex;
				_pendingInitialIndex = -1;
			}
			AdjustHeight();
		}

		protected override void OnBackColorChanged(object sender, EventArgs e)
		{
			// if it's already loaded, change it
			if (_initialSelectLoad)
			{
				var content = (GeckoSelectElement) _browser.Document.GetElementById("main");
				if (content != null)
				{
					content.SetAttribute("style", SelectStyle());
				}
			}
		}

		protected override void OnGeckoBox_Load(object sender, EventArgs e)
		{
			_browserIsReadyToNavigate = true;
			_browser.AddMessageEventListener("selectChanged", ((string s) => this.OnSelectedValueChanged(s)));
			if (_pendingHtmlLoad != null)
			{
				SetHtml(_pendingHtmlLoad);
				_pendingHtmlLoad = null;
			}
		}

		private void SetHtml(string html)
		{
			if (!_browserIsReadyToNavigate)
			{
				_pendingHtmlLoad = html;
			}
			else
			{
				const string type = "text/html";
				var bytes = System.Text.Encoding.UTF8.GetBytes(html);
				_browser.Navigate(string.Format("data:{0};base64,{1}", type, Convert.ToBase64String(bytes)),
					GeckoLoadFlags.BypassHistory);
			}
		}
		public bool Sorted { get; set; }
		public AutoCompleteSource AutoCompleteSource { get; set; }
		public AutoCompleteMode AutoCompleteMode { get; set; }
		public ComboBoxStyle DropDownStyle { get; set; }
		public int MaxDropDownItems { get; set; }


	}
}
