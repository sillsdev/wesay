// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using Gecko;
using SIL.Code;
using SIL.UiBindings;
using SIL.WritingSystems;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.LexicalModel.Foundation;
using WeSay.UI.TextBoxes;

namespace WeSay.UI.AutoCompleteTextBox
{
	/// <summary>
	/// Summary description for AutoCompleteTextBox.
	/// </summary>
	[Serializable]
	public class GeckoAutoCompleteTextBox : GeckoBox, IWeSayAutoCompleteTextBox
	{
		private FormToObjectFinderDelegate _formToObjectFinderDelegate;

		public event EventHandler SelectedItemChanged;
		private bool _inMidstOfSettingSelectedItem;

		#region Members

		private IDisplayStringAdaptor _itemDisplayAdaptor = new ToStringAutoCompleteAdaptor();
		private readonly GeckoListBox _listBox;
		private Control _popupParent;

		#endregion

		#region Properties

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public FormToObjectFinderDelegate FormToObjectFinder
		{
			get { return _formToObjectFinderDelegate; }
			set { _formToObjectFinderDelegate = value; }
		}

		private EntryMode mode = EntryMode.Text;

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public EntryMode Mode
		{
			get { return mode; }
			set
			{
				mode = value;
				if (Mode == EntryMode.List)
				{
					DisplayListIfTextTriggers();
				}
			}
		}

		private IEnumerable _items = new Collection<object>();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public IEnumerable Items
		{
			get { return _items; }
			set
			{
				_items = value;
				if (Mode == EntryMode.List)
				{
					DisplayListIfTextTriggers();
				}
			}
		}

		private AutoCompleteTriggerCollection triggers = new AutoCompleteTriggerCollection();

		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public AutoCompleteTriggerCollection Triggers
		{
			get { return triggers; }
			set { triggers = value; }
		}

		private bool _autoSizePopup = false;

		[Browsable(true)]
		[Description(
				"The width of the popup (-1 will auto-size the popup to the larger of the width of the textbox or the content of the list)."
				)]
		public int PopupWidth
		{
			get { return _listBox.Width; }
			set
			{
				if (value == -1)
				{
					_autoSizePopup = true;
					_listBox.Width = Width;
				}
				else
				{
					_autoSizePopup = false;
					_listBox.Width = value;
				}
			}
		}

		public BorderStyle PopupBorderStyle
		{
			get { return _listBox.BorderStyle; }
			set { _listBox.BorderStyle = value; }
		}

		private Point popOffset = new Point(0, 0);

		[Description("The popup defaults to the lower left edge of the textbox.")]
		public Point PopupOffset
		{
			get { return popOffset; }
			set { popOffset = value; }
		}

		private Color popSelectBackColor = SystemColors.Highlight;

		public Color PopupSelectionBackColor
		{
			get { return popSelectBackColor; }
			set { popSelectBackColor = value; }
		}

		private Color popSelectForeColor = SystemColors.HighlightText;

		public Color PopupSelectionForeColor
		{
			get { return popSelectForeColor; }
			set { popSelectForeColor = value; }
		}

		private bool triggersEnabled = true;

		protected bool TriggersEnabled
		{
			get { return triggersEnabled; }
			set { triggersEnabled = value; }
		}

		//nb: if we used an interface (e.g. IFilter) rather than a delegate, then the adaptor part wouldn't have to be
		// part of the interface, where it doesn't really fit. It would instead be added to the constructor of the filterer,
		// if appropriate.
		private ItemFilterDelegate _itemFilterDelegate;
		private object _selectedItem;
		private readonly ToolTip _toolTip;
		private object _previousToolTipTarget;
		private bool _windowJustGotFocus = true;

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ItemFilterDelegate ItemFilterer
		{
			get { return _itemFilterDelegate; }
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_itemFilterDelegate = value;
			}
		}

		[Browsable(true)]
		public override string Text
		{
			get { return base.Text; }
			set
			{
				//				TriggersEnabled = false;
				base.Text = value;
				//				TriggersEnabled = true;
			}
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IDisplayStringAdaptor ItemDisplayStringAdaptor
		{
			get { return _itemDisplayAdaptor; }
			set { _itemDisplayAdaptor = value; }
		}

		public object SelectedItem
		{
			get { return _selectedItem; }

			set
			{
				if (_inMidstOfSettingSelectedItem)
				{
					return;
				}

				if (_selectedItem == value)
				{
					//handle WS-1171, where a) a baseform was set b) the target was deleted c) the user deletes the now-displayed red id of the missing item
					//in this case, the target was null before and after the edit, but we need to notify that the edit happened, else it is lost
					if (string.IsNullOrEmpty(Text))
					{
						if (SelectedItemChanged != null)
						{
							SelectedItemChanged.Invoke(this, null);
						}
					}
					return;
				}
				_inMidstOfSettingSelectedItem = true;

				_selectedItem = value;
				if (value != null)
				{
					if (_itemDisplayAdaptor != null)
					{
						Text = _itemDisplayAdaptor.GetDisplayLabel(value);
						//keep cursor form jumping to start when user typed in
						//a matched value that varied by case
						SelectionStart = Text.Length;
					}
					else
					{
						Text = value.ToString();
					}
				}

				if (SelectedItemChanged != null)
				{
					SelectedItemChanged.Invoke(this, null);
				}
				//Debug.WriteLine("AutomCOmplete Set selectedItem(" + value + ") now " + _selectedItem);
				_inMidstOfSettingSelectedItem = false;
			}
		}

		public IWeSayListBox FilteredChoicesListBox
		{
			get { return _listBox; }
		}

		#endregion

		public GeckoAutoCompleteTextBox()
		{
			if (DesignMode)
			{
				return;
			}

			_formToObjectFinderDelegate = DefaultFormToObjectFinder;
			_itemFilterDelegate = FilterList;

			Leave += Popup_Deactivate;

			_toolTip = new ToolTip();
			MouseHover += OnMouseHover;

			// Create the list box that will hold matching items
			_listBox = new GeckoListBox();
			_listBox.MaximumSize = new Size(800, 100);
			_listBox.Cursor = Cursors.Hand;
			_listBox.BorderStyle = BorderStyle.FixedSingle;
			_listBox.MultiColumn = false;
			//_listBox.SelectedIndexChanged += List_SelectedIndexChanged;
			_listBox.UserClick += List_Click;
			_listBox.ListLostFocus += OnListLostFocus;
			_listBox.Enter += List_Enter;
			_listBox.Leave += List_Leave;
			_listBox.ItemHeight = _listBox.Font.Height;
			_listBox.Visible = false;
			_listBox.Sorted = false;
			_listBox.HighlightSelect = true;

			// Add default triggers.
			triggers.Add(new TextLengthTrigger(2));
			triggers.Add(new ShortCutTrigger(Keys.Enter, TriggerState.Select));
			triggers.Add(new ShortCutTrigger(Keys.Tab, TriggerState.Select));
			triggers.Add(new ShortCutTrigger(Keys.Control | Keys.Space, TriggerState.ShowAndConsume));
			triggers.Add(new ShortCutTrigger(Keys.Escape, TriggerState.HideAndConsume));
		}

		private void OnListLostFocus(object sender, EventArgs e)
		{
			OnLostFocus(e);
		}

		private void OnMouseHover(object sender, EventArgs e)
		{
			if (_itemDisplayAdaptor == null || SelectedItem == null)
			{
				return;
			}

			string tip = _itemDisplayAdaptor.GetToolTip(SelectedItem);
			_toolTip.SetToolTip(this, tip);
			_toolTip.ToolTipTitle = _itemDisplayAdaptor.GetToolTipTitle(SelectedItem);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			using (var detect = Detect.Reentry(this, "OnSizeChanged"))
			{
				if (detect.DidReenter)
				{
					return;
				}

				var height = this.Height;
				base.OnSizeChanged(e);
				if (height > this.Height)   //this is for the search box, where the ws label could be much taller that the list text
					this.Height = height;

				if (_listBox != null)
				{
					//NB: this height can be multiple lines, so we don't just want the Height
					//this._listBox.ItemHeight = Height;
					_listBox.ItemHeight = _listBox.Font.Height;
				}
				if (_listBox != null && _autoSizePopup)
				{
					if (_listBox.Width < Width)
					{
						_listBox.Width = Width;
					}
				}
			}
		}

		public override bool Focused
		{
			get { return (base.Focused || InFocus || _listBox.InFocus); }
		}

		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (_popupParent != null)
			{
				_popupParent.Move -= Parent_Move;
				_popupParent.ParentChanged -= popupParent_ParentChanged;
			}
			_popupParent = Parent;
			if (_popupParent != null)
			{
				while (_popupParent.Parent != null)
				{
					_popupParent = _popupParent.Parent;
				}
				_popupParent.Move += Parent_Move;
				_popupParent.ParentChanged += popupParent_ParentChanged;
			}
		}

		private void popupParent_ParentChanged(object sender, EventArgs e)
		{
			OnParentChanged(e);
		}

		private void Parent_Move(object sender, EventArgs e)
		{
			HideList();
		}

		protected virtual bool DefaultCmdKey(ref Message msg, Keys keyData)
		{
			bool val = base.ProcessCmdKey(ref msg, keyData);

			if (TriggersEnabled)
			{
				switch (Triggers.OnCommandKey(keyData))
				{
					case TriggerState.ShowAndConsume:
						{
							val = true;
							ShowList();
						}
						break;
					case TriggerState.Show:
						{
							ShowList();
						}
						break;
					case TriggerState.HideAndConsume:
						{
							val = true;
							HideList();
						}
						break;
					case TriggerState.Hide:
						{
							HideList();
						}
						break;
					case TriggerState.SelectAndConsume:
						{
							if (_listBox.Visible)
							{
								val = true;
								SelectCurrentItemAndHideList();
							}
						}
						break;
					case TriggerState.Select:
						{
							if (_listBox.Visible)
							{
								SelectCurrentItemAndHideList();
							}
						}
						break;
					default:
						break;
				}
			}
#if __MonoCS__
			if (keyData == Keys.Enter)
			{
				OnKeyDown(new KeyEventArgs(keyData));
			}
#endif
			return val;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			uint keyValue = (uint)keyData;
			switch (keyData)
			{
				case Keys.Up:
					{
						TriggersEnabled = false;
						Mode = EntryMode.List;
						TriggersEnabled = true;
						if (_listBox.Visible == false)
						{
							ShowList();
						}
						if (_listBox.SelectedIndex > 0)
						{
							_listBox.SelectedIndex--;
						}
						return true;
					}
				case Keys.Down:
					{
						TriggersEnabled = false;
						Mode = EntryMode.List;
						TriggersEnabled = true;
						if (_listBox.Visible == false)
						{
							ShowList();
						}
						if (_listBox.SelectedIndex < _listBox.Items.Count - 1)
						{
							_listBox.SelectedIndex++;
						}
						return true;
					}
				default:
					{
						return DefaultCmdKey(ref msg, keyData);
					}
			}
		}

		protected override void OnTextChanged(object sender, EventArgs e)
		{
			base.OnTextChanged(sender, e);
			_listBox.SelectedIndex = -1;
			DisplayListIfTextTriggers();
		}

		private void DisplayListIfTextTriggers()
		{
			if (TriggersEnabled)
			{
				switch (Triggers.OnTextChanged(Text))
				{
					case TriggerState.Show:
						{
							ShowList();
						}
						break;
					case TriggerState.Hide:
						{
							HideList();
						}
						break;
					default:
						{
							UpdateList();
						}
						break;
				}
			}

			SelectedItem = _formToObjectFinderDelegate(Text);
		}

		/// <summary>
		/// can be replaced by something smarter, using the FormToObjectFinderDelegate
		/// </summary>
		/// <param name="form"></param>
		private object DefaultFormToObjectFinder(string form)
		{
			foreach (object item in _items)
			{
				if (_itemDisplayAdaptor.GetDisplayLabel(item) == form)
				{
					return item;
				}
			}
			return null;
		}

		protected override void OnDomFocus(object sender, DomEventArgs e)
		{
			if (!InFocus)
			{
				base.OnDomFocus(sender, e);
				OnGotFocus(null);
			}
		}

		protected override void OnDomBlur(object sender, DomEventArgs e)
		{
			base.OnDomBlur(sender, e);
			OnLostFocus(null);
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus(e);
			if (!(Focused || ListBoxFocused))
			{
				HideList();
			}
		}

		protected override void OnGotFocus(EventArgs e)
		{
			base.OnGotFocus(e);

			_windowJustGotFocus = true;

			if (Text.Length > 0)
			{
				SelectionStart = 0;
				SelectionLength = Text.Length;
			}
		}

		protected override void OnClick(EventArgs e)
		{
			base.OnClick(e);
			if (_windowJustGotFocus)
			{
				_windowJustGotFocus = false;
				SelectionStart = 0;
				SelectionLength = Text.Length;
			}
		}

		protected virtual void SelectCurrentItem()
		{
			if (_listBox.SelectedIndex == -1)
			{
				return;
			}

			_keyPressed = false;
			SelectedItem = ((ItemWrapper)_listBox.SelectedItem).Item;
			Text = ItemDisplayStringAdaptor.GetDisplayLabel(SelectedItem);// SelectedItem.ToString();
			if (Text.Length > 0)
			{
				SelectionStart = Text.Length;
			}
		}

		protected void SelectCurrentItemAndHideList()
		{
			if (_listBox.SelectedIndex != -1)
			{
				SelectCurrentItem();
			}
			HideList();
		}

		protected virtual void ShowList()
		{
			if (_listBox.Visible == false)
			{
				UpdateList();
				Form form = FindForm();
				if (form != null)
				{
					Point parentPointOnScreen = Parent.PointToClient(form.Location);
					Point formPointOnScreen = form.PointToClient(form.Location);
					Point offset = new Point(formPointOnScreen.X - parentPointOnScreen.X,
						formPointOnScreen.Y - parentPointOnScreen.Y);
					Point p = Location;
					p.X += offset.X;
					p.Y += offset.Y;
					p.X += PopupOffset.X;
					p.Y += Height + PopupOffset.Y;
					_listBox.Location = p;
					if (_listBox.Items.Count > 0)
					{
						if (!form.Controls.Contains(_listBox))
						{
							form.Controls.Add(_listBox);
						}

						_listBox.BringToFront();
						_listBox.Visible = true;
						_listBox.Location = p;
					}
				}
			}
			else
			{
				UpdateList();
			}
		}

		public virtual void HideList()
		{
			Mode = EntryMode.Text;
			_listBox.Visible = false;
		}

		private class ItemWrapper
		{
			private object _item;
			private readonly string _label;

			public ItemWrapper(object item, string label)
			{
				Item = item;
				_label = label;
			}

			public object Item
			{
				get { return _item; }
				set { _item = value; }
			}

			public override string ToString()
			{
				return _label;
			}
		}

		protected virtual void UpdateList()
		{
			int selectedIndex = _listBox.SelectedIndex;
			_listBox.Clear();
			_listBox.Font = Font;
			_listBox.ItemHeight = _listBox.Font.Height;

			int maxWidth = Width;
			using (Graphics g = (_autoSizePopup) ? CreateGraphics() : null)
			{
				// Text.Trim() allows "1 " to trigger updating the list and matching "1 Universe", where
				// the match involves "1" and "Universe" separately.  (Note that "1 " does not match either
				// "1" or "Universe".)
				foreach (object item in ItemFilterer.Invoke(Text.Trim(), Items, ItemDisplayStringAdaptor))
				{
					string label = ItemDisplayStringAdaptor.GetDisplayLabel(item);
					_listBox.AddItem(new ItemWrapper(item, label));
					if (_autoSizePopup)
					{
						maxWidth = Math.Max(maxWidth, MeasureItem(g, label).Width);
					}
				}
			}
			_listBox.ListCompleted();
			_listBox.SelectedIndex = selectedIndex;

			if (_listBox.Items.Count == 0)
			{
				HideList();
			}
			else
			{
				int visItems = _listBox.Items.Count;
				if (visItems > 8)
				{
					visItems = 8;
				}

				_listBox.Height = (visItems * _listBox.ItemHeight) + 4;

				switch (BorderStyle)
				{
					case BorderStyle.FixedSingle:
						{
							_listBox.Height += 2;
							break;
						}
					case BorderStyle.Fixed3D:
						{
							_listBox.Height += 4;
							break;
						}
				}

				if (_autoSizePopup)
				{
					bool hasScrollBar = visItems < _listBox.Items.Count;
					_listBox.Width = maxWidth + (hasScrollBar ? 10 : 0);
					// in theory this shouldn't be needed
				}
				_listBox.RightToLeft = RightToLeft;
			}
		}

		private Size MeasureItem(IDeviceContext dc, string s)
		{
			TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.NoClipping;
			if (WritingSystem != null && WritingSystem.RightToLeftScript)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			return TextRenderer.MeasureText(dc,
											s,
											_listBox.Font,
											new Size(int.MaxValue, _listBox.ItemHeight),
											flags);
		}

		private static IEnumerable FilterList(string text,
											  IEnumerable items,
											  IDisplayStringAdaptor adaptor)
		{
			ICollection<object> newList = new Collection<object>();

			foreach (object item in items)
			{
				string label = adaptor.GetDisplayLabel(item);
				if (label.ToLower().StartsWith(text.ToLower()))
				{
					newList.Add(item);
					break;
				}
			}
			return newList;
		}

		public event EventHandler AutoCompleteChoiceSelected = delegate { };

		private void List_Click(object sender, EventArgs e)
		{
			SelectCurrentItemAndHideList();
			AutoCompleteChoiceSelected.Invoke(this, new EventArgs());
		}

		private void Popup_Deactivate(object sender, EventArgs e)
		{
			if (!(Focused || ListBoxFocused))
			{
				HideList();
			}
		}

		public bool ListBoxFocused
		{
			// The order of WM_SETFOCUS and WM_KILLFOCUS is different between Windows and Mono.
			// So this.Focused is set false before _listBox.Focused is set true in Mono.  But
			// WM_ENTER is sent before WM_KILLFOCUS in Mono, so our internal flag may be true
			// even when neither Focused flag is true.  This internal flag isn't needed for
			// Windows, but it also doesn't hurt.
			get
			{
				return _listBox.Focused || _listBoxEntered;
			}
		}

		private bool _listBoxEntered;
		/// <summary>
		/// Record when the child ListBox has been entered, but not yet left.  In Mono, this
		/// starts before _listBox obtains focus, and also before "this" loses focus.  (In
		/// Windows, this isn't needed but it doesn't hurt either.)
		/// </summary>
		private void List_Enter(object sender, System.EventArgs e)
		{
			_listBoxEntered = true;
		}

		private void List_Leave(object sender, System.EventArgs e)
		{
			_listBoxEntered = false;
		}

		public override WritingSystemDefinition WritingSystem
		{
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_writingSystem = value;
				_listBox.FormWritingSystem = value;
				Font = WritingSystemInfo.CreateFont(_writingSystem);
			}
		}
		protected override void OnBackColorChanged(object sender, EventArgs e)
		{
			RefreshDisplay();
		}
		protected override void OnForeColorChanged(object sender, EventArgs e)
		{
			RefreshDisplay();
		}
	}
}
