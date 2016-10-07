using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace WeSay.UI.Buttons
{
	[Description("Rectangular Button Control")]
	public class RectangularImageButton: RegionButton
	{
		private Size _keepThisSize;

		public RectangularImageButton()
		{
			Size = new Size(50, 50); // Default size
			SizeChanged += Button_SizeChanged;
		}

		private void Button_SizeChanged(object sender, EventArgs e)
		{
			//we don't leave the size up to anyone; it's determined by the width of the image
			if (this.Image != null)
			{
				var sz = new Size(Image.Size.Width + 5, Image.Size.Height +5);
				if (Size.Equals(sz))
					Size = sz;
			}
		}

		protected override void MakeRegion()
		{
			GraphicsPath path = new GraphicsPath();
			Rectangle rectangle = ClientRectangle;
			path.AddRectangle(rectangle);
			Path = path;
			Invalidate();
		}
	}
}