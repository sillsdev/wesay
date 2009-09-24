using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;

namespace WeSay.UI
{
	public partial class WeSayListView: ListView
	{
		private WritingSystem _writingSystem;
		private int _itemToNotDrawYet = -1;
		private IList _dataSource;
		private readonly Dictionary<int, ListViewItem> _itemsCache;

		public WeSayListView()
		{
			InitializeComponent();
			AdjustColumnWidth();
			SimulateListBox = true;
			_itemsCache = new Dictionary<int, ListViewItem>();
			_selectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
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
				e.Item = new ListViewItem(_dataSource[e.ItemIndex].ToString());
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
		public WritingSystem WritingSystem
		{
			get
			{
				if (_writingSystem == null)
				{
					throw new InvalidOperationException(
							"WritingSystem must be initialized prior to use.");
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
				RemoveBindingListNotifiers();
				_dataSource = value;
				if (value == null)
				{
					VirtualListSize = 0;
				}
				else
				{
					VirtualListSize = value.Count;
					if (value.Count > 0)
					{
						SelectedIndex = 0;
					}
				}
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
			int originalSelectedIndex = SelectedIndex;

			// this needs to be at the beginning so we don't make an invalid
			// reference past the end when an item is deleted
			// n.b. To cooperate with the virtual mode, when a Selection
			// is made (i.e., SelectedIndex is assigned) RetrieveVirtualItem
			// will get called to update the old selection and the new
			// unless we lower the size.
			VirtualListSize = _dataSource.Count;

			//restore our selection
			int index = _dataSource.IndexOf(_selectedItem);
			if (index != -1)
			{
				// the selected item didn't change but it's index did
				SelectedIndex = index;
			}
			else if (originalSelectedIndex >= VirtualListSize)
			{
				SelectedIndex = VirtualListSize - 1;
			}
			else
			{
				// the previously selected item was deleted
				// the index didn't change but the item did
				_selectedItem = SelectedItem;

				// even though technically the index didn't change, the effect is that a new item is selected
				OnSelectedIndexChanged(new EventArgs());
			}
		}

		private object _selectedItem;

		protected override void OnItemSelectionChanged(ListViewItemSelectionChangedEventArgs e)
		{
			if (Environment.OSVersion.Version.Major >= 6 || Environment.OSVersion.Platform == PlatformID.Unix)
			{
				Invalidate(); //needed to prevent artifacts of previous selections hanging around
			}

			base.OnItemSelectionChanged(e);
			_selectedItem = SelectedItem;
			OnSelectedIndexChanged(new EventArgs());

			//jh sept 2009 to help with WS-14934 (cambell) Dictionary word list scrolls unnecessarily when editing headword
			//it'd be better to not scroll, but this occurs to me as a quick way to at least keep it from scrolling to the bottom
			if (!_clickSelecting && e.Item != null && e.ItemIndex > 0 && Items.Count>0)
			{
				const int numberToShowBelowSelectedOne = 10;
				//though we'd like to not scroll at all, this will
				//make our selected one be at least 10 up from the bottom, which isn't so bad.
				int lastOneToShow = Math.Min(Items.Count - 1, e.ItemIndex + numberToShowBelowSelectedOne);
				Items[lastOneToShow].EnsureVisible();
				//enhance... figure out where the middle would be, and the arrange for the selected item to be in the middle
				//this.Height / e.Item.Font.Height
			}
		}

		#region extend hot click area to simulate list box behavior

		// see comment on OnMouseUp
		private bool _clickSelecting;
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
		}

		private void SelectFromClickLocation()
		{
			if (SimulateListBox && _clickSelecting)
			{
				ListViewItem item = GetItemAt(0, _currentMouseLocation.Y);
				if (item != null)
				{
					SelectedIndex = item.Index;
					item.Focused = true;
				}
				else
				{
					// restore the selection
					int index = _dataSource.IndexOf(_selectedItem);
					if (index != -1)
					{
						SelectedIndex = index;
						if (VirtualMode)
						{
							GetVirtualItem(index).Focused = true;
						}
						else
						{
							Items[index].Focused = true;
						}
					}
				}
			}
			_clickSelecting = false;
		}

		protected override void OnClick(EventArgs e)
		{
			SelectFromClickLocation();
			base.OnClick(e);
		}

		protected override void OnMouseDown(MouseEventArgs e)
		{
			_clickSelecting = true;
			_currentMouseLocation = e.Location;
			base.OnMouseDown(e);
		}

		protected override void OnMouseMove(MouseEventArgs e)
		{
			// As of 03-oct-2008 Mono calls OnMouseMove in between OnMouseDown
			// and OnMouseUp even if the mouse did not move.
			if (Type.GetType("Mono.Runtime") != null && _currentMouseLocation == e.Location)
			{
				return;
			}
			_clickSelecting = false;
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
				if (MeasureItemText(e.Item.Text).Width > Width)
				{
					tooltip.Show(e.Item.Text, this, e.Item.Position, int.MaxValue);
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
			SelectFromClickLocation();
			_currentMouseLocation = e.Location;
			base.OnMouseUp(e);
		}

		#endregion

		protected override void OnDrawItem(DrawListViewItemEventArgs e)
		{
			if (e.ItemIndex == _itemToNotDrawYet)
			{
				return;
			}
			// As long as we have our special setup to make us look like a list box,
			// then render us for that special setup otherwise just use the default.
			if (SimulateListBox)
			{
				// All this is to make the selection across the whole list box
				// and not just the extent of the text itself
				Rectangle bounds = new Rectangle(e.Bounds.X,
												 e.Bounds.Y,
												 ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth,
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
				if (_writingSystem != null && WritingSystem.RightToLeft)
				{
					flags |= TextFormatFlags.RightToLeft;
				}
				TextRenderer.DrawText(e.Graphics, e.Item.Text, e.Item.Font, bounds, textColor, flags);

				if (backgroundBrushNeedsDisposal)
				{
					backgroundBrush.Dispose();
				}
			}
			else
			{
				e.DrawDefault = true;
			}

			base.OnDrawItem(e);
		}

		private bool _simulateListBoxBehavior;

		[Browsable(true)]
		[DefaultValue(true)]
		public bool SimulateListBox
		{
			get
			{
				return _simulateListBoxBehavior && Columns.Contains(header) &&
					(View == View.SmallIcon);
			}
			set
			{
				_simulateListBoxBehavior = value;
				if (value)
				{
					if (!Columns.Contains(header))
					{
						Columns.Insert(0, header);
					}
					View = View.SmallIcon;
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			AdjustColumnWidth();
			base.OnResize(e);
		}

		private void AdjustColumnWidth()
		{
			int newWidth = ClientRectangle.Width - SystemInformation.VerticalScrollBarWidth;
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
				if (value == -1)
				{
					_selectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
					SelectedIndices.Clear();
					_selectedItem = null;
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
						_selectedIndexForUseBeforeSelectedIndicesAreInitialized = value;
						OnSelectedIndexChanged(new EventArgs());
					}
					else
					{
						// done with its usefulness
						_selectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
					}

					_selectedItem = SelectedItem;
					EnsureVisible(value);
				}
			}
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
			if (_writingSystem != null && WritingSystem.RightToLeft)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			TextRenderer.DrawText(e.Graphics,
								  e.ToolTipText,
								  _writingSystem.Font,
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
			if (_writingSystem != null && WritingSystem.RightToLeft)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			int maxWidth = Screen.GetWorkingArea(this).Width;
			using (Graphics g = Graphics.FromHwnd(Handle))
			{
				return TextRenderer.MeasureText(g,
												text,
												_writingSystem.Font,
												new Size(maxWidth, int.MaxValue),
												flags);
			}
		}
	}
}