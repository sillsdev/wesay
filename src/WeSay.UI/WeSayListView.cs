using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Palaso.DictionaryServices.Model;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI
{
	public interface IWeSayListView
	{
		[Browsable(false)]
		string Text { set; get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		IWritingSystemDefinition WritingSystem { get; set; }

		int MinLength { get; set; }
		int MaxLength { get; set; }
		int SelectedIndex { get; set; }
		Rectangle Bounds { get; set; }
		IList DataSource { get; set; }
		int Length { get; }
		DockStyle Dock { get; set; }
		Point Location { get; set; }
		string Name { get; set; }
		Size Size { get; set; }
		int TabIndex { get; set; }
		View View { get; set; }
		BorderStyle BorderStyle { get; set; }
		Object SelectedItem { get; }
		AnchorStyles Anchor { get; set; }
		int VirtualListSize { get; set; }
		Color BackColor { get; set; }
		int ListWidth { get; }

		void SetBounds(int x, int y, int width, int height);
		bool Focus();
		Size MinimumSize { get; set; }
		bool Bold { get; set; }

		event ListViewItemSelectionChangedEventHandler ItemSelectionChanged;
		event RetrieveVirtualItemEventHandler RetrieveVirtualItem;
	}

	public partial class WeSayListView: ListView, IWeSayListView
	{
		private const int WM_HSCROLL = 0x114;
		private IWritingSystemDefinition _writingSystem;
		private int _itemToNotDrawYet = -1;
		private IList _dataSource;
		private readonly Dictionary<int, ListViewItem> _itemsCache;

		private bool _ensureVisibleCalledBeforeWindowHandleCreated = false;
		public new event EventHandler<ListViewItemSelectionChangedEventArgs> ItemSelectionChanged;
		public new event EventHandler<RetrieveVirtualItemEventArgs> RetrieveVirtualItem;

		public WeSayListView()
		{
			InitializeComponent();
			_itemsCache = new Dictionary<int, ListViewItem>();
			_selectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
			SimulateListBox();
		}

		private void SimulateListBox()
		{
			if (!Columns.Contains(header))
			{
				Columns.Insert(0, header);
			}
			View = View.SmallIcon;
			AdjustColumnWidth();
		}

		[DefaultValue(false)]
		[Browsable(true)]
		public new bool AutoArrange
		{
			get { return base.AutoArrange; }
			set { base.AutoArrange = value; }
		}

		[DefaultValue(false)]
		[Browsable(true)]
		public new bool MultiSelect
		{
			get { return base.MultiSelect; }
			set { base.MultiSelect = value; }
		}

		[DefaultValue(false)]
		[Browsable(true)]
		public new bool HideSelection
		{
			get { return base.HideSelection; }
			set { base.HideSelection = value; }
		}

		[DefaultValue(false)]
		[Browsable(true)]
		public new bool LabelWrap
		{
			get { return base.LabelWrap; }
			set { base.LabelWrap = value; }
		}

		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool VirtualMode
		{
			get { return base.VirtualMode; }
			private set { base.VirtualMode = value; }
		}

		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool OwnerDraw
		{
			get { return base.OwnerDraw; }
			private set { base.OwnerDraw = value; }
		}

		[DefaultValue(true)]
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public new bool UseCompatibleStateImageBehavior
		{
			get { return base.UseCompatibleStateImageBehavior; }
			private set { base.UseCompatibleStateImageBehavior = value; }
		}

		protected override void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
		{
			_itemsCache.Clear();
			base.OnCacheVirtualItems(e);
			for (int i = e.StartIndex;i <= e.EndIndex;++i)
			{
				_itemsCache[i] = GetVirtualItem(i);
			}
		}

		protected override void OnRetrieveVirtualItem(RetrieveVirtualItemEventArgs e)
		{
			ListViewItem result;
			// try to get a cached result first.
			if (!_itemsCache.TryGetValue(e.ItemIndex, out result))
			{
				result = GetVirtualItem(e.ItemIndex);
			}
			e.Item = result;
		}

		// ask for a real virtual item using RetrieveVirtualItem event
		private ListViewItem GetVirtualItem(int index)
		{
			RetrieveVirtualItemEventArgs e = new RetrieveVirtualItemEventArgs(index);
			base.OnRetrieveVirtualItem(e);
			if (e.Item == null)
			{
				var entry = _dataSource[e.ItemIndex] as LexEntry;
				if(entry!=null)
				{
					var form = entry.GetHeadWordForm(_writingSystem.Id);
					if(string.IsNullOrEmpty(form))
					{
						//this is only going to come up with something in two very unusual cases:
						//1) a monolingual dictionary (well, one with meanings in the same WS as the lexical units)
						//2) the SIL CAWL list, where the translator adds glosses, and fails to add
						//lexical entries.
						form = entry.GetSomeMeaningToUseInAbsenseOfHeadWord(_writingSystem.Id);
					}
					e.Item = new ListViewItem(form);
				}
				else
				{
					e.Item = new ListViewItem( _dataSource[e.ItemIndex].ToString());
				}
			}

			if (e.ItemIndex == SelectedIndex)
			{
				e.Item.Selected = true;
			}

			return e.Item;
		}

		[Browsable(false)]
		[DefaultValue(null)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public IWritingSystemDefinition WritingSystem
		{
			get
			{
				if (_writingSystem == null)
				{
					if(DesignMode)
						return new WritingSystemDefinition();
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

		//used when animating additions to the list
		[Browsable(false)]
		[DefaultValue(-1)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public int ItemToNotDrawYet
		{
			get { return _itemToNotDrawYet; }
			set { _itemToNotDrawYet = value; }
		}

		[DefaultValue(null)]
		public IList DataSource
		{
			get { return _dataSource; }
			set
			{
				_itemsCache.Clear();
				_dataSource = value;
				if (value == null)
				{
					VirtualListSize = 0;
				}
				else
				{
					VirtualListSize = value.Count;
				}
				Invalidate();

				if (value is IBindingList)
				{
					((IBindingList) value).ListChanged += OnListChanged;
				}
			 }
		 }

		 private void RemoveBindingListNotifiers()
		 {
			 if (_dataSource != null && _dataSource is IBindingList)
			 {
				 ((IBindingList) _dataSource).ListChanged -= OnListChanged;
			 }
		 }

		 private void OnListChanged(object sender, ListChangedEventArgs e)
		 {
			_itemsCache.Clear();
			// this needs to be at the beginning so we don't make an invalid
			// reference past the end when an item is deleted
			// n.b. To cooperate with the virtual mode, when a Selection
			// is made (i.e., SelectedIndex is assigned) RetrieveVirtualItem
			// will get called to update the old selection and the new
			// unless we lower the size.
			VirtualListSize = _dataSource.Count;
		 }

		protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
		{
			if (_mouseDownInfo.MouseIsDown) return;

			base.OnItemSelectionChanged(e);
		}

		#region extend hot click area to simulate list box behavior

		private Point _currentMouseLocation;

		protected override void WndProc(ref Message m)
		{
			// ignore double-clicks - WM_LBUTTONDBLCLK == 0x0203, WM_RBUTTONDBLCLK = 0x0206
			// Can't just override OnDoubleClick because that is not called if the user
			// clicks on blank space within the ListView control.
			if (m.Msg == 0x0203 || m.Msg == 0x0206)
			{
				m.Result = new IntPtr(0);
				return;
			}
			base.WndProc(ref m);
#if __MonoCS__
			if (m.Msg == WM_HSCROLL)
			{
				this.OnScroll();
			}
#endif
		}
#if __MonoCS__
		protected void OnScroll()
		{
			Invalidate();
		}
#endif

		private void SelectFromClickLocation()
		{
			ListViewItem item = GetItemAt(0, _currentMouseLocation.Y);
			if (item != null)
			{
				_mouseDownInfo.IndexSelected = item.Index;
				item.Focused = true;
			}
			else
			{
				_mouseDownInfo.IndexSelected = -1;
			}
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_mouseDownInfo.OldIndex = SelectedIndex;
			InvalidateItemRect(SelectedIndex);
			_currentMouseLocation = e.Location;
			SelectFromClickLocation();
			base.OnMouseDown(e);
			InvalidateItemRect(_mouseDownInfo.IndexSelected);
			_mouseDownInfo.MouseIsDown = true;
		}

		private readonly MouseDownInfo _mouseDownInfo = new MouseDownInfo();

		private class MouseDownInfo
		{
			//We use this to suppress ItemSelectionChanged events
			//this is necassary because clicking on the item triggers and ItemSelectionChangedEvent
			//while clicking on the white space next to an item does not.
			//We try to trigger these events exclusively via the SelectedIndex property
			public bool MouseIsDown;
			//We need to set this on mouse down as that is when the listview usually makes it's selection.
			//But we don't actually make the change until MouseUp.
			public int IndexSelected;
			//We need to log what the old index was so that we get proper ItemSelectionChanged events on MouseUp
			public int OldIndex;
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			// As of 03-oct-2008 Mono calls OnMouseMove in between OnMouseDown
			// and OnMouseUp even if the mouse did not move.
			if (Type.GetType("Mono.Runtime") != null && _currentMouseLocation == e.Location)
			{
				return;
			}
			_currentMouseLocation = e.Location;
			if (GetItemAt(e.X, e.Y) == null)
			{
				tooltip.Hide(this);
			}
			base.OnMouseMove(e);
		}

		// we set the tool tip explicitly due to a bug with
		// MS implementation of ListView when Virtual
		// and ShowItemTooltips

		// show the tooltip if there is one
		// otherwise see if the text has been truncated
		// and if it has show the text as a tooltip
		protected override void OnItemMouseHover(ListViewItemMouseHoverEventArgs e)
		{
			if (string.IsNullOrEmpty(e.Item.ToolTipText))
			{
				string textMinusAccelerators = e.Item.Text.Replace("&", "&&");
				int textWidth = MeasureItemText(textMinusAccelerators).Width;
				//This is identical to the width used in OnDrawItem
				int rectanglesize = ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
				if (textWidth > rectanglesize)
				{
					tooltip.Show(textMinusAccelerators, this, _currentMouseLocation, int.MaxValue);
				}
			}
			else
			{
				tooltip.Show(e.Item.ToolTipText, this, _currentMouseLocation, int.MaxValue);
			}

			base.OnItemMouseHover(e);
		}

		// By default this control will turn off the selection when
		// clicking off of any text, this will allow that to be
		// kept selected
		// we have to be careful here though since the control may have already
		// handled this and we can have cases where because of mouse movement
		// the coordinates returned now don't reflect the user's intentions
		protected override void OnMouseUp(MouseEventArgs e)
		{
			SelectedIndex = _mouseDownInfo.OldIndex;
			_mouseDownInfo.MouseIsDown = false;
			SelectedIndex = _mouseDownInfo.IndexSelected;
			//Console.WriteLine("Up! Selected index: {0}", SelectedIndex);
			base.OnMouseUp(e);
		}

		private void InvalidateItemRect(int dirtyIndex)
		{
			if (dirtyIndex != -1)
			{
				// Needlessly accessing the Handle here causes the Handle to be created if it
				// is currently null, which appeared to happen after Deactivate/Activate sequence
				System.IntPtr myHandle = Handle;

				var itemRect = GetItemRect(dirtyIndex);
				var rectSpanningWholeControl = new Rectangle(itemRect.X, itemRect.Y, ClientRectangle.Width,
															 itemRect.Height);
				Invalidate(rectSpanningWholeControl);
			}
		}
		#endregion

		protected override void OnDrawItem(DrawListViewItemEventArgs e)
		{
			// All this is to make the selection across the whole list box
			// and not just the extent of the text itself
			Rectangle bounds = new Rectangle(e.Bounds.X,
												e.Bounds.Y,
												ClientRectangle.Width,
												e.Bounds.Height);

			Brush backgroundBrush;
			bool backgroundBrushNeedsDisposal = false;
			Color textColor;
			if (SelectedIndex == e.ItemIndex && (!HideSelection || Focused))
			{
				backgroundBrush = SystemBrushes.Highlight;
				textColor = SystemColors.HighlightText;
			}
			else
			{
				backgroundBrush = new SolidBrush(e.Item.BackColor);
				backgroundBrushNeedsDisposal = true;
				textColor = e.Item.ForeColor;
			}

			e.Graphics.FillRectangle(backgroundBrush, bounds);
			TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.Left |
									TextFormatFlags.EndEllipsis;
			if (_writingSystem != null && WritingSystem.RightToLeftScript)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			if (e.Item.Text.Equals("(No Gloss)") || e.Item.Text.Equals("(Empty)"))
			{
				TextRenderer.DrawText(e.Graphics, e.Item.Text, SystemFonts.DefaultFont, bounds, textColor, flags);
			}
			else
			{
				string textMinusAccelerators = e.Item.Text.Replace("&","&&");
				TextRenderer.DrawText(e.Graphics, textMinusAccelerators, Font, bounds, textColor, flags);
			}

			if (backgroundBrushNeedsDisposal)
			{
				backgroundBrush.Dispose();
			}

			base.OnDrawItem(e);
		}

		protected override void OnResize(EventArgs e)
		{
			base.OnResize(e);
			AdjustColumnWidth();
		}

		private void AdjustColumnWidth()
		{
			int newWidth = ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
			// Column width seems to have some maximum, after which it allows multiple columns.
			// So we constrain it to a 'reasonable' but large enough value.
			newWidth = Math.Max(newWidth, 300);
			SuspendLayout();
			if (Columns.Count > 0)
			{
				Columns[0].Width = newWidth;
			}
			header.Width = newWidth;
			ResumeLayout();
		}

		private int _selectedIndexForUseBeforeSelectedIndicesAreInitialized;

		[DefaultValue(-1)]
		[Browsable(true)]
		public int SelectedIndex
		{
			get
			{
				if (SelectedIndices.Count > 0)
				{
					return SelectedIndices[0];
				}
				return _selectedIndexForUseBeforeSelectedIndicesAreInitialized;
			}
			set
			{
				if (DataSource == null)
				{
					throw new InvalidOperationException(
							"DataSource must be initialized before SelectedIndex can be set.");
				}
				if (value < -1 || value >= DataSource.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (SelectedIndex != value)
				{
					InvalidateItemRect(SelectedIndex);
					InvalidateItemRect(value);
					if (value == -1)
					{
						_selectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
						SelectedIndices.Clear();
						OnItemSelectionChanged(new ListViewItemSelectionChangedEventArgs(null,value,true));
					}
					else
					{
						if (!SelectedIndices.Contains(value))
						{
							SelectedIndices.Add(value);
						}

						// We can't get a selection to stay until the real handle is created
						// this gets around that
						if (SelectedIndices.Count == 0)
						{
							if (_selectedIndexForUseBeforeSelectedIndicesAreInitialized != SelectedIndex)
							{
								_selectedIndexForUseBeforeSelectedIndicesAreInitialized = SelectedIndex;
								OnItemSelectionChanged(new ListViewItemSelectionChangedEventArgs(GetVirtualItem(SelectedIndex),SelectedIndex, true));
							}

						}
						else
						{
							// done with its usefulness
							_selectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
						}
						if (!IsHandleCreated) //this is a mono bug workaround.
						{
							_ensureVisibleCalledBeforeWindowHandleCreated = true;
						}
						else
						{
							if (SelectedIndex != -1)
							{
								EnsureVisible(SelectedIndex);
							}
						}
					}
				}
			}
		}

		private void SetFocusIndex(int index)
		{
			if (index == -1)
			{
				FocusedItem = null;
			}
			else
			{
				FocusedItem = Items[index];
			}
		}
		public void RemoveItem(int index)
		{
			Items.RemoveAt(index);
		}
		protected override void OnHandleCreated(EventArgs e)
		{
			base.OnHandleCreated(e);
			if (_ensureVisibleCalledBeforeWindowHandleCreated)
			{
				EnsureVisible(SelectedIndex);
				_ensureVisibleCalledBeforeWindowHandleCreated = false;
			}
			SimulateListBox();
		}

		public object SelectedItem
		{
			get
			{
				if (SelectedIndex == -1)
				{
					return null;
				}
				return _dataSource[SelectedIndex];
			}
		}

		private void DrawToolTip(object sender, DrawToolTipEventArgs e)
		{
			e.DrawBackground();
			e.DrawBorder();
			TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.Left |
									TextFormatFlags.VerticalCenter;
			if (_writingSystem != null && WritingSystem.RightToLeftScript)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			TextRenderer.DrawText(e.Graphics,
								  e.ToolTipText,
								  WritingSystemInfo.CreateFont(_writingSystem),
								  e.Bounds,
								  tooltip.ForeColor,
								  flags);
		}

		private void ToolTipPopup(object sender, PopupEventArgs e)
		{
			e.ToolTipSize = Size.Add(MeasureItemText(tooltip.GetToolTip(this)), new Size(0, 5));
		}

		private Size MeasureItemText(string text)
		{
			TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.Left;
			if (_writingSystem != null && WritingSystem.RightToLeftScript)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			int maxWidth = Screen.GetWorkingArea(this).Width;
			using (Graphics g = Graphics.FromHwnd(Handle))
			{
				return TextRenderer.MeasureText(g,
												text,
											   WritingSystemInfo.CreateFont(_writingSystem),
												new Size(maxWidth, int.MaxValue),
												flags);
			}
		}
		public int Length
		{
			get
			{
				return Items.Count;
			}
		}

		public int ListWidth
		{
			get
			{
				return Width;
			}
		 }

		public int MinLength { get; set; }
		public int MaxLength { get; set; }
		public bool Bold { get; set; }
	}
}
