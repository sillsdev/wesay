using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using SIL.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI
{
	public interface IWeSayListBox
	{
		[Browsable(false)]
		string Text { set; get; }

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		WritingSystemDefinition FormWritingSystem { get; set; }
		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		WritingSystemDefinition MeaningWritingSystem { get; set; }

		void Clear();
		void AddItem(Object item);
		void AddRange(Object[] items);
		Point Location { get; set; }
		object GetItem(int index);
		int Length { get; }
		int SelectedIndex { get; set; }
		bool MultiColumn { get; set; }
		Object ItemToNotDrawYet { get; set; }
		Object SelectedItem { get; }
		string Name { get; set; }
		Size Size { get; set; }
		Font Font { get; set; }
		DockStyle Dock { get; set; }
		Size MinimumSize { get; set; }
		AnchorStyles Anchor { get; set; }
		BorderStyle BorderStyle { get; set; }
		Color BackColor { get; set; }
		DrawMode DrawMode { get; set; }
		bool TabStop { get; set; }
		int TabIndex { get; set; }
		int ItemHeight { get; set; }
		int Top { get; set; }
		int Height { get; set; }
		int ColumnWidth { get; set; }
		Rectangle GetItemRectangle(int index);
		Control Control { get; }
		bool Sorted { get; set; }

		bool DisplayMeaning { get; set; }
		void ItemToHtml(string word, int index, bool useFormWS, Color textColor);
		void ListCompleted();

		Action<object, object> ItemDrawer { get; set; }
		void SetBounds(int x, int y, int width, int height);
		bool Focus();

		event EventHandler UserClick;
		event KeyPressEventHandler KeyPress;
	}

	public partial class WeSayListBox : ListBox, IWeSayListBox
	{
		private WritingSystemDefinition _formWritingSystem;
		private WritingSystemDefinition _meaningWritingSystem;
		private object _itemToNotDrawYet;
		public event EventHandler UserClick;

		public WeSayListBox()
		{
			InitializeComponent();
			//DrawMode = DrawMode.OwnerDrawVariable; //"variable" was suppose to make it actualy fire the MeasureItem, but it never did. You can set the ColumnWidth directly.
			DrawMode = DrawMode.OwnerDrawFixed;

			DrawItem += WeSayListBox_DrawItem;
			Click += WeSayListBox_Click;
			ItemDrawer = DefaultDrawItem;
		}

		private void WeSayListBox_DrawItem(object sender, DrawItemEventArgs e)
		{
			//surprisingly, this *is* called with bogus values, by the system
			if (e.Index < 0 || e.Index >= Items.Count)
			{
				return;
			}

			if (Items[e.Index] == _itemToNotDrawYet)
			{
				return;
			}
			// Draw the background of the ListBox control for each item.
			e.DrawBackground();
			ItemDrawer(Items[e.Index], e);
			// If the ListBox has focus, draw a focus rectangle around the selected item.
			e.DrawFocusRectangle();
		}
		private void WeSayListBox_Click(object sender, EventArgs e)
		{
			if (UserClick != null)
			{
				UserClick.Invoke(sender, e);
			}
		}
		/// <summary>
		/// Change this if you need to draw something special. THe default just draws the string of the item.
		/// Make sure to make a custom MeasureItem handler too!
		///
		/// <param name="item"> The first object is the object to be added to the list and will be interpreted by the caller
		/// in the drawing routine and when accessed.  If the default drawer is used, it is assumed that it can be represented
		/// in the list by ToString.</param>
		/// <param name="a" > The second parameter when calling the WeSayListBox will be a DrawItemEventArgs.</param>
		/// </summary>
		public Action<object, object> ItemDrawer { get; set; }

		private void DefaultDrawItem(object item, object a)
		{
			DrawItemEventArgs e = (DrawItemEventArgs)a;
			// Draw the current item text based on the current Font and the custom brush settings.
			TextRenderer.DrawText(e.Graphics, item.ToString(), e.Font, e.Bounds, Color.Black, TextFormatFlags.Left);
			//Do not use Graphics.Drawstring as it does not use Uniscribe and thus has problems with complex scripts WS-14881
			//e.Graphics.DrawString(Items[e.Index].ToString(),
			//                      e.Font,
			//                      myBrush,
			//                      e.Bounds,
			//                      StringFormat.GenericDefault);
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//    base.OnPaint(e);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystemDefinition FormWritingSystem
		{
			get
			{
				if (_formWritingSystem == null)
				{
					throw new InvalidOperationException(
						"Form input system must be initialized prior to use.");
				}
				return _formWritingSystem;
			}
			set
			{
				if (value == null)
				{
					throw new ArgumentNullException();
				}
				_formWritingSystem = value;
				Font = WritingSystemInfo.CreateFont(value);
				if (value.RightToLeftScript)
				{
					RightToLeft = RightToLeft.Yes;
				}
				else
				{
					RightToLeft = RightToLeft.No;
				}
				ComputeItemHeight();
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
				ComputeItemHeight();
			}
		}

		private void ComputeItemHeight()
		{
			ItemHeight = (int) (Math.Ceiling(WritingSystemInfo.CreateFont(_formWritingSystem).GetHeight()));
			if (_meaningWritingSystem != null)
			{
				ItemHeight += (int) (Math.Ceiling(WritingSystemInfo.CreateFont(_meaningWritingSystem).GetHeight()));
				ItemHeight += 10; //margin
			}
		}

		//used when animating additions to the list
		public object ItemToNotDrawYet
		{
			get { return _itemToNotDrawYet; }
			set
			{
				_itemToNotDrawYet = value;
				Refresh();
			}
		}

		public void AddItem(Object item)
		{
			this.Items.Add(item);
		}

		public void AddRange(Object[] items)
		{
			this.Items.AddRange(items);
		}

		public object GetItem(int index)
		{
			return Items[index];
		}

		public void Clear()
		{
			this.Items.Clear();
		}

		public int Length
		{
			get
			{
				return this.Items.Count;
			}
		}

		public Control Control
		{
			get
			{
				return this;
			}
		}

		public bool DisplayMeaning { get; set; }
		public void ListCompleted()
		{
			// Gecko Only method
		}
		public void ItemToHtml(string word, int index, bool useFormWS, Color textColor)
		{
			// Gecko Only method
		}

	}
}