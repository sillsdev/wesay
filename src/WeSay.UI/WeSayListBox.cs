using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Palaso.WritingSystems;
using WeSay.LexicalModel.Foundation;

namespace WeSay.UI
{
	public partial class WeSayListBox: ListBox
	{
		private WritingSystemDefinition _formWritingSystem;
		private WritingSystemDefinition _meaningWritingSystem;
		private object _itemToNotDrawYet;

		public WeSayListBox()
		{
			InitializeComponent();
			//DrawMode = DrawMode.OwnerDrawVariable; //"variable" was suppose to make it actualy fire the MeasureItem, but it never did. You can set the ColumnWidth directly.
			DrawMode = DrawMode.OwnerDrawFixed;

			DrawItem += WeSayListBox_DrawItem;
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
			ItemDrawer(Items[e.Index],e);
			// If the ListBox has focus, draw a focus rectangle around the selected item.
			e.DrawFocusRectangle();
		}

		/// <summary>
		/// Change this if you need to draw something special. THe default just draws the string of the item.
		/// Make sure to make a custom MeasureItem handler too!
		/// </summary>
		public Action<object, DrawItemEventArgs> ItemDrawer;

		private void DefaultDrawItem(object item, DrawItemEventArgs e)
		{
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
							"FormWritingSystem must be initialized prior to use.");
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
			if(_meaningWritingSystem !=null)
			{
				ItemHeight += (int) (Math.Ceiling(WritingSystemInfo.CreateFont(_meaningWritingSystem).GetHeight()));
				ItemHeight += 10;//margin
			}
		}

		//used when animating additions to the list
		public object ItemToNotDrawYet
		{
			get { return _itemToNotDrawYet; }
			set {
					_itemToNotDrawYet = value;
					Refresh();
				}
		}
	}
}