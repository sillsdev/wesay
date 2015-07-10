using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gecko;
using Gecko.DOM;
using Gecko.Events;
using SIL.Reporting;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI.TextBoxes
{
	public partial class GeckoListBox : GeckoBase, IControlThatKnowsWritingSystem, IWeSayListBox
	{
		private bool _initialSelectLoad;
		private int _pendingInitialIndex;
		private string _pendingHtmlLoad;
		private List<Object> _items;
		private readonly StringBuilder _itemHtml;
		public event EventHandler UserClick;
		private int _numberOfItemsInColumn;
		private object _itemToNotDrawYet;
		protected WritingSystemDefinition _meaningWritingSystem;
		protected int _selectedIndex;
		public event EventHandler ListLostFocus;
		private static List<string> _fontFamilies;
		private static StringBuilder _fontFamiliesStyle;

		public GeckoListBox()
		{
			InitializeComponent();

			_handleEnter = false;
			_initialSelectLoad = false;
			_pendingInitialIndex = -1;
			_items = new List<object>();
			_itemHtml = new StringBuilder();
			_numberOfItemsInColumn = 3;
			ItemDrawer = DefaultDrawItem;
			HighlightSelect = false;
			_fontFamilies = new List<string>();
			_fontFamiliesStyle = new StringBuilder();

			var designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			if (designMode)
				return;

		}

		public GeckoListBox(WritingSystemDefinition ws, string nameForLogging)
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
			SelectedIndex = -1;
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
		}
		public void AddRange(Object[] items)
		{
			this.Items.AddRange(items);
		}
		public object GetItem(int index)
		{
			return Items[index];
		}

		// The WeSayListBox doesn't use writing system but does use FormWritingSystem
		// and in some cases MeaningWritingSystem.  For this, FormWritingSystem will
		// become another alias for WritingSystem
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystemDefinition FormWritingSystem
		{
			get
			{
				return WritingSystem;
			}
			set
			{
				Font = WritingSystemInfo.CreateFont(value); // This makes column width calculation work
				WritingSystem = value;
				AddFontFamily(value);
			}
		}
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystemDefinition MeaningWritingSystem
		{
			get
			{
				return _meaningWritingSystem;
			}
			set
			{
				_meaningWritingSystem = value;
				AddFontFamily(value);
			}
		}
		public Object SelectedItem
		{
			get
			{
				if ((_items != null) && (SelectedIndex > -1) && (SelectedIndex < _items.Count))
				{
					return (_items[SelectedIndex]);
				}
				return null;
			}

		}

		public int SelectedIndex
		{
			get
			{
				return _selectedIndex;
			}
			set
			{
				int oldIndex = _selectedIndex;
				_selectedIndex = value;
				if (_selectedIndex > -1)
				{
					if (HighlightSelect)
					{
						string id = _selectedIndex + "-1";
						var content = (GeckoLIElement) _browser.Document.GetElementById(id);
						if (content != null)
						{
							content.SetAttribute("class", "selected");
							content.ScrollIntoView(false);
						}
					}
				}
				else
				{
					// -1 is the only valid negative value
					_selectedIndex = -1;
				}
				if ((oldIndex > -1) && HighlightSelect && (_browser != null) && (_browser.Document != null))
				{
					string id = oldIndex + "-1";
					var content = (GeckoLIElement)_browser.Document.GetElementById(id);
					if (content != null)
					{
						content.RemoveAttribute("class");
					}
				}
			}
		}

		public bool HighlightSelect { get; set; }

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

		public Rectangle GetItemRectangle(int index)
		{
			Rectangle itemRectangle = this.ClientRectangle; // Default
			if (!((_browser == null) || (_browser.Document == null)))
			{

				string id = index.ToString() + "-1";
				var content = (GeckoLIElement)_browser.Document.GetElementById(id);
				if (content != null)
				{
					nsIDOMClientRect domRect = content.DOMHtmlElement.GetBoundingClientRect();
					itemRectangle = new Rectangle((int)domRect.GetLeftAttribute(), (int)domRect.GetTopAttribute(), (int)domRect.GetWidthAttribute(), (int)domRect.GetHeightAttribute());
				}
			}
			return itemRectangle;
		}

		// Called from the ItemDrawer routine from the data source to add the item to the
		// html being built by CreateHtmlFromItems
		public void ItemToHtml(string word, int index, bool useFormWS, Color textColor)
		{
			WritingSystemDefinition ws = useFormWS ? FormWritingSystem : MeaningWritingSystem;
			Font font = WritingSystemInfo.CreateFont(ws );
			String entry = String.IsNullOrEmpty(word) ? "&nbsp;" : System.Security.SecurityElement.Escape(word);
			String subId = useFormWS ? "-1" : "-2";
			String id = index.ToString() + subId;
			_itemHtml.AppendFormat("<li id='{2}' {5} style='font-family:{3}; font-size:{4}pt;' onclick=\"fireEvent('selectChanged','{0}');\">{1}</li>",
				index.ToString(), entry, id, font.Name, font.Size, GetLanguageHtml(ws));
		}
		/// <summary>
		/// Change this if you need to draw something special. THe default just draws the string of the item.
		/// Make sure to make a custom MeasureItem handler too!
		///
		/// <param name="item"> The first object is the object to be added to the list and will be interpreted by the caller
		/// in the drawing routine and when accessed.  If the default drawer is used, it is assumed that it can be represented
		/// in the list by ToString.</param>
		/// <param name="a" > The second parameter when calling the GeckoListBox will be the index of the item, which is used
		/// to identify which record has been selected.  Since multiple items in the list can reference the same index (i.e.
		/// the word and its meaning) this parameter allows the code to know which item in the items array this entry is
		/// associated with.</param>
		/// </summary>
		public Action<object, object> ItemDrawer { get; set; }
		private void DefaultDrawItem(object item, object a)
		{
			int itemIndex = (int)a;
			ItemToHtml(item.ToString(), itemIndex, true, Color.Black);
		}

		private String CreateHtmlFromItems()
		{
			int itemsAdded = 0;
			if (DisplayMeaning)
			{
				_numberOfItemsInColumn = Height / 65;
			}
			else
			{
				_numberOfItemsInColumn = Height / 35;
			}
			if (_numberOfItemsInColumn == 0)
			{
				_numberOfItemsInColumn = 1;
			}
			_itemHtml.Clear();
			_itemHtml.Append("<ul id='main'><li><ul>"); // Initial Column
			for (int index = 0; index < _items.Count; index++)
			{
				if (_items[index] != _itemToNotDrawYet)
				{
					if (MultiColumn && ((itemsAdded % _numberOfItemsInColumn) == 0))
					{
						if (itemsAdded != 0)
						{
							// Time to start a new column
							_itemHtml.Append("</ul></li><li><ul>");
						}
					}
					ItemDrawer(_items[index], index);
					itemsAdded++;
				}
			}
			// Finish the last column and the block
			_itemHtml.Append("</ul></li></ul>");
			return _itemHtml.ToString();
		}

		public void ListCompleted()
		{
			_initialSelectLoad = false;
			CreateHtmlFromItems();

			var html = new StringBuilder();
			html.Append("<!DOCTYPE html>");
			html.Append("<html><head><meta charset=\"UTF-8\">");
			html.Append("<style>");
			html.Append(_fontFamiliesStyle);
			html.Append("</style>");
			html.Append("<script type='text/javascript'>");
			html.Append(" function fireEvent(name, data)");
			html.Append(" {");
			html.Append("   var event = new MessageEvent(name, {'data' : data});");
			html.Append("   document.dispatchEvent(event);");
			html.Append(" }");
			html.Append("</script>");
			html.Append("<style>");
			html.Append("ul { border: 0px solid black; display: inline-block; margin:0px; padding:0px; } ");
			html.Append("ul li { display: inline-block; list-style: none; vertical-align: top: } ");
			html.Append("ul li ul { border: 0px; padding: 0px; } ");
			html.AppendFormat("ul li ul li {{ cursor: pointer; display: list-item; white-space: nowrap; height:30px; list-style: none; text-align:left; width: {0}px; color:black; }} ", ColumnWidth.ToString());
			html.Append("ul li ul li.selected { color:blue; } ");
			html.Append("ul li ul li.hover { background-color: #CCCCCC; } ");
			html.Append("</style>");
			html.Append("</head>");
			html.AppendFormat("<body style='background:{0}; width:{1}; overflow-x:hidden' id='mainbody'><ul id='main'></ul>",
				System.Drawing.ColorTranslator.ToHtml(BackColor),
				this.Width);

			// The following line is removed at this point and done later as a change to the inner
			// html because otherwise the browser blows up because of the length of the
			// navigation line.  Leaving this and this comment in as a warning to anyone who
			// may be tempted to try the same thing.
			html.Append("</body></html>");
			SetHtml(html.ToString());
		}
		private void OnSelectedValueChanged(String s)
		{
			try
			{
				SelectedIndex = int.Parse(s);
			}
			catch (Exception e)
			{
				SelectedIndex = 0;  // Shouldn't happen, but set to first item if it does
			}
			if (UserClick != null)
			{
				UserClick.Invoke(this, null);
			}
		}
		protected override void OnDomDocumentCompleted(object sender, GeckoDocumentCompletedEventArgs e)
		{
			base.OnDomDocumentCompleted(sender, e);
			if (!_initialSelectLoad)
			{
				_initialSelectLoad = true;
				var content = (GeckoBodyElement)_browser.Document.GetElementById("mainbody");
				content.InnerHtml = _itemHtml.ToString();
			}
			if (_pendingInitialIndex > -1)
			{
				SelectedIndex = _pendingInitialIndex;
				_pendingInitialIndex = -1;
			}
		}

		protected override void OnResize(EventArgs e)
		{
			AdjustHeight();
			base.OnResize(e);
		}

		protected override void AdjustHeight()
		{
			if ((_browser == null) || (_browser.Document == null))
			{
				return;
			}

			var content = (GeckoBodyElement)_browser.Document.GetElementById("mainbody");
			if (content != null)
			{
				content.InnerHtml = CreateHtmlFromItems();
			}
		}

		protected override void OnDomFocus(object sender, DomEventArgs e)
		{
			var content = (GeckoUListElement)_browser.Document.GetElementById("main");
			if (content != null)
			{
				// The following is required because we get two in focus events every time this
				// is entered.  This is normal for Gecko.  But I don't want to be constantly
				// refocussing.
				if (!_inFocus)
				{
					_focusElement = (GeckoHtmlElement)content;
					base.OnDomFocus(sender, e);
				}
			}
		}

		protected override void OnDomBlur(object sender, DomEventArgs e)
		{
			if (_inFocus)
			{
				_inFocus = false;
				if (ListLostFocus != null)
				{
					ListLostFocus.Invoke(this, null);
				}
			}
		}


		protected override void OnGeckoBox_Load(object sender, EventArgs e)
		{
			_browserIsReadyToNavigate = true;
			_browser.AddMessageEventListener("selectChanged", ((string s) => this.OnSelectedValueChanged(s)));
			if (_pendingHtmlLoad != null)
			{
#if DEBUG
				//Debug.WriteLine("Load: " + _pendingHtmlLoad);
#endif
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
				//Debug.WriteLine("SetHTML: " + html);
				const string type = "text/html";
				var bytes = System.Text.Encoding.UTF8.GetBytes(html);
				_browser.Navigate(string.Format("data:{0};base64,{1}", type, Convert.ToBase64String(bytes)),
					GeckoLoadFlags.BypassHistory);
			}
		}
		private void AddFontFamily(WritingSystemDefinition ws)
		{
			if (ws != null)
			{
				Font font = WritingSystemInfo.CreateFont(ws);
				string fontFamily = font.Name;
				var match = _fontFamilies.FirstOrDefault(stringToCheck => stringToCheck.Equals(fontFamily));

				if (match == null)
				{
					_fontFamilies.Add(fontFamily);
					_fontFamiliesStyle.AppendLine("@font-face {");
					_fontFamiliesStyle.AppendFormat("    font-family: \"{0}\";\n", fontFamily);
					_fontFamiliesStyle.AppendFormat("    src: local(\"{0}\");\n", fontFamily);
					_fontFamiliesStyle.AppendLine("}");
				}
			}
		}


		//used when animating additions to the list
		public object ItemToNotDrawYet
		{
			get { return _itemToNotDrawYet; }
			set
			{
				_itemToNotDrawYet = value;
				if (_initialSelectLoad)
				{
					var content = (GeckoBodyElement)_browser.Document.GetElementById("mainbody");
					content.InnerHtml = CreateHtmlFromItems();
				}
			}
		}

		public Control Control
		{
			get
			{
				return this;
			}
		}

		public bool IsSpellCheckingEnabled { get; set; }
		public bool MultiColumn { get; set; }
		public bool DisplayMeaning { get; set; }
		public int ItemHeight { get; set; }
		public int ColumnWidth { get; set; }
		public bool Sorted { get; set; }
	}
}
