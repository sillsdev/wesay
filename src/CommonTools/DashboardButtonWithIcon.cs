using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation.Dashboard;


//!!!!!!!!!!!!!! see http://www.codeproject.com/KB/GDI-plus/colormatrix.aspx  for a way
// to automatically re-color the icons. THe key search terms are "Color Matrix" and "Sepia"

namespace WeSay.CommonTools
{
	public class DashboardButtonWithIcon : DashboardButton
	{
		private readonly Image _image;
		private const int ImageWidth = 30;
		private const int SpaceBetweenImageAndLabel=10;

		public DashboardButtonWithIcon(IThingOnDashboard thingToShowOnDashboard)
			:base(thingToShowOnDashboard)
		 {
			_image = thingToShowOnDashboard.DashboardButtonImage;
		}

		public DashboardButtonWithIcon()
		{

		}
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (ThingToShowOnDashboard.DashboardButtonStyle == ButtonStyle.IconVariableWidth)
			{
				int labelWidth =
					TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
											 TextFormatFlags.LeftAndRightPadding).Width;

				Width = ImageWidth + SpaceBetweenImageAndLabel + labelWidth + 10;
			}
		}

		protected override void PaintContents(PaintEventArgs e)
		{
			if (_image != null)
			{
				e.Graphics.DrawImage(_image,
					ClientRectangle.Left + LeftMarginWidth + CurrentMouseButtonNudge,
					ClientRectangle.Top + CurrentMouseButtonNudge + TopMarginWidth,
					ImageWidth, ImageWidth);
			}

			int left = ClientRectangle.Left + LeftMarginWidth + ImageWidth + SpaceBetweenImageAndLabel;
			int top = ClientRectangle.Top + TopMarginWidth;
			TextRenderer.DrawText(e.Graphics, Text, Font, new Rectangle(left + CurrentMouseButtonNudge,
				top + CurrentMouseButtonNudge, ClientRectangle.Right - left - RightMarginWidth + 1,
				ClientRectangle.Bottom - BottomMarginWidth - top + 1), Color.Black, FormatFlags);
		}

		public override IEnumerable<Size> GetPossibleButtonSizes()
		{
			List<Size> textSizes = GetPossibleTextSizes();
			Dictionary<int, int> workingSizes = new Dictionary<int, int>(textSizes.Count);
			List<Size> possibleSizes = new List<Size>(textSizes.Count);
			int minimumButtonHeight = TopMarginWidth + BottomMarginWidth + ImageWidth;
			foreach (Size size in textSizes)
			{
				Size possibleSize = new Size(size.Width + LeftMarginWidth + RightMarginWidth + ImageWidth + SpaceBetweenImageAndLabel,
											 size.Height + TopMarginWidth + BottomMarginWidth);
				// if text size would be too short, adjust to fit image
				possibleSize.Height = Math.Max(possibleSize.Height, minimumButtonHeight);
				// Ensure that we only end up with the smallest width for a height.  We could end up with multiple
				// widths for the same height due to the image height check.
				if (!workingSizes.ContainsKey(possibleSize.Height))
				{
					workingSizes.Add(possibleSize.Height, possibleSize.Width);
				}
				else if (possibleSize.Width < workingSizes[possibleSize.Height])
				{
					workingSizes[possibleSize.Height] = possibleSize.Width;
				}
			}
			foreach (KeyValuePair<int, int> size in workingSizes)
			{
				possibleSizes.Add(new Size(size.Value, size.Key));
			}
			return possibleSizes;
		}

	}
}
