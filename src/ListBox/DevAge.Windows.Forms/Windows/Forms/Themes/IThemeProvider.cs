using System;
using System.Drawing;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Interface that each Theme Provider must implement. Contains all the method to draw themed controls. For an implementation look at the ThemeProviderBase class.
	/// </summary>
	public interface IThemeProvider
	{
		void DrawHeader(System.Windows.Forms.Control control, Graphics g, Rectangle headerRect, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);
		Size MeasureHeader(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);

		void DrawRowHeader(System.Windows.Forms.Control control, Graphics g, Rectangle headerRect, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);
		Size MeasureRowHeader(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ControlDrawStyle drawStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);

		void DrawColumnHeader(System.Windows.Forms.Control control, Graphics g, Rectangle headerRect, ControlDrawStyle drawStyle, HeaderSortStyle sortStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);
		Size MeasureColumnHeader(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ControlDrawStyle drawStyle, HeaderSortStyle sortStyle, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);

		void DrawSortArrow(System.Windows.Forms.Control control, Graphics g, Rectangle contentRect, HeaderSortStyle sortStyle);

		void DrawButton(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ButtonDrawStyle drawStyle, bool focused, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);
		Size MeasureButton(System.Windows.Forms.Control control, DevAge.Drawing.MeasureHelper measureHelper, ButtonDrawStyle drawStyle, bool focused, string text, StringFormat stringFormat, Image image, DevAge.Drawing.ContentAlignment imageAlignment, bool alignTextToImage);

		void DrawDropDownButton(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle);

		void DrawCloseButton(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle);

		void DrawCheckBox(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle, bool bChecked);

		void DrawCheckBoxWithLabel(System.Windows.Forms.Control control, Graphics g, Rectangle rect, ControlDrawStyle drawStyle, bool bChecked, DevAge.Drawing.ContentAlignment checkAlignment, String label, StringFormat stringFormat, Font labelFont, Color labelColor);

		Size MeasureCheckBox();

		void DrawEditableControlPanel(System.Windows.Forms.Control control, Graphics g, Rectangle rect);
	}

	public enum HeaderSortStyle
	{
		None = 0,
		Ascending = 1,
		Descending = 2
	}

	public enum ButtonDrawStyle
	{
		Normal = 1,
		Pressed = 2,
		Hot = 3,
		Disabled = 4,
		DefaultButton = 5
	}

	public enum ControlDrawStyle
	{
		Normal = 1,
		Pressed = 2,
		Hot = 3,
		Disabled = 4
	}
}
