// Derived from code by Peter Femiani available on CodeProject http://www.codeproject.com/csharp/AutoCompleteTextBox.asp

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace WeSay.UI
{
	/// <summary>
	/// Summary description for AutoCompleteTextBox.
	/// </summary>
	[Serializable]
	public class WeSayAutoCompleteTextBox : WeSayTextBox
	{

		#region EntryMode

		public enum EntryMode
		{
			Text,
			List
		}

		#endregion

		#region Members

		private ListBox _list;
		private Form _popup;
		private Control _popupParent;

		#endregion

		#region Properties

		private WeSayAutoCompleteTextBox.EntryMode mode = EntryMode.Text;
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		[Browsable(false)]
		public WeSayAutoCompleteTextBox.EntryMode Mode
		{
			get
			{
				return this.mode;
			}
			set
			{
				this.mode = value;
			}
		}

		private IEnumerable<string> items = new Collection<string>();
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public IEnumerable<string> Items
		{
			get
			{
				return this.items;
			}
			set
			{
				this.items = value;
				if(Mode == EntryMode.List)
				{
					UpdateList();
				}
			}
		}

		private AutoCompleteTriggerCollection triggers = new AutoCompleteTriggerCollection();
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
	[Browsable(false)]
	public AutoCompleteTriggerCollection Triggers
		{
			get
			{
				return this.triggers;
			}
			set
			{
				this.triggers = value;
			}
		}

		private bool _autoSizePopup = true;
		[Browsable(true)]
		[Description("The width of the popup (-1 will auto-size the popup to the width of the textbox).")]
		public int PopupWidth
		{
			get
			{
				return this._popup.Width;
			}
			set
			{
				if (value == -1)
				{
					_autoSizePopup = true;
					this._popup.Width = Width;
				}
				else
				{
					_autoSizePopup = false;
					this._popup.Width = value;
				}
			}
		}

		public BorderStyle PopupBorderStyle
		{
			get
			{
				return this._list.BorderStyle;
			}
			set
			{
				this._list.BorderStyle = value;
			}
		}

		private Point popOffset = new Point(0, 0);
		[Description("The popup defaults to the lower left edge of the textbox.")]
		public Point PopupOffset
		{
			get
			{
				return this.popOffset;
			}
			set
			{
				this.popOffset = value;
			}
		}

		private Color popSelectBackColor = SystemColors.Highlight;
		public Color PopupSelectionBackColor
		{
			get
			{
				return this.popSelectBackColor;
			}
			set
			{
				this.popSelectBackColor = value;
			}
		}

		private Color popSelectForeColor = SystemColors.HighlightText;
		public Color PopupSelectionForeColor
		{
			get
			{
				return this.popSelectForeColor;
			}
			set
			{
				this.popSelectForeColor = value;
			}
		}

		private bool triggersEnabled = true;
		protected bool TriggersEnabled
		{
			get
			{
				return this.triggersEnabled;
			}
			set
			{
				this.triggersEnabled = value;
			}
		}

		public delegate IEnumerable<string> ItemFilterDelegate(string text, IEnumerable<string> items);
		private ItemFilterDelegate _itemFilterDelegate;

	  [Browsable(false)]
	[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public ItemFilterDelegate ItemFilterer
		{
			get
			{
				return this._itemFilterDelegate;
			}
			set
			{
				if(value == null)
				{
					throw new ArgumentNullException();
				}
				this._itemFilterDelegate = value;
			}
		}

		[Browsable(true)]
		public override string Text
		{
			get
			{
				return base.Text;
			}
			set
			{
				TriggersEnabled = false;
				base.Text = value;
				TriggersEnabled = true;
			}
		}

		#endregion

		public WeSayAutoCompleteTextBox()
		{
			_itemFilterDelegate = FilterList;

			// Create the form that will hold the list
			this._popup = new Form();
			this._popup.StartPosition = FormStartPosition.Manual;
			this._popup.ShowInTaskbar = false;
			this._popup.FormBorderStyle = FormBorderStyle.None;
			this._popup.TopMost = true;
			this._popup.Deactivate += new EventHandler(Popup_Deactivate);

			// Create the list box that will hold mathcing items
			this._list = new ListBox();
			this._list.Cursor = Cursors.Hand;
			this._list.BorderStyle = BorderStyle.FixedSingle;
			this._list.SelectedIndexChanged += new EventHandler(List_SelectedIndexChanged);
			this._list.MouseClick += new MouseEventHandler(List_MouseClick);
			this._list.DrawMode = DrawMode.OwnerDrawFixed;
			this._list.DrawItem += new DrawItemEventHandler(List_DrawItem);
			this._list.ItemHeight = Height;
			this._list.Dock = DockStyle.Fill;

			// Add the list box to the popup form
			this._popup.Controls.Add(this._list);

			// Add default triggers.
			this.triggers.Add(new TextLengthTrigger(2));
			this.triggers.Add(new ShortCutTrigger(Keys.Enter, TriggerState.Select));
			this.triggers.Add(new ShortCutTrigger(Keys.Tab, TriggerState.Select));
			this.triggers.Add(new ShortCutTrigger(Keys.Control | Keys.Space, TriggerState.ShowAndConsume));
			this.triggers.Add(new ShortCutTrigger(Keys.Escape, TriggerState.HideAndConsume));
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			base.OnSizeChanged(e);
			if (_list != null)
			{
				this._list.ItemHeight = Height;
			}
			if(_popup !=null && _autoSizePopup)
			{
				this._popup.Width = Width;
			}
		}
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (this._popupParent != null)
			{
				this._popupParent.Move -= Parent_Move;
				this._popupParent.ParentChanged -= popupParent_ParentChanged;
			}
			this._popupParent = Parent;
			if (this._popupParent != null)
			{
				while (this._popupParent.Parent != null)
				{
					this._popupParent = this._popupParent.Parent;
				}
				this._popupParent.Move += Parent_Move;
				this._popupParent.ParentChanged += new EventHandler(popupParent_ParentChanged);
			}
		}

		void popupParent_ParentChanged(object sender, EventArgs e)
		{
			OnParentChanged(e);
		}

		void Parent_Move(object sender, EventArgs e)
		{
			HideList();
		}

		protected virtual bool DefaultCmdKey(ref Message msg, Keys keyData)
		{
			bool val = base.ProcessCmdKey (ref msg, keyData);

			if (TriggersEnabled)
			{
				switch (Triggers.OnCommandKey(keyData))
				{
					case TriggerState.ShowAndConsume:
					{
						val = true;
						ShowList();
					} break;
					case TriggerState.Show:
					{
						ShowList();
					} break;
					case TriggerState.HideAndConsume:
					{
						val = true;
						HideList();
					} break;
					case TriggerState.Hide:
					{
						HideList();
					} break;
					case TriggerState.SelectAndConsume:
					{
						if (this._list.Visible)
						{
							val = true;
							SelectCurrentItemAndHideList();
						}
					} break;
					case TriggerState.Select:
					{
						if (this._list.Visible)
						{
							SelectCurrentItemAndHideList();
						}
					} break;
					default:
						break;
				}
			}

			return val;
		}

		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			switch (keyData)
			{
				case Keys.Up:
				{
		  Mode = EntryMode.List;
					if (this._list.SelectedIndex > 0)
					{
						this._list.SelectedIndex--;
					}
		  if (this._list.Visible == false)
		  {
			ShowList();
		  }
		  return true;
				}
				case Keys.Down:
				{
		  Mode = EntryMode.List;
					if (this._list.SelectedIndex < this._list.Items.Count - 1)
					{
						this._list.SelectedIndex++;
					}
		  if (this._list.Visible == false)
		  {
			ShowList();
		  }
		  return true;
				}
				default:
				{
					return DefaultCmdKey(ref msg, keyData);
				}
			}
		}

		protected override void OnTextChanged(EventArgs e)
		{
			base.OnTextChanged (e);

			if (TriggersEnabled)
			{
				switch (Triggers.OnTextChanged(Text))
				{
					case TriggerState.Show:
					{
						ShowList();
					} break;
					case TriggerState.Hide:
					{
						HideList();
					} break;
					default:
					{
						UpdateList();
					} break;
				}
			}
		}

		protected override void OnLostFocus(EventArgs e)
		{
			base.OnLostFocus (e);

			if (!(Focused || this._popup.Focused || this._list.Focused))
			{
				HideList();
			}
		}

		protected virtual void SelectCurrentItem()
		{
			if (this._list.SelectedIndex == -1)
			{
				return;
			}

			Focus();
			Text = this._list.SelectedItem.ToString();
			if (Text.Length > 0)
			{
				SelectionStart = Text.Length;
			}
		}

		protected void SelectCurrentItemAndHideList()
		{
			if (this._list.SelectedIndex != -1)
			{
				SelectCurrentItem();
			}
			HideList();
		}

		protected virtual void ShowList()
		{
			if (this._list.Visible == false)
			{
				this._list.SelectedIndex = -1;
				UpdateList();
				Point p = PointToScreen(new Point(0,0));
				p.X += PopupOffset.X;
				p.Y += Height + PopupOffset.Y;
				this._popup.Location = p;
				if (this._list.Items.Count > 0)
				{
					this._popup.Show();
					//this.list.BringToFront();
					Focus();
				}
			}
			else
			{
				UpdateList();
			}
		}

		protected virtual void HideList()
		{
			Mode = EntryMode.Text;
			this._popup.Hide();
		}

		protected virtual void UpdateList()
		{
			//object selectedItem = this.list.SelectedItem;

			this._list.Items.Clear();
			foreach (string item in ItemFilterer.Invoke(Text, Items))
			{
				this._list.Items.Add(item);
			}

			//if (selectedItem != null &&
			//    this.list.Items.Contains(selectedItem))
			//{
			//    EntryMode oldMode = Mode;
			//    Mode = EntryMode.List;
			//    this.list.SelectedItem = selectedItem;
			//    Mode = oldMode;
			//}

			this._list.SelectedIndex = -1;

			if (this._list.Items.Count == 0)
			{
				HideList();
			}
			else
			{
				int visItems = this._list.Items.Count;
				if (visItems > 8)
					visItems = 8;

				this._popup.Height = (visItems * this._list.ItemHeight) + 2;
				switch (BorderStyle)
				{
					case BorderStyle.FixedSingle:
					{
						this._popup.Height += 2;
						break;
					}
					case BorderStyle.Fixed3D:
					{
						this._popup.Height += 4;
						break;
					}
				}

				this._popup.Width = PopupWidth;

				//if (this.list.Items.Count > 0 &&
				//    this.list.SelectedIndex == -1)
				//{
				//    EntryMode oldMode = Mode;
				//    Mode = EntryMode.List;
				//    this.list.SelectedIndex = 0;
				//    Mode = oldMode;
				//}

			}
		}

		private static IEnumerable<string> FilterList(string text, IEnumerable<string> items)
		{
			ICollection<string> newList = new Collection<string>();
			foreach (string item in items)
			{
				if (item.ToLower().StartsWith(text.ToLower()))
				{
					newList.Add(item);
					break;
				}
			}
			return newList;
		}

		private void List_SelectedIndexChanged(object sender, EventArgs e)
		{
			if (Mode != EntryMode.List)
			{
				SelectCurrentItemAndHideList();
			}
			else
			{
				SelectCurrentItem();
			}
		}

		public event EventHandler AutoCompleteChoiceSelected = delegate {};

		private void List_MouseClick(object sender, MouseEventArgs e)
		{
			for (int i=0; i<this._list.Items.Count; i++)
			{
				if (this._list.GetItemRectangle(i).Contains(e.X, e.Y))
				{
					this._list.SelectedIndex = i;
					SelectCurrentItemAndHideList();
					AutoCompleteChoiceSelected.Invoke(this, new EventArgs());
					return;
				}
			}
			HideList();
		}

		private void List_DrawItem(object sender, DrawItemEventArgs e)
		{
			//Color bColor = e.BackColor;
			if (e.State == DrawItemState.Selected)
			{
//                e.Graphics.FillRectangle(new SolidBrush(PopupSelectionBackColor), e.Bounds);
				TextRenderer.DrawText(e.Graphics,
									  this._list.Items[e.Index].ToString(),
									  Font,
									  e.Bounds,
									  PopupSelectionForeColor,
									  PopupSelectionBackColor,
									  TextFormatFlags.Default);
			}
			else
			{
				//e.DrawBackground();
				TextRenderer.DrawText(e.Graphics,
									  this._list.Items[e.Index].ToString(),
									  Font,
									  e.Bounds,
									  ForeColor,
									  BackColor,
									  TextFormatFlags.Default);
//                e.Graphics.DrawString(this.list.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds, StringFormat.GenericDefault);
			}
		}

		private void Popup_Deactivate(object sender, EventArgs e)
		{
			if (!(Focused || this._popup.Focused || this._list.Focused))
			{
				HideList();
			}
		}

	}
}
