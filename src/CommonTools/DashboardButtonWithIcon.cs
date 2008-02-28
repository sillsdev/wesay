using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace WeSay.CommonTools
{
	public class DashboardButtonWithIcon : DashboardButton
	{
		private Image _image;
		private readonly bool _isVariableWidth;
		private int _imageWidth = 30;
		private int _spaceBetweenImageAndLabel=10;

		public DashboardButtonWithIcon(Image image, bool isVariableWidth)
		 {
			_image = image;
			_isVariableWidth = isVariableWidth;
			_label.Left = ClientRectangle.Left + _imageWidth + _spaceBetweenImageAndLabel;
			_label.Width = ClientRectangle.Width - (_label.Left + 10);
			_label.Height = ClientRectangle.Height - 20;

			this.Load += new EventHandler(DashboardButtonWithIcon_Load);
		}

		void DashboardButtonWithIcon_Load(object sender, EventArgs e)
		{
			if (_isVariableWidth)
			{
				int labelWidth =
					TextRenderer.MeasureText(_label.Text, _label.Font, new Size(int.MaxValue, int.MaxValue),
											 TextFormatFlags.LeftAndRightPadding).Width;

				this.Width = _imageWidth + _spaceBetweenImageAndLabel + labelWidth + 10;
			}
		}

		protected override void PaintContents(System.Windows.Forms.PaintEventArgs e)
		{
			if (_image != null)
			{
				e.Graphics.DrawImage(_image, this.ClientRectangle.Left + 5, this.ClientRectangle.Top + 5, _imageWidth, _imageWidth);
			}

		}
	}
}
