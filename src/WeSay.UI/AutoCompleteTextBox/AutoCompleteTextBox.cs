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

		private ListBox list;
		protected Form popup;
		private Control popupParent;

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
				return this.popup.Width;
			}
			set
			{
				if (value == -1)
				{
					_autoSizePopup = true;
					this.popup.Width = Width;
				}
				else
				{
					_autoSizePopup = false;
					this.popup.Width = value;
				}
			}
		}

		public BorderStyle PopupBorderStyle
		{
			get
			{
				return this.list.BorderStyle;
			}
			set
			{
				this.list.BorderStyle = value;
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
			this.popup = new Form();
			this.popup.StartPosition = FormStartPosition.Manual;
			this.popup.ShowInTaskbar = false;
			this.popup.FormBorderStyle = FormBorderStyle.None;
			this.popup.TopMost = true;
			this.popup.Deactivate += new EventHandler(Popup_Deactivate);

			// Create the list box that will hold mathcing items
			this.list = new ListBox();
			this.list.Cursor = Cursors.Hand;
			this.list.BorderStyle = BorderStyle.FixedSingle;
			this.list.SelectedIndexChanged += new EventHandler(List_SelectedIndexChanged);
			this.list.MouseDown += new MouseEventHandler(List_MouseDown);
			this.list.DrawMode = DrawMode.OwnerDrawFixed;
			this.list.DrawItem += new DrawItemEventHandler(List_DrawItem);
			this.list.ItemHeight = Height;
			this.list.Dock = DockStyle.Fill;

			// Add the list box to the popup form
			this.popup.Controls.Add(this.list);

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
			this.list.ItemHeight = Height;
			if(_autoSizePopup)
			{
				this.popup.Width = Width;
			}
		}
		protected override void OnParentChanged(EventArgs e)
		{
			base.OnParentChanged(e);
			if (this.popupParent != null)
			{
				this.popupParent.Move -= Parent_Move;
				this.popupParent.ParentChanged -= popupParent_ParentChanged;
			}
			this.popupParent = Parent;
			if (this.popupParent != null)
			{
				while (this.popupParent.Parent != null)
				{
					this.popupParent = this.popupParent.Parent;
				}
				this.popupParent.Move += Parent_Move;
				this.popupParent.ParentChanged += new EventHandler(popupParent_ParentChanged);
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
						if (this.list.Visible)
						{
							val = true;
							SelectCurrentItemAndHideList();
						}
					} break;
					case TriggerState.Select:
					{
						if (this.list.Visible)
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
					if (this.list.SelectedIndex > 0)
					{
						this.list.SelectedIndex--;
					}
					return true;
				}
				case Keys.Down:
				{
					Mode = EntryMode.List;
					if (this.list.SelectedIndex < this.list.Items.Count - 1)
					{
						this.list.SelectedIndex++;
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

			if (!(Focused || this.popup.Focused || this.list.Focused))
			{
				HideList();
			}
		}

		protected virtual void SelectCurrentItem()
		{
			if (this.list.SelectedIndex == -1)
			{
				return;
			}

			Focus();
			Text = this.list.SelectedItem.ToString();
			if (Text.Length > 0)
			{
				SelectionStart = Text.Length;
			}
		}

		protected void SelectCurrentItemAndHideList()
		{
			if (this.list.SelectedIndex == -1)
			{
				return;
			}

			SelectCurrentItem();
			HideList();
		}

		protected virtual void ShowList()
		{
			if (this.list.Visible == false)
			{
				this.list.SelectedItem = null;
				//this.list.SelectedIndex = -1;
				UpdateList();
				Point p = PointToScreen(new Point(0,0));
				p.X += PopupOffset.X;
				p.Y += Height + PopupOffset.Y;
				this.popup.Location = p;
				if (this.list.Items.Count > 0)
				{
					this.popup.Show();
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
			this.popup.Hide();
		}

		protected virtual void UpdateList()
		{
			//object selectedItem = this.list.SelectedItem;

			this.list.Items.Clear();
			foreach (string item in ItemFilterer.Invoke(Text, Items))
			{
				this.list.Items.Add(item);
			}

			//if (selectedItem != null &&
			//    this.list.Items.Contains(selectedItem))
			//{
			//    EntryMode oldMode = Mode;
			//    Mode = EntryMode.List;
			//    this.list.SelectedItem = selectedItem;
			//    Mode = oldMode;
			//}

			this.list.SelectedIndex = -1;

			if (this.list.Items.Count == 0)
			{
				HideList();
			}
			else
			{
				int visItems = this.list.Items.Count;
				if (visItems > 8)
					visItems = 8;

				this.popup.Height = (visItems * this.list.ItemHeight) + 2;
				switch (BorderStyle)
				{
					case BorderStyle.FixedSingle:
					{
						this.popup.Height += 2;
						break;
					}
					case BorderStyle.Fixed3D:
					{
						this.popup.Height += 4;
						break;
					}
				}

				this.popup.Width = PopupWidth;

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

		private void List_MouseDown(object sender, MouseEventArgs e)
		{
			for (int i=0; i<this.list.Items.Count; i++)
			{
				if (this.list.GetItemRectangle(i).Contains(e.X, e.Y))
				{
					this.list.SelectedIndex = i;
					SelectCurrentItemAndHideList();
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
									  this.list.Items[e.Index].ToString(),
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
									  this.list.Items[e.Index].ToString(),
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
			if (!(Focused || this.popup.Focused || this.list.Focused))
			{
				HideList();
			}
		}

	}
}
