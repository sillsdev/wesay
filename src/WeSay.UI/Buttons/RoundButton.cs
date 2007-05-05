using System.Drawing;
using System.Drawing.Drawing2D;
using System.ComponentModel;

namespace WeSay.UI.Buttons
{
	[Description("Round Button Control")]
	public class RoundButton : RegionButton
	{
		public RoundButton()
		{
			Size = new Size(50, 50); // Default size
		}

		protected override void MakeRegion()
		{
			GraphicsPath path = new GraphicsPath();
			Rectangle rectangle = ClientRectangle;
			path.AddEllipse(rectangle);
			Path = path;
			Invalidate();
		}

	}
}
