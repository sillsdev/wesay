using System;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// Summary description for Utilities.
	/// </summary>
	public class Utilities
	{
		public Utilities()
		{
		}

		public static System.Windows.Forms.HorizontalAlignment ContentToHorizontalAlignment(DevAge.Drawing.ContentAlignment a)
		{
			if (Drawing.Utilities.IsLeft(a))
				return System.Windows.Forms.HorizontalAlignment.Left;
			else if (Drawing.Utilities.IsRight(a))
				return System.Windows.Forms.HorizontalAlignment.Right;
			else
				return System.Windows.Forms.HorizontalAlignment.Center;
		}
	}
}
