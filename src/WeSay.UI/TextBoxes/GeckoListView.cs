using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;
using Gecko;
using Gecko.DOM;
using Palaso.DictionaryServices.Model;
using Palaso.Data;
using Palaso.i18n;
using Palaso.Reporting;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI.TextBoxes
{
	public partial class GeckoListView : GeckoBase, IControlThatKnowsWritingSystem, IWeSayListView
	{
		private bool _initialSelectLoad;
		private int _pendingInitialIndex;
		private string _pendingHtmlLoad;
		private GeckoSelectElement _selectElement;
		private GeckoOptionElement _optionElement;
		private int _optionHeight;
		private List<Object> _items;
		private readonly StringBuilder _itemHtml;
		public event ListViewItemSelectionChangedEventHandler ItemSelectionChanged;
		public event RetrieveVirtualItemEventHandler RetrieveVirtualItem;
		private IList _dataSource;

		public GeckoListView()
		{
			InitializeComponent();

			_initialSelectLoad = false;
			_pendingInitialIndex = -1;
			_items = new List<object>();
			_itemHtml = new StringBuilder();
			MaxLength = 50;  // Default value
			_optionHeight = 10; // Default value
			_handleEnter = false;

			var designMode = (LicenseManager.UsageMode == LicenseUsageMode.Designtime);
			if (designMode)
				return;

			Debug.WriteLine("New GeckoListView");
		}

		public GeckoListView(IWritingSystemDefinition ws, string nameForLogging)
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
			var paddedItem = item.ToString();
			if (paddedItem.Length < MinLength)
			{
				// Add characters to fill at the end of the string to the min length
				if (_writingSystem != null && WritingSystem.RightToLeftScript)
				{
					paddedItem = paddedItem.PadLeft(MinLength);
				}
				else
				{
					paddedItem = paddedItem.PadRight(MinLength);
				}
			}
			if ((MaxLength > MinLength) && (paddedItem.Length > MaxLength))
			{
				// Truncate items from the end of the string and attach an elipsis to
				// the appropriate end of the string
				if (_writingSystem != null && WritingSystem.RightToLeftScript)
				{
					paddedItem = String.Format("...{0}", paddedItem.Remove(MaxLength - 4));
				}
				else
				{
					paddedItem = String.Format("{0}...", paddedItem.Remove(MaxLength - 4));
				}
			}
			// replace leading and trailing spaces that may have been added with
			// the &nbsp markers.
			paddedItem = Regex.Replace(paddedItem, @"(?<=^\s*)\s", "&nbsp;");
			paddedItem = Regex.Replace(paddedItem, @"\s(?=\s*$)", "&nbsp;");
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
				if (!_initialSelectLoad || !_browserDocumentLoaded || _browser.Document == null)
				{
					_pendingInitialIndex = value;
					return;
				}
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				if (content != null)
				{
					content.SelectedIndex = value;
				}
			}
		}

		public int MaxLength { get; set; }

		public int MinLength { get; set; }

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
		protected override void OnResize(EventArgs e)
		{
			AdjustHeight();
			base.OnResize(e);
		}
		private String SelectStyle()
		{
			String justification = "left";
			if (_writingSystem != null && WritingSystem.RightToLeftScript)
			{
				justification = "right";
			}
			Font font = WritingSystemInfo.CreateFont(_writingSystem);
			return String.Format("min-height:15px; font-family:{0}; font-size:{1}pt; text-align:{2}; font-weight:{3}; background:{4}; width:{5}",
				font.Name,
				font.Size,
				justification,
				Bold ? "bold" : "normal",
				System.Drawing.ColorTranslator.ToHtml(BackColor),
				this.Width);
		}
		public void ListCompleted()
		{
			_initialSelectLoad = false;

			var html = new StringBuilder();
			html.Append("<!DOCTYPE html>");
			html.Append("<html><head><meta charset=\"UTF-8\">");
			html.Append("<script type='text/javascript'>");
			html.Append(" function fireEvent(name, data)");
			html.Append(" {");
			html.Append("   event = document.createEvent('MessageEvent');");
			html.Append("   event.initMessageEvent(name, false, false, data, null, null, null, null);");
			html.Append("   document.dispatchEvent(event);");
			html.Append(" }");
			html.Append("</script>");
			html.Append("</head>");
			html.AppendFormat("<body style='background:{0}; width:{1}' id='mainbody'>",
				System.Drawing.ColorTranslator.ToHtml(Color.White),
				this.Width);
			html.Append("<select size='10' id='itemList' style='" + SelectStyle() + "' onchange=\"fireEvent('selectChanged','changed');\">");
			// The following line is removed at this point and done later as a change to the inner
			// html because otherwise the browser blows up because of the length of the
			// navigation line.  Leaving this and this comment in as a warning to anyone who
			// may be tempted to try the same thing.
			// html.Append(_itemHtml);

			// Adding a one character option instead to get an accurate height for the font
			html.Append("<option id='optionElement' value=\" \">&nbsp</option>");
			html.Append("</select></body></html>");
			SetHtml(html.ToString());
		}
		private void OnSelectedValueChanged(String s)
		{
			if (ItemSelectionChanged != null)
			{
				ItemSelectionChanged.Invoke(this, new ListViewItemSelectionChangedEventArgs(null, SelectedIndex, true));
			}
		}
		protected override void OnBackColorChanged(object sender, EventArgs e)
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
		protected override void OnDomDocumentCompleted(object sender, EventArgs e)
		{
			base.OnDomDocumentCompleted(sender, e);
			if (!_initialSelectLoad)
			{
				_initialSelectLoad = true;
				var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
				content.InnerHtml = _itemHtml.ToString();
			}
			if (_pendingInitialIndex > -1)
			{
				SelectedIndex = _pendingInitialIndex;
				if (ItemSelectionChanged != null)
				{
					ItemSelectionChanged.Invoke(this, new ListViewItemSelectionChangedEventArgs(null, _pendingInitialIndex, true));
				}
				_pendingInitialIndex = -1;
			}
		}

		protected override void AdjustHeight()
		{
			if ((_browser == null) ||(_browser.Document == null))
			{
				return;
			}

			if (_optionElement == null)
			{
				var content = (GeckoOptionElement) _browser.Document.GetElementById("optionElement");
				if (content != null)
				{
					_optionElement = (GeckoOptionElement) content;
					_optionHeight = _optionElement.ClientHeight;
					_selectElement = (GeckoSelectElement) _optionElement.Parent;
				}
				else
				{
					return;
				}
			}
			var selectElement = (GeckoSelectElement) _browser.Document.GetElementById("itemList");
			if (selectElement != null)
			{
				int numberOfEntries = (Height/(_optionHeight + 1)) - 1;
				if (numberOfEntries <= 0)
				{
					numberOfEntries = 10;
				}
				selectElement.Size = numberOfEntries;
			}
		}

		private delegate void ChangeFocusDelegate(GeckoSelectElement ctl);
		protected override void OnDomFocus(object sender, GeckoDomEventArgs e)
		{
			var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
			if (content != null)
			{
				// The following is required because we get two in focus events every time this
				// is entered.  This is normal for Gecko.  But I don't want to be constantly
				// refocussing.
				if (!_inFocus)
				{
					_inFocus = true;
					_selectElement = (GeckoSelectElement)content;
					this.BeginInvoke(new ChangeFocusDelegate(changeFocus), _selectElement);
				}
			}
		}

		private void changeFocus(GeckoSelectElement ctl)
		{
			ctl.Focus();
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
			}
		}

		[DefaultValue(null)]
		public IList DataSource
		{
			get { return _dataSource; }
			set
			{
				Clear();
				_dataSource = value;
				foreach (var recordEntry in _dataSource)
				{
					LexEntry entry = null;
					if (recordEntry is LexEntry)
					{
						entry = (LexEntry)recordEntry;
					}
					else if (recordEntry is RecordToken<LexEntry>)
					{
						entry = ((RecordToken<LexEntry>) recordEntry).RealObject;
					}
					if (entry != null)
					{
						var form = entry.GetHeadWordForm(_writingSystem.Id);
						if (string.IsNullOrEmpty(form))
						{
							form = entry.ToString();
							if (string.IsNullOrEmpty(form))
							{
								form = "(";
								form += StringCatalog.Get("~Empty",
																	"This is what shows for a word in a list when the user hasn't yet typed anything in for the word.  Like if you click the 'New Word' button repeatedly.");
								form += ")";								//this is only going to come up with something in two very unusual cases:
								//1) a monolingual dictionary (well, one with meanings in the same WS as the lexical units)
								//2) the SIL CAWL list, where the translator adds glosses, and fails to add
								//lexical entries.
								//form = entry.GetSomeMeaningToUseInAbsenseOfHeadWord(_writingSystem.Id);
							}
						}
						AddItem(form);
					}
					else
					{
						AddItem(recordEntry.ToString());
					}
				}
				if (!_browserDocumentLoaded)
				{
					ListCompleted();
				}
				else
				{
					var content = (GeckoSelectElement)_browser.Document.GetElementById("itemList");
					content.InnerHtml = _itemHtml.ToString();
				}
			}
		}
		// Set this to true to get bold print for the list box
		public bool Bold { get; set; }

		public bool IsSpellCheckingEnabled { get; set; }
		public int VirtualListSize { get; set; }

		public int SelectionStart
		{
			get
			{
				return 0;
			}
			set
			{
			}
		}
	}
}
