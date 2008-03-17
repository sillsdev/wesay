using System;
using System.Drawing;
using System.Windows.Forms;
using WeSay.Project;


//!!!!!!!!!!!!!! see http://www.codeproject.com/KB/GDI-plus/colormatrix.aspx  for a way
// to automatically re-color the icons. THe key search terms are "Color Matrix" and "Sepia"

namespace WeSay.CommonTools
{
	public class DashboardButtonWithIcon : DashboardButton
	{
		private Image _image;
		private int _imageWidth = 30;
		private int _spaceBetweenImageAndLabel=10;

		public DashboardButtonWithIcon(IThingOnDashboard thingToShowOnDashboard)
			:base(thingToShowOnDashboard)
		 {
			_image = thingToShowOnDashboard.Image;
			this.Load += new EventHandler(DashboardButtonWithIcon_Load);
		}

		void DashboardButtonWithIcon_Load(object sender, EventArgs e)
		{
			if (_thingToShowOnDashboard.Style == ButtonStyle.IconVariableWidth)
			{
				int labelWidth =
					TextRenderer.MeasureText(Text, Font, new Size(int.MaxValue, int.MaxValue),
											 TextFormatFlags.LeftAndRightPadding).Width;

				this.Width = _imageWidth + _spaceBetweenImageAndLabel + labelWidth + 10;
			}
		}

		protected override void PaintContents(System.Windows.Forms.PaintEventArgs e)
		{
			if (_image != null)
			{
				e.Graphics.DrawImage(_image,
					this.ClientRectangle.Left + _leftMarginWidth + CurrentMouseButtonNudge,
					this.ClientRectangle.Top + CurrentMouseButtonNudge + 10,
					_imageWidth, _imageWidth);
			}

			int left = ClientRectangle.Left + _imageWidth + _spaceBetweenImageAndLabel;
			e.Graphics.DrawString(this.Text, this.Font, Brushes.Black, left + CurrentMouseButtonNudge, 16 + CurrentMouseButtonNudge);
		}

		public override int GetRequiredWidth()
		{
			int textWidth = TextRenderer.MeasureText(Text, this.Font, new Size(int.MaxValue, int.MaxValue), TextFormatFlags.LeftAndRightPadding).Width + _buttonDownHorizontalNudge;
			int unknownHack = 20;
			return textWidth + _buttonDownHorizontalNudge + _imageWidth +
				_spaceBetweenImageAndLabel + _leftMarginWidth + unknownHack;
		}

	}
}
