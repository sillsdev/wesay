using System;
using System.Drawing;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Base implementation of a IThemeProvider interface. This class is used for basic drawing without any special theme, using standard .NET drawing methods, usually using System.Windows.Forms.ControlPaint class.
	/// </summary>
	public class ThemeProviderBase : IThemeProvider
	{
		public ThemeProviderBase()
		{
		}

		public virtual void DrawHeader(System.Windows.Forms.Control control, Graphics g, Rectangle headerRect, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Rectangle contentRectangle = new Rectangle(headerRect.X + 3, headerRect.Y + 3, headerRect.Width - 6, headerRect.Height - 6);

			System.Windows.Forms.ControlPaint.DrawButton(g, headerRect, ButtonStateFromDrawStyle(drawStyle, false));

			bool disabled = false;
			if (drawStyle == ControlDrawStyle.Disabled)
				disabled = true;

			Drawing.Utilities.DrawTextAndImage(g, contentRectangle, image, imageAlignment, false, disabled, text, stringFormat, alignTextToImage, Color.FromKnownColor(KnownColor.ControlText), System.Windows.Forms.Control.DefaultFont, disabled);
		}

		public virtual Size MeasureHeader(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Size size = new Size(3, 3); //3 pixel for the border width

			Size textImageSize = measureHelper.MeasureStringAndImage(text, stringFormat, System.Windows.Forms.Control.DefaultFont, image, imageAlignment, alignTextToImage, new Size(2000, 2000));

			size.Width += textImageSize.Width;
			size.Height += textImageSize.Height;

			return size;
		}

		public virtual void DrawRowHeader(System.Windows.Forms.Control control, Graphics g, Rectangle headerRect, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Rectangle contentRectangle = new Rectangle(headerRect.X + 3, headerRect.Y + 3, headerRect.Width - 6, headerRect.Height - 6);

			System.Windows.Forms.ControlPaint.DrawButton(g, headerRect, ButtonStateFromDrawStyle(drawStyle, false));

			bool disabled = false;
			if (drawStyle == ControlDrawStyle.Disabled)
				disabled = true;

			Drawing.Utilities.DrawTextAndImage(g, contentRectangle, image, imageAlignment, false, disabled, text, stringFormat, alignTextToImage, Color.FromKnownColor(KnownColor.ControlText), System.Windows.Forms.Control.DefaultFont, disabled);
		}

		public virtual Size MeasureRowHeader(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Size size = new Size(3, 3); //3 pixel for the border width

			Size textImageSize = measureHelper.MeasureStringAndImage(text, stringFormat, System.Windows.Forms.Control.DefaultFont, image, imageAlignment, alignTextToImage, new Size(2000, 2000));

			size.Width += textImageSize.Width;
			size.Height += textImageSize.Height;

			return size;
		}
		public virtual void DrawColumnHeader(System.Windows.Forms.Control control, Graphics g, Rectangle headerRect, ControlDrawStyle drawStyle, HeaderSortStyle sortStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			////Remove some pixel for the themed border
			Rectangle innerRectangle = new Rectangle(headerRect.X + 3, headerRect.Y + 3,
													 headerRect.Width - 6, headerRect.Height - 6);

			Rectangle contentRectangle = innerRectangle;

			//remove the width of sort indicator if present
			if (sortStyle != HeaderSortStyle.None)
			{
				int sortWidth = CommonImages.SortDown.Width;
				if (contentRectangle.Width > sortWidth)
					contentRectangle.Width -= sortWidth;
			}

			System.Windows.Forms.ControlPaint.DrawButton(g, headerRect, ButtonStateFromDrawStyle(drawStyle, false));

			bool disabled = false;
			if (drawStyle == ControlDrawStyle.Disabled)
				disabled = true;

			Drawing.Utilities.DrawTextAndImage(g, contentRectangle, image, imageAlignment, false, disabled, text, stringFormat, alignTextToImage, Color.FromKnownColor(KnownColor.ControlText), System.Windows.Forms.Control.DefaultFont, disabled);

			DrawSortArrow(control, g, innerRectangle, sortStyle);
		}

		public virtual Size MeasureColumnHeader(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ControlDrawStyle drawStyle, HeaderSortStyle sortStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Size size = new Size(6, 6); //3 pixel for the border width (approximation ...)

			//Pixel for the Sort icon
			if (sortStyle == HeaderSortStyle.Ascending || sortStyle == HeaderSortStyle.Descending)
				size.Width += CommonImages.SortDown.Width;

			 Size textImageSize = measureHelper.MeasureStringAndImage(text, stringFormat, System.Windows.Forms.Control.DefaultFont, image, imageAlignment, alignTextToImage, new Size(2000, 2000));

			size.Width += textImageSize.Width;
			size.Height += textImageSize.Height;

			return size;
		}

		public virtual void DrawSortArrow(System.Windows.Forms.Control control, Graphics g, Rectangle contentRect, HeaderSortStyle sortStyle)
		{
			if (sortStyle == HeaderSortStyle.Ascending)
				Drawing.Utilities.DrawTextAndImage(g, contentRect, CommonImages.SortUp, Drawing.ContentAlignment.MiddleRight, false, false, null, null, false, Color.Black, null, false);
			else if (sortStyle == HeaderSortStyle.Descending)
				Drawing.Utilities.DrawTextAndImage(g, contentRect, CommonImages.SortDown, Drawing.ContentAlignment.MiddleRight, false, false, null, null, false, Color.Black, null, false);
		}
		public virtual void DrawButton(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ButtonDrawStyle drawStyle, bool focused, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Rectangle contentRectangle = new Rectangle(rect.X + 3, rect.Y + 3, rect.Width - 6, rect.Height - 6);
			System.Windows.Forms.ControlPaint.DrawButton(g, rect, ButtonStateFromButtonDrawStyle(drawStyle));

			bool disabled = false;
			if (drawStyle == ButtonDrawStyle.Disabled)
				disabled = true;

			Drawing.Utilities.DrawTextAndImage(g, contentRectangle, image, imageAlignment, false, disabled, text, stringFormat, alignTextToImage, Color.FromKnownColor(KnownColor.ControlText), System.Windows.Forms.Control.DefaultFont, disabled);

			if (focused)
				System.Windows.Forms.ControlPaint.DrawFocusRectangle(g, contentRectangle);
		}
		public virtual Size MeasureButton(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ButtonDrawStyle drawStyle, bool focused, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage)
		{
			Size size = new Size(3, 3); //3 pixel for the border width

			Size textImageSize = measureHelper.MeasureStringAndImage(text, stringFormat, System.Windows.Forms.Control.DefaultFont, image, imageAlignment, alignTextToImage, new Size(2000, 2000));

			size.Width += textImageSize.Width;
			size.Height += textImageSize.Height;

			return size;
		}

		public virtual void DrawDropDownButton(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle)
		{
			System.Windows.Forms.ControlPaint.DrawComboButton(g, rect, ButtonStateFromDrawStyle(drawStyle, false));
		}

		public virtual void DrawCloseButton(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle)
		{
			System.Windows.Forms.ControlPaint.DrawCaptionButton(g, rect, System.Windows.Forms.CaptionButton.Close, ButtonStateFromDrawStyle(drawStyle, false));
		}

		public virtual void DrawCheckBox(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle, bool bChecked)
		{
			Rectangle checkBoxRect = Drawing.Utilities.CalculateContentRectangle(Drawing.ContentAlignment.MiddleCenter, rect, MeasureCheckBox() );
			System.Windows.Forms.ControlPaint.DrawCheckBox(g, checkBoxRect, ButtonStateFromDrawStyle(drawStyle, bChecked));
		}

		public virtual void DrawCheckBoxWithLabel(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle, bool bChecked, DevAge.Drawing.ContentAlignment checkAlignment, String label, StringFormat stringFormat, Font labelFont, Color labelColor)
		{
			if (rect.Width <= 0 || rect.Height <= 0)
				return;

			//CheckBox
			Size sizeCheck = MeasureCheckBox();
			Rectangle rectCheck = Drawing.Utilities.CalculateContentRectangle(checkAlignment, rect, sizeCheck);
			DrawCheckBox(control, g, rectCheck, drawStyle, bChecked);

			//Text
			if (label != null && label.Length > 0)
			{
				Rectangle rectDrawText = Drawing.Utilities.CalculateTextRectangleWithContent(rect, label, stringFormat, rectCheck, checkAlignment);

				if (rectDrawText.Width <= 0 || rectDrawText.Height <= 0)
					return;

				Drawing.Utilities.DrawString(g, rectDrawText, label, stringFormat, labelColor, labelFont);
			}
		}

		public virtual Size MeasureCheckBox()
		{
			return System.Windows.Forms.SystemInformation.MenuCheckSize;
		}

		private System.Windows.Forms.ButtonState ButtonStateFromButtonDrawStyle(ButtonDrawStyle drawStyle)
		{
			switch (drawStyle)
			{
				case ButtonDrawStyle.Disabled:
					return System.Windows.Forms.ButtonState.Inactive;
				case ButtonDrawStyle.Hot:
				case ButtonDrawStyle.DefaultButton:
				case ButtonDrawStyle.Normal:
					return System.Windows.Forms.ButtonState.Normal;
				case ButtonDrawStyle.Pressed:
					return System.Windows.Forms.ButtonState.Pushed;
				default:
					throw new ApplicationException("Invalid ControlDrawStyle enum value.");
			}
		}

		private System.Windows.Forms.ButtonState ButtonStateFromDrawStyle(ControlDrawStyle drawStyle, bool pChecked)
		{
			System.Windows.Forms.ButtonState checkedState;
			if (pChecked)
				checkedState = System.Windows.Forms.ButtonState.Checked;
			else
				checkedState = (System.Windows.Forms.ButtonState)0;

			switch (drawStyle)
			{
				case ControlDrawStyle.Disabled:
					return System.Windows.Forms.ButtonState.Inactive | checkedState;
				case ControlDrawStyle.Hot:
					return System.Windows.Forms.ButtonState.Normal | checkedState;
				case ControlDrawStyle.Normal:
					return System.Windows.Forms.ButtonState.Normal | checkedState;
				case ControlDrawStyle.Pressed:
					return System.Windows.Forms.ButtonState.Pushed | checkedState;
				default:
					throw new ApplicationException("Invalid ControlDrawStyle enum value.");
			}
		}

		public virtual void DrawEditableControlPanel(System.Windows.Forms.Control control, Graphics g, Rectangle rect)
		{
			using (SolidBrush solidBrush = new SolidBrush(System.Drawing.Color.FromKnownColor(KnownColor.Window)))
				g.FillRectangle(solidBrush, rect);
			System.Windows.Forms.ControlPaint.DrawBorder3D(g, rect, System.Windows.Forms.Border3DStyle.Sunken);
		}
	}
}
