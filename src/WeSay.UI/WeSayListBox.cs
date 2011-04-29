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
		private WritingSystemDefinition _writingSystem;
		private object _itemToNotDrawYet;

		public WeSayListBox()
		{
			InitializeComponent();
			// Set the DrawMode property to draw fixed sized items.
			DrawMode = DrawMode.OwnerDrawFixed;

			DrawItem += WeSayListBox_DrawItem;
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
			// Define the default color of the brush as black.
			Brush myBrush = Brushes.Black;

			// Draw the current item text based on the current Font and the custom brush settings.
			TextRenderer.DrawText(e.Graphics, Items[e.Index].ToString(), e.Font, e.Bounds, Color.Black, TextFormatFlags.Left);
			//Do not use Graphics.Drawstring as it does not use Uniscribe and thus has problems with complex scripts WS-14881
			//e.Graphics.DrawString(Items[e.Index].ToString(),
			//                      e.Font,
			//                      myBrush,
			//                      e.Bounds,
			//                      StringFormat.GenericDefault);
			// If the ListBox has focus, draw a focus rectangle around the selected item.
			e.DrawFocusRectangle();
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			//    base.OnPaint(e);
		}

		[Browsable(false)]
		[DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
		public WritingSystemDefinition WritingSystem
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
				Font = WritingSystemInfo.CreateFont(value);
				ItemHeight = (int)(Math.Ceiling(WritingSystemInfo.CreateFont(value).GetHeight()));
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