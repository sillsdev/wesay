using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Language;
using System.Collections.Generic;

namespace WeSay.UI
{
	public partial class WeSayListBox: ListView
	{
		private WritingSystem _writingSystem;
		private int _itemToNotDrawYet = -1;
		private IList _dataSource;
		private readonly Dictionary<int, ListViewItem> _itemsCache;

		public WeSayListBox()
		{
			InitializeComponent();
			AdjustColumnWidth();
			SimulateListBox = true;
			_itemsCache = new Dictionary<int, ListViewItem>();
			_SelectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
		}

		protected override void OnCacheVirtualItems(CacheVirtualItemsEventArgs e)
		{
			_itemsCache.Clear();
			base.OnCacheVirtualItems(e);
			for (int i = e.StartIndex; i <= e.EndIndex; ++i)
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
		private ListViewItem GetVirtualItem(int index) {
			RetrieveVirtualItemEventArgs e = new RetrieveVirtualItemEventArgs(index);
			base.OnRetrieveVirtualItem(e);
			if (e.Item == null)
			{
				e.Item = new ListViewItem(this._dataSource[e.ItemIndex].ToString());
			}

			if (e.ItemIndex == SelectedIndex)
			{
				e.Item.Selected = true;
			}

			return e.Item;
		}

		[Browsable(false)]
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
		public int ItemToNotDrawYet
		{
			get { return _itemToNotDrawYet; }
			set { _itemToNotDrawYet = value; }
		}

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
				Refresh();
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
			if (System.Environment.OSVersion.Version.Major >= 6)
			{
				this.Invalidate();//needed to prevent artifacts of previous selections hanging around
			}

			base.OnItemSelectionChanged(e);
				_selectedItem = SelectedItem;
				OnSelectedIndexChanged(new EventArgs());
			}

		#region extend hot click area to simulate list box behavior
		// see comment on OnMouseUp
		private bool _clickSelecting;
		private Point _currentMouseLocation;

		protected override void OnClick(EventArgs e)
		{
			_clickSelecting = false;
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
			if (SimulateListBox && _clickSelecting)
			{
				ListViewItem item = GetItemAt(0, _currentMouseLocation.Y);
				if (item != null)
				{
					SelectedIndex = item.Index;
				}
				else
				{
					// restore the selection
					int index = _dataSource.IndexOf(_selectedItem);
					if (index != -1)
					{
						SelectedIndex = index;
					}
				}
			}
			_clickSelecting = false;
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
				Rectangle bounds =
						new Rectangle(e.Bounds.X, e.Bounds.Y, header.Width, e.Bounds.Height);

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
		public bool SimulateListBox {
			get
			{
				return _simulateListBoxBehavior && Columns.Contains(this.header) && View == System.Windows.Forms.View.SmallIcon;
			}
			set
			{
				_simulateListBoxBehavior = value;
				if(value)
				{
					if(!Columns.Contains(header))
					{
						Columns.Insert(0, header);
					}
					View = System.Windows.Forms.View.SmallIcon;
				}
			}
		}

		protected override void OnResize(EventArgs e)
		{
			AdjustColumnWidth();
		}

		private void AdjustColumnWidth()
		{
			header.Width = Width - 20; // to account for scrollbar
		}

		private int _SelectedIndexForUseBeforeSelectedIndicesAreInitialized;
		public int SelectedIndex
		{
			get
			{
				if (SelectedIndices.Count > 0)
				{
					return SelectedIndices[0];
				}
				return _SelectedIndexForUseBeforeSelectedIndicesAreInitialized;
			}
			set
			{
				if(DataSource == null)
				{
					throw new InvalidOperationException("DataSource must be initialized before SelectedIndex can be set.");
				}
				if (value < -1 || value >= DataSource.Count)
				{
					throw new ArgumentOutOfRangeException();
				}
				if (value == -1)
				{
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
						_SelectedIndexForUseBeforeSelectedIndicesAreInitialized = value;
						OnSelectedIndexChanged(new EventArgs());
					}
					else
					{
						// done with it's usefulness
						_SelectedIndexForUseBeforeSelectedIndicesAreInitialized = -1;
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
			TextFormatFlags flags = TextFormatFlags.Default | TextFormatFlags.Left | TextFormatFlags.VerticalCenter;
			if (_writingSystem != null && WritingSystem.RightToLeft)
			{
				flags |= TextFormatFlags.RightToLeft;
			}
			TextRenderer.DrawText(e.Graphics, e.ToolTipText, _writingSystem.Font, e.Bounds, tooltip.ForeColor, flags);
		}

		private void ToolTipPopup(object sender, PopupEventArgs e)
		{
				e.ToolTipSize =
					   Size.Add(MeasureItemText(this.tooltip.GetToolTip(this)),
												 new Size(0,5));
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
												this._writingSystem.Font,
												new Size(maxWidth, int.MaxValue),
												flags);
			}
		}
	}
}
