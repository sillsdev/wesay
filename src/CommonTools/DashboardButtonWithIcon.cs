using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Foundation;

//!!!!!!!!!!!!!! see http://www.codeproject.com/KB/GDI-plus/colormatrix.aspx  for a way
// to automatically re-color the icons. THe key search terms are "Color Matrix" and "Sepia"

namespace WeSay.CommonTools
{
	public class DashboardButtonWithIcon: DashboardButton
	{
		private readonly Image _image;
		private const int _imageWidth = 30;
		private const int _spaceBetweenImageAndLabel = 10;

		public DashboardButtonWithIcon(IThingOnDashboard thingToShowOnDashboard)
				: base(thingToShowOnDashboard)
		{
			_image = thingToShowOnDashboard.DashboardButtonImage;
		}

		public DashboardButtonWithIcon() {}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
			if (ThingToShowOnDashboard.DashboardButtonStyle == ButtonStyle.IconVariableWidth)
			{
				int labelWidth =
						TextRenderer.MeasureText(Text,
												 Font,
												 new Size(int.MaxValue, int.MaxValue),
												 TextFormatFlags.LeftAndRightPadding).Width;

				Width = _imageWidth + _spaceBetweenImageAndLabel + labelWidth + 10;
			}
		}

		protected override void PaintContents(PaintEventArgs e)
		{
			if (_image != null)
			{
				e.Graphics.DrawImage(_image,
									 ClientRectangle.Left + LeftMarginWidth +
									 CurrentMouseButtonNudge,
									 ClientRectangle.Top + CurrentMouseButtonNudge + 10,
									 _imageWidth,
									 _imageWidth);
			}

			int left = ClientRectangle.Left + _imageWidth + _spaceBetweenImageAndLabel;
			e.Graphics.DrawString(Text,
								  Font,
								  Brushes.Black,
								  left + CurrentMouseButtonNudge,
								  16 + CurrentMouseButtonNudge);
		}

		public override int GetRequiredWidth()
		{
			int textWidth =
					TextRenderer.MeasureText(Text,
											 Font,
											 new Size(int.MaxValue, int.MaxValue),
											 TextFormatFlags.LeftAndRightPadding).Width +
					ButtonDownHorizontalNudge;
			int unknownHack = 20;
			return
					textWidth + ButtonDownHorizontalNudge + _imageWidth + _spaceBetweenImageAndLabel +
					LeftMarginWidth + unknownHack;
		}
	}
}