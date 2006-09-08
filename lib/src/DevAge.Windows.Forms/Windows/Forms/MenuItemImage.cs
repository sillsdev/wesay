using System;

namespace DevAge.Windows.Forms
{
	/// <summary>
	/// A menu with Image support, using Chris.Beckett.MenuImageLib
	/// </summary>
	public class MenuItemImage : System.Windows.Forms.MenuItem
	{
		public MenuItemImage ( System.Windows.Forms.MenuMerge mergeType , System.Int32 mergeOrder , System.Windows.Forms.Shortcut shortcut , System.String text , System.EventHandler onClick , System.EventHandler onPopup , System.EventHandler onSelect , System.Windows.Forms.MenuItem[] items )
			:base( mergeType , mergeOrder , shortcut , text , onClick , onPopup , onSelect , items )
		{
		}

		public MenuItemImage ( System.String text , System.Windows.Forms.MenuItem[] items )
			:base(text, items)
		{
		}

		public MenuItemImage ( System.String text , System.EventHandler onClick , System.Windows.Forms.Shortcut shortcut )
			:base(text, onClick, shortcut )
		{

		}

		public MenuItemImage ( System.String text , System.EventHandler onClick )
			:base(text, onClick)
		{
		}

		public MenuItemImage ( System.String text )
			:base(text)
		{
		}

		public MenuItemImage (  )
			:base()
		{
		}


		public MenuItemImage ( System.String text , System.EventHandler onClick , System.Drawing.Image p_Image)
			:base(text, onClick)
		{
			SetImage(p_Image);
		}

		public MenuItemImage ( System.String text , System.Drawing.Image p_Image)
			:base(text)
		{
			SetImage(p_Image);
		}


		public MenuItemImage ( System.String text , System.EventHandler onClick , System.Windows.Forms.ImageList p_ImageList, int p_ImageIndex)
			:base(text, onClick)
		{
			SetImage(p_ImageList, p_ImageIndex);
		}

		public MenuItemImage ( System.String text , System.Windows.Forms.ImageList p_ImageList, int p_ImageIndex)
			:base(text)
		{
			SetImage(p_ImageList, p_ImageIndex);
		}

		private Chris.Beckett.MenuImageLib.MenuImage m_ImageLib = null;

		/// <summary>
		/// Set the image associated with this menu, this method can be called only one time.
		/// </summary>
		/// <param name="p_Image"></param>
		public void SetImage(System.Drawing.Image p_Image)
		{
			System.Windows.Forms.ImageList l_ImageList = new System.Windows.Forms.ImageList();
			l_ImageList.Images.Add(p_Image);

			SetImage(l_ImageList, 0);
		}

		/// <summary>
		/// Set the image associated with this menu, this method can be called only one time.
		/// </summary>
		/// <param name="p_ImageList"></param>
		/// <param name="p_ImageIndex"></param>
		public void SetImage(System.Windows.Forms.ImageList p_ImageList, int p_ImageIndex)
		{
			if (m_ImageLib!=null)
				throw new ApplicationException("SetImage already called");

			m_ImageLib = new Chris.Beckett.MenuImageLib.MenuImage();
			m_ImageLib.ImageList = p_ImageList;

			m_ImageLib.SetMenuImage(this, p_ImageIndex.ToString());
		}
	}
}
