using System;
using System.Drawing;

namespace DevAge.Windows.Forms
{
	public class CommonImages
	{
		private CommonImages()
		{
		}

		private static Image ExtractImage(string p_Image)
		{
#if !MINI
			System.Reflection.Assembly l_as = System.Reflection.Assembly.GetExecutingAssembly();
			return Image.FromStream(l_as.GetManifestResourceStream("DevAge.Res." + p_Image));
#else
			System.Reflection.Assembly l_as = System.Reflection.Assembly.GetExecutingAssembly();
			Icon tmp = new Icon(l_as.GetManifestResourceStream("DevAge.Windows.Forms.Res." + p_Image));
			Bitmap b = new Bitmap(tmp.Width, tmp.Height);
			using (Graphics g = Graphics.FromImage(b))
			{
				g.Clear(Color.Transparent);
				g.DrawIcon(tmp,0,0);
			}
			return b;
#endif
		}

		private static System.Windows.Forms.Cursor ExtractCursor(string p_Image)
		{
			System.Reflection.Assembly l_as = System.Reflection.Assembly.GetExecutingAssembly();
			return new System.Windows.Forms.Cursor(l_as.GetManifestResourceStream("DevAge.Res." + p_Image));
		}

		static CommonImages()
		{
			//these die under mono

			//m_SortDown = ExtractImage("SortDown.ico");
			//m_SortUp = ExtractImage("SortUp.ico");

			mRightArrow = ExtractCursor("right.cur");
			mLeftArrow = ExtractCursor("left.cur");


			//jdh
			m_SortDown = new Bitmap(8, 8);
			m_SortUp = new Bitmap(8, 8);
			//mRightArrow = new Bitmap(8, 8);
			//mLeftArrow = new Bitmap(8, 8);

		}

		private static Image m_SortDown;
		public static Image SortDown
		{
			get{return m_SortDown;}
		}
		private static Image m_SortUp;
		public static Image SortUp
		{
			get{return m_SortUp;}
		}
		private static System.Windows.Forms.Cursor mRightArrow;
		public static System.Windows.Forms.Cursor RightArrow
		{
			get{return mRightArrow;}
		}
		private static System.Windows.Forms.Cursor mLeftArrow;
		public static System.Windows.Forms.Cursor LeftArrow
		{
			get{return mLeftArrow;}
		}
	}
}
